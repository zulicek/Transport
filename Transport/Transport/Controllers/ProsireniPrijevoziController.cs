using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Transport.Models;
using Transport.ViewModels;

namespace Transport.Controllers
{
    public class ProsireniPrijevoziController : Controller
    {
        private readonly transportContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProsireniPrijevoziController(transportContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ProsireniPrijevozs
        public IActionResult Index()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ApplicationUser user = _context.Users.Where(u => u.Id == userId).FirstOrDefault();

           
            var  prijevozi = _context.Prijevoz.Include(p => p.IdPonudaPrijevozaNavigation)
                                                   .Include(p => p.IdPonudaPrijevozaNavigation.IdPrijevoznikNavigation)
                                                   .Include(p => p.IdPonudaPrijevozaNavigation.IdPrijevoznikNavigation.IdKorisnikNavigation)
                                                   .Include(p => p.IdPonudaPrijevozaNavigation.IdZahtjevNavigation)
                                                   .Include(p => p.IdPonudaPrijevozaNavigation.IdZahtjevNavigation.IdStatusZahtjevaNavigation)
                                                   .Include(p => p.IdPonudaPrijevozaNavigation.IdZahtjevNavigation.IdNaruciteljNavigation)
                                                   .Include(p => p.IdPonudaPrijevozaNavigation.IdZahtjevNavigation.IdNaruciteljNavigation.IdKorisnikNavigation)
                                                   .Where(p => p.IdPonudaPrijevozaNavigation.IdPrijevoznik == user.IdKorisnik ||
                                                        p.IdPonudaPrijevozaNavigation.IdZahtjevNavigation.IdNarucitelj == user.IdKorisnik)
                                                    .ToList();


            var prosireniPrijevozi = new List<ProsireniPrijevozViewModel>();

            if (prijevozi.Count != 0)
            {
                foreach (var prijevoz in prijevozi)
                {
                    var zahtjev = _context.Zahtjev.Where(m => m.Id == prijevoz.IdPonudaPrijevozaNavigation.IdZahtjevNavigation.Id)
                        .Include(m => m.IdStatusZahtjevaNavigation)
                        .Include(m => m.IdNaruciteljNavigation).FirstOrDefault();

                    var lokacijeZahtjeva = _context.ZahtjevLokacija.Where(m => m.IdZahtjev == zahtjev.Id)
                                                .Include(z => z.IdLokacijaNavigation)
                                                .Include(z => z.IdLokacijaNavigation.IdVrstaLokacijeNavigation)
                                                .Include(z => z.IdLokacijaNavigation.IdMjestoNavigation);

                    Lokacija pocetnaLokacija = null;
                    Lokacija odredisnaLokacija = null;

                    foreach (var lokacija in lokacijeZahtjeva)
                    {

                        if (lokacija.IdLokacijaNavigation.IdVrstaLokacijeNavigation.Vrsta == "početna")
                        {

                            pocetnaLokacija = lokacija.IdLokacijaNavigation;
                        }
                        else
                        {
                            odredisnaLokacija = lokacija.IdLokacijaNavigation;
                        }
                    }

                    Mjesto pocetnoMjesto = pocetnaLokacija.IdMjestoNavigation;
                    Mjesto odredisnoMjesto = odredisnaLokacija.IdMjestoNavigation;


                    ProsireniZahtjevViewModel prosireniZahtjev = new ProsireniZahtjevViewModel();

                    prosireniZahtjev.Id = zahtjev.Id;
                    prosireniZahtjev.VrijemePocetka = zahtjev.VrijemePocetka;
                    prosireniZahtjev.VrijemeZavrsetka = zahtjev.VrijemeZavrsetka;
                    prosireniZahtjev.CijenaNeizvrsenja = zahtjev.CijenaNeizvrsenja;
                    prosireniZahtjev.Sirina = zahtjev.Sirina;
                    prosireniZahtjev.Visina = zahtjev.Visina;
                    prosireniZahtjev.Duiljina = zahtjev.Duiljina;
                    prosireniZahtjev.Masa = zahtjev.Masa;
                    prosireniZahtjev.Opis = zahtjev.Opis;
                    prosireniZahtjev.IdNarucitelj = zahtjev.IdNarucitelj;
                    prosireniZahtjev.IdNaruciteljNavigation = zahtjev.IdNaruciteljNavigation;
                    prosireniZahtjev.IdStatusZahtjeva = zahtjev.IdStatusZahtjeva;
                    prosireniZahtjev.IdStatusZahtjevaNavigation = zahtjev.IdStatusZahtjevaNavigation;
                    prosireniZahtjev.PocetnaLokacijaUlica = pocetnaLokacija.Ulica;
                    prosireniZahtjev.PocetnaLokacijaKucniBroj = pocetnaLokacija.KucniBroj;
                    prosireniZahtjev.PocetnoMjesto = pocetnoMjesto.Naziv;
                    prosireniZahtjev.PocetnoMjestoPbr = pocetnoMjesto.PostanskiBroj;
                    prosireniZahtjev.OdredisnaLokacijaUlica = odredisnaLokacija.Ulica;
                    prosireniZahtjev.OdredisnaLokacijaKucniBroj = odredisnaLokacija.KucniBroj;
                    prosireniZahtjev.OdredisnoMjesto = odredisnoMjesto.Naziv;
                    prosireniZahtjev.OdredisnoMjestoPbr = odredisnoMjesto.PostanskiBroj;

                    ProsireniPrijevozViewModel prosireniPrijevoz = new ProsireniPrijevozViewModel
                    {
                        Id = prijevoz.Id,
                        Prijevoz = prijevoz,
                        ProsireniZahtjev = prosireniZahtjev
                    };

                    prosireniPrijevozi.Add(prosireniPrijevoz);
                }
           

            }

                return View(prosireniPrijevozi);
        }

