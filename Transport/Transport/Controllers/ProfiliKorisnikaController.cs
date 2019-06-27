using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Transport.Models;
using Transport.ViewModels;
using Transport.Extensions;
using System.Security.Claims;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using Microsoft.IdentityModel.Protocols;

namespace Transport.Controllers
{
    public class ProfiliKorisnikaController : Controller
    {
        private readonly transportContext _context;

        public ProfiliKorisnikaController(transportContext context)
        {
            _context = context;
        }

        // GET: ProfiliKorisnika
        public IActionResult Index()
        {
            KorisniciVozilaViewModel korisniciVozilaViewModel = new KorisniciVozilaViewModel();
            korisniciVozilaViewModel.Korisnici = _context.Korisnik.ToList();
            korisniciVozilaViewModel.Vozila = _context.Vozilo.Include(z => z.IdPrijevoznikNavigation).ToList();
            korisniciVozilaViewModel.Narucitelji = _context.Narucitelj.ToList();
            korisniciVozilaViewModel.Prijevoznici = _context.Prijevoznik.ToList();

            return View(korisniciVozilaViewModel);
        }

        // GET: ProfiliKorisnika/Details/5
        public async Task<IActionResult> Details(int id)
        {
            ProfilVozilaViewModel profilVozila = VratiProfil(id);
            return View(profilVozila);
        }

        public IActionResult MojProfil()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ApplicationUser user = _context.Users.Where(u => u.Id == userId).FirstOrDefault();

            ProfilVozilaViewModel profilVozila = VratiProfil(user.IdKorisnik);

            return View(profilVozila);
        }

        // GET: ProfiliKorisnika/DodavanjeVozila
        public IActionResult DodavanjeVozila()
        {
            return View();
        }

        public ProfilVozilaViewModel VratiProfil(int id)
        {
            var korisnik =  _context.Korisnik.SingleOrDefault(m => m.Id == id);

            KorisnikovProfilViewModel korisnikovProfil = new KorisnikovProfilViewModel();

            korisnikovProfil.Id = korisnik.Id;
            korisnikovProfil.Oib = korisnik.Oib;
            korisnikovProfil.Ime = korisnik.Ime;
            korisnikovProfil.Prezime = korisnik.Prezime;
            korisnikovProfil.Lozinka = korisnik.Lozinka;
            korisnikovProfil.TelBroj = korisnik.TelBroj;
            korisnikovProfil.Email = korisnik.Email;
            korisnikovProfil.PrimaEmail = korisnik.PrimaEmail;


            if (_context.Narucitelj.Any(m => m.IdKorisnik == id))
            {
                var narucitelj = _context.Narucitelj.SingleOrDefault(m => m.IdKorisnik == id);
                korisnikovProfil.ZahtijevaEko = narucitelj.ZahtijevaEko;

                var prijevoziNarucitelj = _context.Prijevoz.Where(p => p.IdPonudaPrijevozaNavigation.IdZahtjevNavigation.IdNarucitelj == korisnikovProfil.Id).ToList();

                if (prijevoziNarucitelj.Count() > 0)
                {
                    int ocjene = 0;
                    int brojOcjena = 0;

                    foreach (var prijevoz in prijevoziNarucitelj)
                    {
                        if (prijevoz.OcjenaNarucitelja != 0 && prijevoz.OcjenaNarucitelja != null)
                        {
                            ocjene += (int)prijevoz.OcjenaNarucitelja;
                            brojOcjena++;
                        }

                    }

                    if (brojOcjena > 0)
                    {
                        korisnikovProfil.OcjenaNarucitelj = (float)(ocjene / brojOcjena);
                    }
                }
            }


            List<Vozilo> vozila = null;

            if (_context.Prijevoznik.Any(m => m.IdKorisnik == id))
            {
                var prijevoznik = _context.Prijevoznik.SingleOrDefault(m => m.IdKorisnik == id);
                korisnikovProfil.NazivTvrtke = prijevoznik.NazivTvrtke;

                var prijevoziPrijevoznik = _context.Prijevoz.Where(p => p.IdPonudaPrijevozaNavigation.IdPrijevoznik == korisnikovProfil.Id).ToList();

                if (prijevoziPrijevoznik.Count() > 0)
                {
                    int ocjene = 0;
                    int brojOcjena = 0;

                    foreach (var prijevoz in prijevoziPrijevoznik)
                    {
                        if (prijevoz.OcjenaPrijevoznika != 0 && prijevoz.OcjenaPrijevoznika != null)
                        {
                            ocjene += (int)prijevoz.OcjenaPrijevoznika;
                            brojOcjena++;
                        }
                    }
                    if (brojOcjena > 0)
                    {
                        korisnikovProfil.OcjenaPrijevoznik = (float)(ocjene / brojOcjena);
                    }

                }

                vozila = _context.Vozilo.Where(v => v.IdPrijevoznik == id).ToList();
            }
          

            ProfilVozilaViewModel profilVozila = new ProfilVozilaViewModel();
            profilVozila.KorisnikovProfil = korisnikovProfil;
            profilVozila.Vozila = vozila;

            return profilVozila;
        }

