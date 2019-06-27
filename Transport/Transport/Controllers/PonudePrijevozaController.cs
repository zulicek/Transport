using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Transport.Extensions;
using Transport.Models;
using Transport.ViewModels;

namespace Transport.Controllers
{
    public class PonudePrijevozaController : Controller
    {
        private readonly transportContext _context;

        public PonudePrijevozaController(transportContext context)
        {
            _context = context;
        }

        // GET: PonudePrijevoza
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ApplicationUser user = _context.Users.Where(u => u.Id == userId).FirstOrDefault();

            PonudeVozilaViewModel ponudeVozilaViewModel = new PonudeVozilaViewModel();
            ponudeVozilaViewModel.PonudePrijevoza = _context.PonudaPrijevoza.Include(z => z.IdPrijevoznikNavigation)
                                                                .Where(z => z.IdPrijevoznik == user.IdKorisnik)
                                                                .Include(z => z.IdPrijevoznikNavigation.IdKorisnikNavigation)
                                                                .Include(z => z.IdStatusPonudeNavigation)
                                                                .Include(z => z.IdZahtjevNavigation)
                                                                .Include(z => z.IdZahtjevNavigation.IdNaruciteljNavigation)
                                                                 .Include(z => z.IdZahtjevNavigation.IdNaruciteljNavigation.IdKorisnikNavigation)
                                                                .ToList();
            ponudeVozilaViewModel.Vozila = _context.Vozilo.Include(z => z.IdPrijevoznikNavigation).ToList();
            ponudeVozilaViewModel.Prijevozi = _context.Prijevoz.ToList();

            return View(ponudeVozilaViewModel);
        }


        // GET: PonudePrijevoza/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ponudaPrijevoza = await _context.PonudaPrijevoza
                .Include(p => p.IdPrijevoznikNavigation)
                .Include(p => p.IdStatusPonudeNavigation)
                .Include(p => p.IdZahtjevNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (ponudaPrijevoza == null)
            {
                return NotFound();
            }

            return View(ponudaPrijevoza);
        }

        // GET: PonudePrijevoza/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PonudePrijevoza/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        //public async Task<IActionResult> Create([Bind("Id,Cijena,RokIstekaPonude,RokOtkazaPonude,CijenaOtkaza,IdZahtjev,IdPrijevoznik,IdStatusPonude")] PonudaPrijevoza ponudaPrijevoza)
        public async Task<IActionResult> Create(int IdZahtjev, [Bind("Id,Cijena,RokIstekaPonude,RokOtkazaPonude,CijenaOtkaza")] PonudaPrijevoza ponudaPrijevoza)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ApplicationUser user = _context.Users.Where(u => u.Id == userId).FirstOrDefault();