        public IActionResult SviPrijevozi()
        {
            var prijevozi = _context.Prijevoz.Include(p => p.IdPonudaPrijevozaNavigation)
                                                  .Include(p => p.IdPonudaPrijevozaNavigation.IdStatusPonudeNavigation)
                                                  .Include(p => p.IdPonudaPrijevozaNavigation.IdPrijevoznikNavigation)
                                                  .Include(p => p.IdPonudaPrijevozaNavigation.IdPrijevoznikNavigation.IdKorisnikNavigation)
                                                  .Include(p => p.IdPonudaPrijevozaNavigation.IdZahtjevNavigation)
                                                  .Include(p => p.IdPonudaPrijevozaNavigation.IdZahtjevNavigation.IdStatusZahtjevaNavigation)
                                                  .Include(p => p.IdPonudaPrijevozaNavigation.IdZahtjevNavigation.IdNaruciteljNavigation)
                                                  .Include(p => p.IdPonudaPrijevozaNavigation.IdZahtjevNavigation.IdNaruciteljNavigation.IdKorisnikNavigation)
                                                  .ToList();


            var prosireniPrijevozi = new List<ProsireniPrijevozViewModel>();

            if (prijevozi.Count != 0)
            {
                foreach (var prijevoz in prijevozi)
                {
                    var zahtjev = _context.Zahtjev.Where(m => m.Id == prijevoz.IdPonudaPrijevozaNavigation.IdZahtjevNavigation.Id)
                        .Include(m => m.IdStatusZahtjevaNavigation)
                        .Include(m => m.IdNaruciteljNavigation).FirstOrDefault();

                    var lokacijeZahtjeva = _context.ZahtjevLokacija.Where(m => m.IdZahtjev == zahtjev.Id)
                                                .Include(z => z.IdLokacijaNavigation)
                                                .Include(z => z.IdLokacijaNavigation.IdVrstaLokacijeNavigation)
                                                .Include(z => z.IdLokacijaNavigation.IdMjestoNavigation);

                    Lokacija pocetnaLokacija = null;
                    Lokacija odredisnaLokacija = null;

                    foreach (var lokacija in lokacijeZahtjeva)
                    {

                        if (lokacija.IdLokacijaNavigation.IdVrstaLokacijeNavigation.Vrsta == "početna")
                        {

                            pocetnaLokacija = lokacija.IdLokacijaNavigation;
                        }
                        else
                        {
                            odredisnaLokacija = lokacija.IdLokacijaNavigation;
                        }
                    }

                    Mjesto pocetnoMjesto = pocetnaLokacija.IdMjestoNavigation;
                    Mjesto odredisnoMjesto = odredisnaLokacija.IdMjestoNavigation;


                    ProsireniZahtjevViewModel prosireniZahtjev = new ProsireniZahtjevViewModel();

                    prosireniZahtjev.Id = zahtjev.Id;
                    prosireniZahtjev.VrijemePocetka = zahtjev.VrijemePocetka;
                    prosireniZahtjev.VrijemeZavrsetka = zahtjev.VrijemeZavrsetka;
                    prosireniZahtjev.CijenaNeizvrsenja = zahtjev.CijenaNeizvrsenja;
                    prosireniZahtjev.Sirina = zahtjev.Sirina;
                    prosireniZahtjev.Visina = zahtjev.Visina;
                    prosireniZahtjev.Duiljina = zahtjev.Duiljina;
                    prosireniZahtjev.Masa = zahtjev.Masa;
                    prosireniZahtjev.Opis = zahtjev.Opis;
                    prosireniZahtjev.IdNarucitelj = zahtjev.IdNarucitelj;
                    prosireniZahtjev.IdNaruciteljNavigation = zahtjev.IdNaruciteljNavigation;
                    prosireniZahtjev.IdStatusZahtjeva = zahtjev.IdStatusZahtjeva;
                    prosireniZahtjev.IdStatusZahtjevaNavigation = zahtjev.IdStatusZahtjevaNavigation;
                    prosireniZahtjev.PocetnaLokacijaUlica = pocetnaLokacija.Ulica;
                    prosireniZahtjev.PocetnaLokacijaKucniBroj = pocetnaLokacija.KucniBroj;
                    prosireniZahtjev.PocetnoMjesto = pocetnoMjesto.Naziv;
                    prosireniZahtjev.PocetnoMjestoPbr = pocetnoMjesto.PostanskiBroj;
                    prosireniZahtjev.OdredisnaLokacijaUlica = odredisnaLokacija.Ulica;
                    prosireniZahtjev.OdredisnaLokacijaKucniBroj = odredisnaLokacija.KucniBroj;
                    prosireniZahtjev.OdredisnoMjesto = odredisnoMjesto.Naziv;
                    prosireniZahtjev.OdredisnoMjestoPbr = odredisnoMjesto.PostanskiBroj;

                    ProsireniPrijevozViewModel prosireniPrijevoz = new ProsireniPrijevozViewModel
                    {
                        Id = prijevoz.Id,
                        Prijevoz = prijevoz,
                        ProsireniZahtjev = prosireniZahtjev
                    };

                    prosireniPrijevozi.Add(prosireniPrijevoz);
                }


            }

            return View(prosireniPrijevozi);
        }