        // GET: Vozila/Edit/5
        public async Task<IActionResult> EditiranjeVozila(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vozilo = await _context.Vozilo.SingleOrDefaultAsync(m => m.Id == id);
            if (vozilo == null)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(MojProfil));
        }



        // POST: Vozila/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditiranjeVozila(int id, [Bind("Id,Tip,Marka,Boja,RegistarskaOznaka,Ekolosko")] Vozilo vozilo)
        {
            if (id != vozilo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //PRIJAVLJENI KORISNIK!!
                    // trenutno: Ivana, Id = 1
                    int prijavljeniKorisnikId = 2;
                    var prijevoznik = _context.Prijevoznik.Where(p => p.IdKorisnik == prijavljeniKorisnikId).FirstOrDefault();

                    var voz = _context.Vozilo.Where(v => v.Id == vozilo.Id).FirstOrDefault();

                    voz.Tip = vozilo.Tip;
                    voz.Marka = vozilo.Marka;
                    voz.Boja = vozilo.Boja;
                    voz.RegistarskaOznaka = vozilo.RegistarskaOznaka;
                    voz.Ekolosko = vozilo.Ekolosko;
                    voz.IdPrijevoznik = prijevoznik.IdKorisnik;

                    _context.Update(voz);
                    await _context.SaveChangesAsync();
                    TempData[Constants.Message] = "Ažuriranje uspješno obavljeno";
                    TempData[Constants.ErrorOccurred] = false;

                    string urlString = "Details/" + prijevoznik.IdKorisnik.ToString();
                    return RedirectToAction(urlString);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VoziloExists(vozilo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(vozilo);
        }


        // GET: ProfiliKorisnika/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ProfiliKorisnika/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Oib,Ime,Prezime,Lozinka,TelBroj,Email")] Korisnik korisnik)
        {
            if (ModelState.IsValid)
            {
                _context.Korisnik.Add(korisnik);

                Narucitelj narucitelj = new Narucitelj
                {
                    IdKorisnik = korisnik.Id,
                    IdKorisnikNavigation = korisnik
                };
                _context.Narucitelj.Add(narucitelj);

                Prijevoznik prijevoznik = new Prijevoznik
                {
                    IdKorisnik = korisnik.Id,
                    IdKorisnikNavigation = korisnik
                };
                _context.Prijevoznik.Add(prijevoznik);

                await _context.SaveChangesAsync();
                TempData[Constants.Message] = $"Korisnik dodan.";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Index));
            }
            return View(korisnik);
        }

        // GET: ProfiliKorisnika/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var korisnik = await _context.Korisnik.SingleOrDefaultAsync(m => m.Id == id);
            if (korisnik == null)
            {
                return NotFound();
            }

            KorisnikovProfilViewModel korisnikovProfil = new KorisnikovProfilViewModel();

            korisnikovProfil.Id = korisnik.Id;
            korisnikovProfil.Oib = korisnik.Oib;
            korisnikovProfil.Ime = korisnik.Ime;
            korisnikovProfil.Prezime = korisnik.Prezime;
            korisnikovProfil.Lozinka = korisnik.Lozinka;
            korisnikovProfil.TelBroj = korisnik.TelBroj;
            korisnikovProfil.PrimaEmail = korisnik.PrimaEmail;
            korisnikovProfil.Email = korisnik.Email;

            if ( _context.Narucitelj.Any(m => m.IdKorisnik == id))
            {
                var narucitelj = await _context.Narucitelj.SingleOrDefaultAsync(m => m.IdKorisnik == id);
                korisnikovProfil.ZahtijevaEko = narucitelj.ZahtijevaEko;
            }

            if (_context.Prijevoznik.Any(m => m.IdKorisnik == id))
            {
                var prijevoznik = await _context.Prijevoznik.SingleOrDefaultAsync(m => m.IdKorisnik == id);
                korisnikovProfil.NazivTvrtke = prijevoznik.NazivTvrtke;
            }
              
            return View(korisnikovProfil);
        }

        // POST: ProfiliKorisnika/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string PrimaEmail, [Bind("Id,Oib,Ime,Prezime,Lozinka,TelBroj,Email,ZahtijevaEko,NazivTvrtke")] KorisnikovProfilViewModel korisnikovProfil)
        {
            var korisnik = await _context.Korisnik.SingleOrDefaultAsync(m => m.Id == id);
            if (korisnik == null)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                try
                {

                    korisnik.Oib = korisnikovProfil.Oib;
                    korisnik.Ime = korisnikovProfil.Ime;
                    korisnik.Prezime = korisnikovProfil.Prezime;
                    korisnik.Lozinka = korisnikovProfil.Lozinka;
                    korisnik.TelBroj = korisnikovProfil.TelBroj;
                    korisnik.Email = korisnikovProfil.Email;
                    
                    if (PrimaEmail == "on")
                    {
                        korisnik.PrimaEmail = true;
                    }
                    else
                    {
                        korisnik.PrimaEmail = false;
                    }
                    _context.Korisnik.Update(korisnik);

                    if (_context.Narucitelj.Any(m => m.IdKorisnik == id))
                    {
                        var narucitelj = await _context.Narucitelj.SingleOrDefaultAsync(m => m.IdKorisnik == id);
                        narucitelj.ZahtijevaEko = korisnikovProfil.ZahtijevaEko;
                        _context.Narucitelj.Update(narucitelj);
                    }

                    if (_context.Prijevoznik.Any(m => m.IdKorisnik == id))
                    {
                        var prijevoznik = await _context.Prijevoznik.SingleOrDefaultAsync(m => m.IdKorisnik == id);
                        prijevoznik.NazivTvrtke = korisnikovProfil.NazivTvrtke;
                        _context.Prijevoznik.Update(prijevoznik);
                    }

        
                    await _context.SaveChangesAsync();
                    TempData[Constants.Message] = "Ažuriranje uspješno obavljeno";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KorisnikExists(korisnik.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                
            }

            return RedirectToAction(nameof(MojProfil));

        }

            // GET: ProfiliKorisnika/Delete/5
            public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var korisnik = await _context.Korisnik
                .SingleOrDefaultAsync(m => m.Id == id);
            if (korisnik == null)
            {
                return NotFound();
            }

            return View(korisnik);
        }

        // POST: ProfiliKorisnika/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var korisnik = await _context.Korisnik.SingleOrDefaultAsync(m => m.Id == id);
                _context.Korisnik.Remove(korisnik);
                await _context.SaveChangesAsync();
                TempData[Constants.Message] = $"Korisnik uspješno obrisan";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception exc)
            {
                TempData[Constants.Message] = "Pogreška prilikom brisanja korisnika: " + exc.CompleteExceptionMessage();
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index));
        }

        private bool KorisnikExists(int id)
        {
            return _context.Korisnik.Any(e => e.Id == id);
        }

        private bool VoziloExists(int id)
        {
            return _context.Vozilo.Any(e => e.Id == id);
        }
    }
}