            if (ModelState.IsValid)
            {
               
                //mijenjanje statusa zahtjeva u PristiglePonude ako već nije
                Zahtjev zahtjev = _context.Zahtjev.Where(z => z.Id == IdZahtjev)
                            .Include(z => z.IdStatusZahtjevaNavigation)
                            .Include(z => z.IdNaruciteljNavigation.IdKorisnikNavigation).FirstOrDefault();

                if (zahtjev.IdStatusZahtjevaNavigation.Status == "Otvoreno")
                {
                    StatusZahtjeva status = _context.StatusZahtjeva.Where(s => s.Status == "PristiglePonude").FirstOrDefault();
                    zahtjev.IdStatusZahtjeva = status.Id;
                    _context.Zahtjev.Update(zahtjev);
                }

                PonudaPrijevoza novaPonudaPrijevoza = new PonudaPrijevoza
                {
                    Id = ponudaPrijevoza.Id,
                    Cijena = ponudaPrijevoza.Cijena,
                    RokIstekaPonude = ponudaPrijevoza.RokIstekaPonude,
                    RokOtkazaPonude = ponudaPrijevoza.RokOtkazaPonude,
                    CijenaOtkaza = ponudaPrijevoza.CijenaOtkaza,
                    IdZahtjev = IdZahtjev,
                    IdPrijevoznik = user.IdKorisnik,
                    IdStatusPonude = 1
                };

                _context.Add(novaPonudaPrijevoza);
                    
                await _context.SaveChangesAsync();
                TempData[Constants.Message] = $"Ponuda prijevoza dodana.";
                TempData[Constants.ErrorOccurred] = false;

                PonudaPrijevoza ponuda = _context.PonudaPrijevoza.Where(p => p.Id == ponudaPrijevoza.Id).
                    Include(p => p.IdPrijevoznikNavigation.IdKorisnikNavigation).FirstOrDefault();


                if (zahtjev.IdNaruciteljNavigation.IdKorisnikNavigation.PrimaEmail)
                {
                    var smtpClient = new SmtpClient
                    {
                        Host = "smtp.gmail.com", // set your SMTP server name here
                        Port = 587, // Port 
                        EnableSsl = true,
                        Credentials = new NetworkCredential("carryontransport@gmail.com", "carryon1")
                    };

                    using (var message = new MailMessage("carryontransport@gmail.com", zahtjev.IdNaruciteljNavigation.IdKorisnikNavigation.Email)
                    {
                        Subject = "Primljena ponuda",
                        Body = "Poštovani korisniče " + zahtjev.IdNaruciteljNavigation.IdKorisnikNavigation.Ime + " " 
                            + zahtjev.IdNaruciteljNavigation.IdKorisnikNavigation.Prezime + ", zaprimili ste ponudu prijevoza od korisnika "
                            + ponuda.IdPrijevoznikNavigation.IdKorisnikNavigation.Ime + " " +
                            ponuda.IdPrijevoznikNavigation.IdKorisnikNavigation.Prezime + ". Ponuda se odnosi na zahtjev s opisom: " 
                            + zahtjev.Opis + "."
                    })
                    {
                        await smtpClient.SendMailAsync(message);
                    }
                }

                string urlString = "Details/" + IdZahtjev.ToString();
                return RedirectToAction(urlString, "ProsireniZahtjevi");
            }
            ViewData["IdPrijevoznik"] = new SelectList(_context.Prijevoznik, "IdKorisnik", "IdKorisnik", ponudaPrijevoza.IdPrijevoznik);
            ViewData["IdStatusPonude"] = new SelectList(_context.StatusPonude, "Id", "Status", ponudaPrijevoza.IdStatusPonude);
            ViewData["IdZahtjev"] = new SelectList(_context.Zahtjev, "Id", "Opis", ponudaPrijevoza.IdZahtjev);
            return View(ponudaPrijevoza);
        }

        // GET: PonudePrijevoza/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ponudaPrijevoza = await _context.PonudaPrijevoza.SingleOrDefaultAsync(m => m.Id == id);
            if (ponudaPrijevoza == null)
            {
                return NotFound();
            }
            ViewData["IdPrijevoznik"] = new SelectList(_context.Prijevoznik, "IdKorisnik", "IdKorisnik", ponudaPrijevoza.IdPrijevoznik);
            ViewData["IdStatusPonude"] = new SelectList(_context.StatusPonude, "Id", "Status", ponudaPrijevoza.IdStatusPonude);
            ViewData["IdZahtjev"] = new SelectList(_context.Zahtjev, "Id", "Opis", ponudaPrijevoza.IdZahtjev);
            return View(ponudaPrijevoza);
        }

        // POST: PonudePrijevoza/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Cijena,RokIstekaPonude,RokOtkazaPonude,CijenaOtkaza,IdZahtjev,IdPrijevoznik,IdStatusPonude")] PonudaPrijevoza ponudaPrijevoza)
        {
            if (id != ponudaPrijevoza.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ponudaPrijevoza);
                    await _context.SaveChangesAsync();
                    TempData[Constants.Message] = "Ažuriranje uspješno obavljeno";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PonudaPrijevozaExists(ponudaPrijevoza.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdPrijevoznik"] = new SelectList(_context.Prijevoznik, "IdKorisnik", "IdKorisnik", ponudaPrijevoza.IdPrijevoznik);
            ViewData["IdStatusPonude"] = new SelectList(_context.StatusPonude, "Id", "Status", ponudaPrijevoza.IdStatusPonude);
            ViewData["IdZahtjev"] = new SelectList(_context.Zahtjev, "Id", "Opis", ponudaPrijevoza.IdZahtjev);
            return View(ponudaPrijevoza);
        }

        // GET: PonudePrijevoza/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ponudaPrijevoza = await _context.PonudaPrijevoza
                .Include(p => p.IdPrijevoznikNavigation)
                .Include(p => p.IdStatusPonudeNavigation)
                .Include(p => p.IdZahtjevNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (ponudaPrijevoza == null)
            {
                return NotFound();
            }

            return View(ponudaPrijevoza);
        }

        // POST: PonudePrijevoza/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var ponudaPrijevoza = await _context.PonudaPrijevoza.SingleOrDefaultAsync(m => m.Id == id);
                _context.PonudaPrijevoza.Remove(ponudaPrijevoza);
                await _context.SaveChangesAsync();

                //mijenjanje statusa zahtjeva u Otvoreno ako se briše zadnja ponuda
                Zahtjev zahtjev = _context.Zahtjev.Where(z => z.Id == ponudaPrijevoza.IdZahtjev).Include(z => z.IdStatusZahtjevaNavigation).FirstOrDefault();
                var ponude = _context.PonudaPrijevoza.Where(p => p.IdZahtjev == zahtjev.Id).ToList();

                if (ponude.Count == 0)
                {
                    StatusZahtjeva status = _context.StatusZahtjeva.Where(s => s.Status == "Otvoreno").FirstOrDefault();
                    zahtjev.IdStatusZahtjevaNavigation = status;
                    _context.Zahtjev.Update(zahtjev);
                    await _context.SaveChangesAsync();
                }

                TempData[Constants.Message] = $"Ponuda prijevoza uspješno obrisan";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception exc)
            {
                TempData[Constants.Message] = "Pogreška prilikom brisanja ponude prijevoza: " + exc.CompleteExceptionMessage();
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PonudaPrijevozaExists(int id)
        {
            return _context.PonudaPrijevoza.Any(e => e.Id == id);
        }

        // POST: Rezerviraj ponudu
        public async Task<IActionResult> Rezervacija(int IdPonuda)
        {
            var ponuda = Rezervacije(IdPonuda, "PristiglaRezervacija");
            TempData[Constants.Message] = $"Poslan zahtjev za rezervacijom ponude";
            TempData[Constants.ErrorOccurred] = false;

            await _context.SaveChangesAsync();

            if (ponuda.IdPrijevoznikNavigation.IdKorisnikNavigation.PrimaEmail)
            {
                var smtpClient = new SmtpClient
                {
                    Host = "smtp.gmail.com", // set your SMTP server name here
                    Port = 587, // Port 
                    EnableSsl = true,
                    Credentials = new NetworkCredential("carryontransport@gmail.com", "carryon1")
                };

                using (var message = new MailMessage("carryontransport@gmail.com", ponuda.IdPrijevoznikNavigation.IdKorisnikNavigation.Email)
                {
                    Subject = "Primljen zahtjev za registracijom",
                    Body = "Poštovani korisniče " + ponuda.IdPrijevoznikNavigation.IdKorisnikNavigation.Ime + " " +
                    ponuda.IdPrijevoznikNavigation.IdKorisnikNavigation.Prezime + ", zaprimili ste zahtjev za rezervacijom od korisnika " +
                    ponuda.IdZahtjevNavigation.IdNaruciteljNavigation.IdKorisnikNavigation.Ime + " " + ponuda.IdZahtjevNavigation.IdNaruciteljNavigation.IdKorisnikNavigation.Prezime +
                    ". Rezervacija se odnosi na zahtjev s opisom: " + ponuda.IdZahtjevNavigation.Opis + "."
                })
                {
                    await smtpClient.SendMailAsync(message);
                }
            }

            string urlString = "MojZahtjev/" + ponuda.IdZahtjev.ToString();
            return RedirectToAction(urlString, "ProsireniZahtjevi");
        }

        // POST: Otkaži zahtjev za rezervacijom
        public async Task<IActionResult> OtkaziZahtjevRezervacije(int IdPonuda)
        {
             var ponuda = Rezervacije(IdPonuda, "Otvoreno");

            TempData[Constants.Message] = $"Rezervacija ponude prijevoza uspješno otkazana";
            TempData[Constants.ErrorOccurred] = false;


            await _context.SaveChangesAsync();
            string urlString = "MojZahtjev/" + ponuda.IdZahtjev.ToString();
            return RedirectToAction(urlString, "ProsireniZahtjevi");
        }

        public async Task<IActionResult> OtkaziZahtjevRezervacijePrijevoznik(int IdPonuda)
        {
            var ponuda = Rezervacije(IdPonuda, "Otvoreno");

            TempData[Constants.Message] = $"Rezervacija ponude prijevoza uspješno otkazana";
            TempData[Constants.ErrorOccurred] = false;

            await _context.SaveChangesAsync();

            if (ponuda.IdZahtjevNavigation.IdNaruciteljNavigation.IdKorisnikNavigation.PrimaEmail)
            {
                var smtpClient = new SmtpClient
                {
                    Host = "smtp.gmail.com", // set your SMTP server name here
                    Port = 587, // Port 
                    EnableSsl = true,
                    Credentials = new NetworkCredential("carryontransport@gmail.com", "carryon1")
                };

                using (var message = new MailMessage("carryontransport@gmail.com", ponuda.IdZahtjevNavigation.IdNaruciteljNavigation.IdKorisnikNavigation.Email)
                {
                    Subject = "Otkazan zahtjev za registracijom",
                    Body = "Poštovani korisniče " + ponuda.IdZahtjevNavigation.IdNaruciteljNavigation.IdKorisnikNavigation.Ime + " " +
                    ponuda.IdZahtjevNavigation.IdNaruciteljNavigation.IdKorisnikNavigation.Prezime + ", Vaš zahtjev za rezervacijom ponude prijevoza od korisnika " +
                    ponuda.IdPrijevoznikNavigation.IdKorisnikNavigation.Ime + " " + ponuda.IdPrijevoznikNavigation.IdKorisnikNavigation.Prezime +
                    " je otkazan. Rezervacija se odnosila na zahtjev s opisom: " + ponuda.IdZahtjevNavigation.Opis + "."
                })
                {
                    await smtpClient.SendMailAsync(message);
                }
            }
            string urlString = "Details/" + ponuda.IdZahtjev.ToString();
            return RedirectToAction(urlString, "ProsireniZahtjevi");
        }

        // POST: Otkaži zahtjev za rezervacijom
        public async Task<IActionResult> PrihvatiRezervaciju(int IdPonuda)
        {
            PonudaPrijevoza ponuda = _context.PonudaPrijevoza.Where(p => p.Id == IdPonuda)
                .Include(p => p.IdPrijevoznikNavigation.IdKorisnikNavigation)
                .Include(p => p.IdZahtjevNavigation.IdNaruciteljNavigation.IdKorisnikNavigation)
                .Include(p => p.IdStatusPonudeNavigation).FirstOrDefault();
            StatusPonude status = _context.StatusPonude.Where(s => s.Status == "Rezervirano").FirstOrDefault();
            ponuda.IdStatusPonude = status.Id;
            _context.PonudaPrijevoza.Update(ponuda);

            Zahtjev zahtjev = _context.Zahtjev.Where(z => z.Id == ponuda.IdZahtjev).FirstOrDefault();
            StatusZahtjeva rezervirano = _context.StatusZahtjeva.Where(s => s.Status == "Rezervirano").FirstOrDefault();
            zahtjev.IdStatusZahtjeva = rezervirano.Id;
            _context.Zahtjev.Update(zahtjev);

            Prijevoz prijevoz = new Prijevoz
            {
                IdPonudaPrijevoza = IdPonuda
            };
            _context.Prijevoz.Add(prijevoz);


            await _context.SaveChangesAsync();
            TempData[Constants.Message] = $"Rezervacija ponude uspješna";
            TempData[Constants.ErrorOccurred] = false;

            if (ponuda.IdZahtjevNavigation.IdNaruciteljNavigation.IdKorisnikNavigation.PrimaEmail)
            {
                var smtpClient = new SmtpClient
                {
                    Host = "smtp.gmail.com", // set your SMTP server name here
                    Port = 587, // Port 
                    EnableSsl = true,
                    Credentials = new NetworkCredential("carryontransport@gmail.com", "carryon1")
                };

                using (var message = new MailMessage("carryontransport@gmail.com", ponuda.IdZahtjevNavigation.IdNaruciteljNavigation.IdKorisnikNavigation.Email)
                {
                    Subject = "Uspješna rezervacija prijevoza",
                    Body = "Poštovani korisniče " + ponuda.IdZahtjevNavigation.IdNaruciteljNavigation.IdKorisnikNavigation.Ime + " " +
                    ponuda.IdZahtjevNavigation.IdNaruciteljNavigation.IdKorisnikNavigation.Prezime + ", Vaš zahtjev za rezervacijom ponude prijevoza od korisnika " +
                    ponuda.IdPrijevoznikNavigation.IdKorisnikNavigation.Ime + " " + ponuda.IdPrijevoznikNavigation.IdKorisnikNavigation.Prezime +
                    " je uspješna. Rezervacija se odnosila na zahtjev s opisom: " + ponuda.IdZahtjevNavigation.Opis + "."
                })
                {
                    await smtpClient.SendMailAsync(message);
                }
            }

            string urlString = "Details/" + ponuda.IdZahtjev.ToString();
            return RedirectToAction(urlString, "ProsireniZahtjevi");
        }

        public PonudaPrijevoza Rezervacije(int IdPonuda, string noviStatus)
        {
            PonudaPrijevoza ponuda = _context.PonudaPrijevoza.Where(p => p.Id == IdPonuda)
                .Include(p => p.IdStatusPonudeNavigation)
                .Include(p => p.IdPrijevoznikNavigation.IdKorisnikNavigation)
                .Include(p => p.IdZahtjevNavigation.IdNaruciteljNavigation.IdKorisnikNavigation)
                .FirstOrDefault();

            StatusPonude status = _context.StatusPonude.Where(s => s.Status == noviStatus).FirstOrDefault();

            ponuda.IdStatusPonude = status.Id;
            _context.PonudaPrijevoza.Update(ponuda);

            return ponuda;
        }

    }
}