        // GET: ProsireniPrijevozs/Details/5
        public IActionResult Details(int? id)
        {

            ProsireniPrijevozViewModel prosireniPrijevoz = vratiPrijevoz(id);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ApplicationUser user = _context.Users.Where(u => u.Id == userId).FirstOrDefault();

            if (user.IdKorisnik == prosireniPrijevoz.ProsireniZahtjev.IdNarucitelj)
            {
                string urlString = "NaruciteljPrijevoz/" + id.ToString();
                return RedirectToAction(urlString);
            } else
            {
                string urlString = "PrijevoznikPrijevoz/" + id.ToString();
                return RedirectToAction(urlString);
            }
        }

        public IActionResult NaruciteljPrijevoz (int id)
        {
            ProsireniPrijevozViewModel prosireniPrijevoz = vratiPrijevoz(id);
            ViewData["IdRazlog"] = new SelectList(_context.RazlogNaplate, "Id", "Razlog");
            return View(prosireniPrijevoz);
        }

        public IActionResult PrijevoznikPrijevoz(int id)
        {
            ProsireniPrijevozViewModel prosireniPrijevoz = vratiPrijevoz(id);
            ViewData["IdRazlog"] = new SelectList(_context.RazlogNaplate, "Id", "Razlog");
            return View(prosireniPrijevoz);
        }

        public ProsireniPrijevozViewModel vratiPrijevoz(int? id)
        {
            var prijevoz = _context.Prijevoz.Where(p => p.Id == id)
                                                  .Include(p => p.IdPonudaPrijevozaNavigation)
                                                  .Include(p => p.IdPonudaPrijevozaNavigation.IdStatusPonudeNavigation)
                                                  .Include(p => p.IdPonudaPrijevozaNavigation.IdPrijevoznikNavigation)
                                                  .Include(p => p.IdPonudaPrijevozaNavigation.IdPrijevoznikNavigation.IdKorisnikNavigation)
                                                  .Include(p => p.IdPonudaPrijevozaNavigation.IdZahtjevNavigation)
                                                  .Include(p => p.IdPonudaPrijevozaNavigation.IdZahtjevNavigation.IdStatusZahtjevaNavigation)
                                                  .Include(p => p.IdPonudaPrijevozaNavigation.IdZahtjevNavigation.IdNaruciteljNavigation)
                                                  .Include(p => p.IdPonudaPrijevozaNavigation.IdZahtjevNavigation.IdNaruciteljNavigation.IdKorisnikNavigation)
                                                  .FirstOrDefault();

            var zahtjev = _context.Zahtjev.Where(m => m.Id == prijevoz.IdPonudaPrijevozaNavigation.IdZahtjevNavigation.Id)
                        .Include(m => m.IdStatusZahtjevaNavigation)
                        .Include(m => m.IdNaruciteljNavigation).FirstOrDefault();

            var lokacijeZahtjeva = _context.ZahtjevLokacija.Where(m => m.IdZahtjev == zahtjev.Id)
                                        .Include(z => z.IdLokacijaNavigation)
                                        .Include(z => z.IdLokacijaNavigation.IdVrstaLokacijeNavigation)
                                        .Include(z => z.IdLokacijaNavigation.IdMjestoNavigation);

            Lokacija pocetnaLokacija = null;
            Lokacija odredisnaLokacija = null;

            foreach (var lokacija in lokacijeZahtjeva)
            {

                if (lokacija.IdLokacijaNavigation.IdVrstaLokacijeNavigation.Vrsta == "početna")
                {

                    pocetnaLokacija = lokacija.IdLokacijaNavigation;
                }
                else
                {
                    odredisnaLokacija = lokacija.IdLokacijaNavigation;
                }
            }

            Mjesto pocetnoMjesto = pocetnaLokacija.IdMjestoNavigation;
            Mjesto odredisnoMjesto = odredisnaLokacija.IdMjestoNavigation;


            ProsireniZahtjevViewModel prosireniZahtjev = new ProsireniZahtjevViewModel();

            prosireniZahtjev.Id = zahtjev.Id;
            prosireniZahtjev.VrijemePocetka = zahtjev.VrijemePocetka;
            prosireniZahtjev.VrijemeZavrsetka = zahtjev.VrijemeZavrsetka;
            prosireniZahtjev.CijenaNeizvrsenja = zahtjev.CijenaNeizvrsenja;
            prosireniZahtjev.Sirina = zahtjev.Sirina;
            prosireniZahtjev.Visina = zahtjev.Visina;
            prosireniZahtjev.Duiljina = zahtjev.Duiljina;
            prosireniZahtjev.Masa = zahtjev.Masa;
            prosireniZahtjev.Opis = zahtjev.Opis;
            prosireniZahtjev.IdNarucitelj = zahtjev.IdNarucitelj;
            prosireniZahtjev.IdNaruciteljNavigation = zahtjev.IdNaruciteljNavigation;
            prosireniZahtjev.IdStatusZahtjeva = zahtjev.IdStatusZahtjeva;
            prosireniZahtjev.IdStatusZahtjevaNavigation = zahtjev.IdStatusZahtjevaNavigation;
            prosireniZahtjev.PocetnaLokacijaUlica = pocetnaLokacija.Ulica;
            prosireniZahtjev.PocetnaLokacijaKucniBroj = pocetnaLokacija.KucniBroj;
            prosireniZahtjev.PocetnoMjesto = pocetnoMjesto.Naziv;
            prosireniZahtjev.PocetnoMjestoPbr = pocetnoMjesto.PostanskiBroj;
            prosireniZahtjev.OdredisnaLokacijaUlica = odredisnaLokacija.Ulica;
            prosireniZahtjev.OdredisnaLokacijaKucniBroj = odredisnaLokacija.KucniBroj;
            prosireniZahtjev.OdredisnoMjesto = odredisnoMjesto.Naziv;
            prosireniZahtjev.OdredisnoMjestoPbr = odredisnoMjesto.PostanskiBroj;


            var naplate = _context.Naplata.Where(n => n.IdPrijevoz == prijevoz.Id)
                                            .Include(n => n.IdVrstaNaplateNavigation)
                                            .Include(n => n.IdRazlogNavigation).ToList();
            Naplata naplataNarucitelju = null;
            Naplata naplataPrijevozniku = null;
            if (naplate.Count != 0)
            {
                foreach (var naplata in naplate)
                {
                    if (naplata.IdVrstaNaplateNavigation.Vrsta == "naručitelju")
                    {
                        naplataNarucitelju = _context.Naplata.Where(n => n.IdPrijevoz == prijevoz.Id).FirstOrDefault();
                    }
                    else if (naplata.IdVrstaNaplateNavigation.Vrsta == "prijevozniku")
                    {
                        naplataPrijevozniku = _context.Naplata.Where(n => n.IdPrijevoz == prijevoz.Id).FirstOrDefault();
                    }
                }
            }

            ProsireniPrijevozViewModel prosireniPrijevoz = new ProsireniPrijevozViewModel
            {
                Id = prijevoz.Id,
                Prijevoz = prijevoz,
                ProsireniZahtjev = prosireniZahtjev,
                NaplataNaručitelju = naplataNarucitelju,
                NaplataPrijevozniku = naplataPrijevozniku
            };

            return prosireniPrijevoz;
        }

        // GET: ProsireniPrijevozs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ProsireniPrijevozs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id")] ProsireniPrijevozViewModel prosireniPrijevoz)
        {
            if (ModelState.IsValid)
            {
                _context.Add(prosireniPrijevoz);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(prosireniPrijevoz);
        }

        // GET: ProsireniPrijevozs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prosireniPrijevoz = await _context.ProsireniPrijevoz.SingleOrDefaultAsync(m => m.Id == id);
            if (prosireniPrijevoz == null)
            {
                return NotFound();
            }
            return View(prosireniPrijevoz);
        }

        // POST: ProsireniPrijevozs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id")] ProsireniPrijevozViewModel prosireniPrijevoz)
        {
            if (id != prosireniPrijevoz.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prosireniPrijevoz);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProsireniPrijevozExists(prosireniPrijevoz.Id))
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
            return View(prosireniPrijevoz);
        }

        // GET: ProsireniPrijevozs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prosireniPrijevoz = await _context.ProsireniPrijevoz
                .SingleOrDefaultAsync(m => m.Id == id);
            if (prosireniPrijevoz == null)
            {
                return NotFound();
            }

            return View(prosireniPrijevoz);
        }

        // POST: ProsireniPrijevozs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var prosireniPrijevoz = await _context.ProsireniPrijevoz.SingleOrDefaultAsync(m => m.Id == id);
            _context.ProsireniPrijevoz.Remove(prosireniPrijevoz);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProsireniPrijevozExists(int id)
        {
            return _context.ProsireniPrijevoz.Any(e => e.Id == id);
        }

        public async Task<IActionResult> GotovPrijevoz (string StatusZahtjeva, int IdPrijevoz)
        {
            var prijevoz = _context.Prijevoz.Where(p => p.Id == IdPrijevoz)
                                                  .Include(p => p.IdPonudaPrijevozaNavigation)
                                                  .Include(p => p.IdPonudaPrijevozaNavigation.IdPrijevoznikNavigation)
                                                  .Include(p => p.IdPonudaPrijevozaNavigation.IdPrijevoznikNavigation.IdKorisnikNavigation)
                                                  .Include(p => p.IdPonudaPrijevozaNavigation.IdZahtjevNavigation)
                                                  .Include(p => p.IdPonudaPrijevozaNavigation.IdZahtjevNavigation.IdStatusZahtjevaNavigation)
                                                  .Include(p => p.IdPonudaPrijevozaNavigation.IdZahtjevNavigation.IdNaruciteljNavigation)
                                                  .Include(p => p.IdPonudaPrijevozaNavigation.IdZahtjevNavigation.IdNaruciteljNavigation.IdKorisnikNavigation)
                                                  .FirstOrDefault();

            var zahtjev = _context.Zahtjev.Where(m => m.Id == prijevoz.IdPonudaPrijevozaNavigation.IdZahtjev).FirstOrDefault();

            StatusZahtjeva noviStatus = _context.StatusZahtjeva.Where(s => s.Status == StatusZahtjeva).FirstOrDefault();

            zahtjev.IdStatusZahtjeva = noviStatus.Id;
            _context.Zahtjev.Update(zahtjev);

            /*if (prijevoz.IdPonudaPrijevozaNavigation.IdPrijevoznikNavigation.IdKorisnikNavigation.PrimaEmail)
            {
                var smtpClient = new SmtpClient
                {
                    Host = "smtp.gmail.com", // set your SMTP server name here
                    Port = 587, // Port 
                    EnableSsl = true,
                    Credentials = new NetworkCredential("carryontransport@gmail.com", "carryon1")
                };

                using (var message = new MailMessage("carryontransport@gmail.com", prijevoz.IdPonudaPrijevozaNavigation.IdPrijevoznikNavigation.IdKorisnikNavigation.Email)
                {
                    Subject = "Informacije o prijevozu",
                    Body = "Poštovani korisniče " + prijevoz.IdPonudaPrijevozaNavigation.IdPrijevoznikNavigation.IdKorisnikNavigation.Ime + " " +
                    prijevoz.IdPonudaPrijevozaNavigation.IdPrijevoznikNavigation.IdKorisnikNavigation.Prezime + ", Vaš prijevoz je korisnik " +
                    zahtjev.IdNaruciteljNavigation.IdKorisnikNavigation.Ime + " " + zahtjev.IdNaruciteljNavigation.IdKorisnikNavigation.Prezime +
                    " označio kao " + noviStatus.Status + "."
                })
                {
                    await smtpClient.SendMailAsync(message);
                }
            }*/
            await _context.SaveChangesAsync();

            string urlString = "Details/" + prijevoz.Id.ToString();
            return RedirectToAction(urlString);
        }


        public async Task<IActionResult> GotovPrijevozPrijevoznik(string StatusPonude, int IdPrijevoz)
        {
            var prijevoz = _context.Prijevoz.Where(p => p.Id == IdPrijevoz)
                                                  .Include(p => p.IdPonudaPrijevozaNavigation)
                                                  .Include(p => p.IdPonudaPrijevozaNavigation.IdStatusPonudeNavigation)
                                                  .Include(p => p.IdPonudaPrijevozaNavigation.IdPrijevoznikNavigation)
                                                  .Include(p => p.IdPonudaPrijevozaNavigation.IdPrijevoznikNavigation.IdKorisnikNavigation)
                                                  .Include(p => p.IdPonudaPrijevozaNavigation.IdZahtjevNavigation)
                                                  .Include(p => p.IdPonudaPrijevozaNavigation.IdZahtjevNavigation.IdStatusZahtjevaNavigation)
                                                  .Include(p => p.IdPonudaPrijevozaNavigation.IdZahtjevNavigation.IdNaruciteljNavigation)
                                                  .Include(p => p.IdPonudaPrijevozaNavigation.IdZahtjevNavigation.IdNaruciteljNavigation.IdKorisnikNavigation)
                                                  .FirstOrDefault();

            StatusPonude noviStatus = _context.StatusPonude.Where(s => s.Status == StatusPonude).FirstOrDefault();

            PonudaPrijevoza ponuda = _context.PonudaPrijevoza.Where(p => p.Id == prijevoz.IdPonudaPrijevoza).FirstOrDefault();

            prijevoz.IdPonudaPrijevozaNavigation.IdStatusPonude = noviStatus.Id;
            _context.PonudaPrijevoza.Update(ponuda);

            /*if (ponuda.IdZahtjevNavigation.IdNaruciteljNavigation.IdKorisnikNavigation.PrimaEmail)
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
                    Subject = "Informacije o prijevozu",
                    Body = "Poštovani korisniče " + ponuda.IdZahtjevNavigation.IdNaruciteljNavigation.IdKorisnikNavigation.Ime + " " +
                    ponuda.IdZahtjevNavigation.IdNaruciteljNavigation.IdKorisnikNavigation.Prezime + ", Vaš prijevoz je korisnik " +
                    ponuda.IdPrijevoznikNavigation.IdKorisnikNavigation.Ime + " " + ponuda.IdPrijevoznikNavigation.IdKorisnikNavigation.Prezime +
                    " označio kao " + noviStatus.Status + "."
                })
                {
                    await smtpClient.SendMailAsync(message);
                }
            }*/

            await _context.SaveChangesAsync();

            string urlString = "Details/" + prijevoz.Id.ToString();
            return RedirectToAction(urlString);
        }

        //Ocijenjivanje naručitelja
        public async Task<IActionResult> OcijeniNarucitelja(int IdPrijevoz, int Ocjena, string OpisUsluge, string IdeNaplata, int IdRazlog, DateTime RokIzvrsenjaNaplate)
        {
            var prijevoz = _context.Prijevoz.Where(p => p.Id == IdPrijevoz).Include(p => p.IdPonudaPrijevozaNavigation).FirstOrDefault();
            prijevoz.OcjenaNarucitelja = Ocjena;
            prijevoz.OpisUslugeNarucitelja = OpisUsluge;

            _context.Prijevoz.Update(prijevoz);

            if (IdeNaplata == "on")
            {
                VrstaNaplate vrstaNaplate = _context.VrstaNaplate.Where(v => v.Vrsta == "naručitelju").FirstOrDefault();

                Naplata naplataNarucitelju = new Naplata
                {
                    IdVrstaNaplate = vrstaNaplate.Id,
                    IdPrijevoz = IdPrijevoz,
                    IdRazlog = IdRazlog,
                    RokIzvrsenjaNaplate = RokIzvrsenjaNaplate
                };
                _context.Naplata.Add(naplataNarucitelju);
            }
           

            await _context.SaveChangesAsync();

            ViewData["IdRazlog"] = new SelectList(_context.RazlogNaplate, "Id", "Razlog");

            string urlString = "Details/" + prijevoz.Id.ToString();
            return RedirectToAction(urlString);
        }

        //Ocijenjivanje prijevoznika
        public async Task<IActionResult> OcijeniPrijevoznika(int IdPrijevoz, int Ocjena, string OpisUsluge, bool IdeNaplata, int IdRazlog, DateTime RokIzvrsenjaNaplate)
        {
            var prijevoz = _context.Prijevoz.Where(p => p.Id == IdPrijevoz).FirstOrDefault();
            prijevoz.OcjenaPrijevoznika = Ocjena;
            prijevoz.OpisUslugePrijevoznika = OpisUsluge;

            _context.Prijevoz.Update(prijevoz);

            if (IdeNaplata)
            {
                VrstaNaplate vrstaNaplate = _context.VrstaNaplate.Where(v => v.Vrsta == "prijevozniku").FirstOrDefault();

                Naplata naplataPrijevozniku = new Naplata
                {
                    IdVrstaNaplate = vrstaNaplate.Id,
                    IdPrijevoz = IdPrijevoz,
                    IdRazlog = IdRazlog,
                    RokIzvrsenjaNaplate = RokIzvrsenjaNaplate
                };
                _context.Add(naplataPrijevozniku);
            }
            
            await _context.SaveChangesAsync();

            ViewData["IdRazlog"] = new SelectList(_context.RazlogNaplate, "Id", "Razlog");

            string urlString = "Details/" + prijevoz.Id.ToString();
            return RedirectToAction(urlString);
        }

        public async Task<IActionResult> OtkaziPrijevoz(int IdPrijevoz)
        {
            var prijevoz = _context.Prijevoz.Where(p => p.Id == IdPrijevoz)
                                                  .Include(p => p.IdPonudaPrijevozaNavigation)
                                                  .Include(p => p.IdPonudaPrijevozaNavigation.IdZahtjevNavigation)
                                                  .Include(p => p.IdPonudaPrijevozaNavigation.IdZahtjevNavigation.IdStatusZahtjevaNavigation)
                                                  .FirstOrDefault();

            var zahtjev = _context.Zahtjev.Where(m => m.Id == prijevoz.IdPonudaPrijevozaNavigation.IdZahtjev).FirstOrDefault();

            StatusZahtjeva noviStatus = _context.StatusZahtjeva.Where(s => s.Status == "PristiglePonude").FirstOrDefault();

            zahtjev.IdStatusZahtjeva = noviStatus.Id;
            _context.Zahtjev.Update(zahtjev);


            var ponudaPrijevoza = _context.PonudaPrijevoza.Where(m => m.Id == prijevoz.IdPonudaPrijevoza).FirstOrDefault();

            StatusPonude statusGotovo = _context.StatusPonude.Where(s => s.Status == "Otvoreno").FirstOrDefault();
            ponudaPrijevoza.IdStatusPonude = statusGotovo.Id;
            _context.PonudaPrijevoza.Update(ponudaPrijevoza);

            _context.Prijevoz.Remove(prijevoz);

            string urlString = "Details/" + zahtjev.Id.ToString();

            await _context.SaveChangesAsync();
            return RedirectToAction(urlString, "ProsireniZahtjevi");
        }
    }
}
