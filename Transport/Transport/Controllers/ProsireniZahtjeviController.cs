using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Transport.Models;
using Transport.ViewModels;
using Transport.Extensions;
using System.Security.Claims;

namespace Transport.Controllers
{
    public class ProsireniZahtjeviController : Controller
    {
        private readonly transportContext _context;

        public ProsireniZahtjeviController(transportContext context)
        {
            _context = context;
        }

        // GET: ProsireniZahtjevi
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ApplicationUser user = _context.Users.Where(u => u.Id == userId).FirstOrDefault();

            ZahtjeviLokacijeViewModel zahtjevLokacijaViewModel = new ZahtjeviLokacijeViewModel();
            zahtjevLokacijaViewModel.Zahtjevi = _context.Zahtjev.Include(z => z.IdNaruciteljNavigation)
                                                                 .Where( z => z.IdNarucitelj != user.IdKorisnik)
                                                                .Include(z => z.IdNaruciteljNavigation.IdKorisnikNavigation)
                                                                .Include(z => z.IdStatusZahtjevaNavigation)
                                                                .ToList();
            zahtjevLokacijaViewModel.ZahtjeviLokacije = _context.ZahtjevLokacija.Include(z => z.IdLokacijaNavigation)
                                                                .Include(z => z.IdLokacijaNavigation.IdVrstaLokacijeNavigation)
                                                                .Include(z => z.IdLokacijaNavigation.IdMjestoNavigation)
                                                                .ToList();

            return View(zahtjevLokacijaViewModel);
        }

        public IActionResult MojiZahtjevi()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ApplicationUser user = _context.Users.Where(u => u.Id == userId).FirstOrDefault();

            ZahtjeviLokacijeViewModel zahtjevLokacijaViewModel = new ZahtjeviLokacijeViewModel();
            zahtjevLokacijaViewModel.Zahtjevi = _context.Zahtjev.Where(z => z.IdNarucitelj == user.IdKorisnik)
                                                                .Include(z => z.IdNaruciteljNavigation)
                                                                .Include(z => z.IdNaruciteljNavigation.IdKorisnikNavigation)
                                                                .Include(z => z.IdStatusZahtjevaNavigation)
                                                                .ToList();
            zahtjevLokacijaViewModel.ZahtjeviLokacije = _context.ZahtjevLokacija.Include(z => z.IdLokacijaNavigation)
                                                                .Include(z => z.IdLokacijaNavigation.IdVrstaLokacijeNavigation)
                                                                .Include(z => z.IdLokacijaNavigation.IdMjestoNavigation)
                                                                .ToList();

            return View(zahtjevLokacijaViewModel);
        }

        // GET: Zahtjevi/Create
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Zahtjevi/Create
        public async Task<IActionResult> Create([Bind("Id,VrijemePocetka,VrijemeZavrsetka,CijenaNeizvrsenja,Sirina,Visina,Duiljina," +
            "Masa,Opis, PocetnaLokacijaUlica, PocetnaLokacijaKucniBroj, PocetnoMjesto, PocetnoMjestoPbr," +
            "OdredisnaLokacijaUlica, OdredisnaLokacijaKucniBroj, OdredisnoMjesto, OdredisnoMjestoPbr")] ProsireniZahtjevViewModel prosireniZahtjev)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                ApplicationUser user = _context.Users.Where(u => u.Id == userId).FirstOrDefault();


                StatusZahtjeva status = _context.StatusZahtjeva.Where(s => s.Status == "Otvoreno").FirstOrDefault();
                var zahtjev = new Zahtjev
                {
                    Id = prosireniZahtjev.Id,
                    VrijemePocetka = prosireniZahtjev.VrijemePocetka,
                    VrijemeZavrsetka = prosireniZahtjev.VrijemeZavrsetka,
                    CijenaNeizvrsenja = prosireniZahtjev.CijenaNeizvrsenja,
                    Sirina = prosireniZahtjev.Sirina,
                    Visina = prosireniZahtjev.Visina,
                    Duiljina = prosireniZahtjev.Duiljina,
                    Masa = prosireniZahtjev.Masa,
                    Opis = prosireniZahtjev.Opis,
                    IdStatusZahtjeva = status.Id,
                    IdNarucitelj = user.IdKorisnik,
                };
                _context.Zahtjev.Add(zahtjev);


                //po?etno mjesto
                Mjesto pocetnoMjesto = null;
                if (_context.Mjesto.Any(m => m.PostanskiBroj == prosireniZahtjev.PocetnoMjestoPbr))
                {
                    pocetnoMjesto = _context.Mjesto.Where(m => m.PostanskiBroj == prosireniZahtjev.PocetnoMjestoPbr).FirstOrDefault();
                    TempData[Constants.Message] = "Po?etno mjesto ve? postoji  u bazi ";
                    TempData[Constants.ErrorOccurred] = true;
                }
                else
                {
                    pocetnoMjesto = new Mjesto
                    {
                        Naziv = prosireniZahtjev.PocetnoMjesto,
                        PostanskiBroj = prosireniZahtjev.PocetnoMjestoPbr
                    };
                    _context.Mjesto.Add(pocetnoMjesto);
                }


                //odredišno mjesto
                Mjesto odredisnoMjesto = null;
                if (_context.Mjesto.Any(m => m.PostanskiBroj == prosireniZahtjev.OdredisnoMjestoPbr))
                {
                    odredisnoMjesto = _context.Mjesto.Where(m => m.PostanskiBroj == prosireniZahtjev.OdredisnoMjestoPbr).FirstOrDefault();
                    TempData[Constants.Message] = "Odredišno mjesto ve? postoji  u bazi ";
                    TempData[Constants.ErrorOccurred] = true;
                }
                else
                {
                    odredisnoMjesto = new Mjesto
                    {
                        Naziv = prosireniZahtjev.OdredisnoMjesto,
                        PostanskiBroj = prosireniZahtjev.OdredisnoMjestoPbr
                    };
                    _context.Mjesto.Add(odredisnoMjesto);
                }


                //vrste lokacije
                var vrstaPocetna = await _context.VrstaLokacije.SingleOrDefaultAsync(m => m.Vrsta == "po?etna");
                var vrstaOdredišna = await _context.VrstaLokacije.SingleOrDefaultAsync(m => m.Vrsta == "odredišna");


                //po?etna lokacija
                Lokacija pocetnaLokacija = null;
                if (_context.Lokacija.Any(m => m.IdMjesto == pocetnoMjesto.Id &&
                     m.Ulica == prosireniZahtjev.PocetnaLokacijaUlica &&
                     m.KucniBroj == prosireniZahtjev.PocetnaLokacijaKucniBroj &&
                     m.IdVrstaLokacije == 1))
                {
                    pocetnaLokacija = _context.Lokacija.Where(m =>
                        m.Ulica == prosireniZahtjev.PocetnaLokacijaUlica &&
                        m.KucniBroj == prosireniZahtjev.PocetnaLokacijaKucniBroj &&
                        m.IdMjesto == pocetnoMjesto.Id &&
                        m.IdVrstaLokacije == vrstaPocetna.Id).FirstOrDefault();
                    TempData[Constants.Message] = "Po?etna lokacija ve? postoji  u bazi ";
                    TempData[Constants.ErrorOccurred] = true;
                }
                else
                {
                    pocetnaLokacija = new Lokacija
                    {
                        Ulica = prosireniZahtjev.PocetnaLokacijaUlica,
                        KucniBroj = prosireniZahtjev.PocetnaLokacijaKucniBroj,
                        IdMjestoNavigation = pocetnoMjesto,
                        IdMjesto = pocetnoMjesto.Id,
                        IdVrstaLokacijeNavigation = vrstaPocetna,
                        IdVrstaLokacije = 1
                    };
                    _context.Lokacija.Add(pocetnaLokacija);
                }


                //odredišna lokacija
                Lokacija odredisnaLokacija = null;
                if (_context.Mjesto.Any(m => m.PostanskiBroj == prosireniZahtjev.OdredisnoMjestoPbr) &&
                    _context.Lokacija.Any(m => m.Ulica == prosireniZahtjev.OdredisnaLokacijaUlica) &&
                    _context.Lokacija.Any(m => m.KucniBroj == prosireniZahtjev.OdredisnaLokacijaKucniBroj) &&
                     _context.Lokacija.Any(m => m.IdVrstaLokacije == 2))
                {
                    odredisnaLokacija = _context.Lokacija.Where(m =>
                        m.Ulica == prosireniZahtjev.OdredisnaLokacijaUlica &&
                        m.KucniBroj == prosireniZahtjev.OdredisnaLokacijaKucniBroj &&
                        m.IdMjesto == odredisnoMjesto.Id &&
                        m.IdVrstaLokacije == vrstaOdredišna.Id).FirstOrDefault();
                    TempData[Constants.Message] = "Odredišna lokacija ve? postoji  u bazi ";
                    TempData[Constants.ErrorOccurred] = true;
                }
                else
                {
                    odredisnaLokacija = new Lokacija
                    {
                        Ulica = prosireniZahtjev.OdredisnaLokacijaUlica,
                        KucniBroj = prosireniZahtjev.OdredisnaLokacijaKucniBroj,
                        IdMjesto = odredisnoMjesto.Id,
                        IdMjestoNavigation = odredisnoMjesto,
                        IdVrstaLokacijeNavigation = vrstaOdredišna,
                        IdVrstaLokacije = 2,
                        
                    };
                    _context.Lokacija.Add(odredisnaLokacija);
                }


                // zahtjev - po?etna lokacija            
                var zahtjevPocetnaLokacija = new ZahtjevLokacija
                {
                    IdZahtjev = zahtjev.Id,
                    IdLokacija = pocetnaLokacija.Id,
                    IdZahtjevNavigation = zahtjev,

                };
                _context.ZahtjevLokacija.Add(zahtjevPocetnaLokacija);
                


                // zahtjev - odredišna lokacija
                var zahtjevOdredisnaLokacija = new ZahtjevLokacija
                {
                    IdZahtjev = zahtjev.Id,
                    IdLokacija = odredisnaLokacija.Id,
                    IdZahtjevNavigation = zahtjev,
                };
                _context.ZahtjevLokacija.Add(zahtjevOdredisnaLokacija);
                

                await _context.SaveChangesAsync();
                TempData[Constants.Message] = $"Prosireni zahtjev uspješno dodan";
                TempData[Constants.ErrorOccurred] = false;

            }

            return RedirectToAction(nameof(MojiZahtjevi));

        }


        //GET: ProsireniZahtjev/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zahtjev = _context.Zahtjev.Where(m => m.Id == id).Include(m => m.IdStatusZahtjevaNavigation)
                                                        .Include(m => m.IdNaruciteljNavigation)
                                                        .Include(m => m.IdNaruciteljNavigation.IdKorisnikNavigation).FirstOrDefault();
            if (id != zahtjev.Id)
            {
                return NotFound();
            }

            var lokacijeZahtjeva = _context.ZahtjevLokacija.Where(m => m.IdZahtjev == id)
                                        .Include(z => z.IdLokacijaNavigation)
                                        .Include(z => z.IdLokacijaNavigation.IdVrstaLokacijeNavigation)
                                        .Include(z => z.IdLokacijaNavigation.IdMjestoNavigation);

            Lokacija pocetnaLokacija = null;
            Lokacija odredisnaLokacija = null;

            foreach (var lokacija in lokacijeZahtjeva)
            {

                if (lokacija.IdLokacijaNavigation.IdVrstaLokacijeNavigation.Vrsta == "po?etna")
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

            var ponudePrijevoza = _context.PonudaPrijevoza.Where(p => p.IdZahtjev == zahtjev.Id)
                                                    .Include(p => p.IdPrijevoznikNavigation)
                                                    .Include(p => p.IdPrijevoznikNavigation.IdKorisnikNavigation)
                                                    .Include(p => p.IdStatusPonudeNavigation).ToList();

            var vozila = _context.Vozilo.Include(z => z.IdPrijevoznikNavigation).ToList();

            ZahtjevPonudeViewModel zahtjevPonude = new ZahtjevPonudeViewModel();
            zahtjevPonude.ProsireniZahtjev = prosireniZahtjev;
            zahtjevPonude.PonudePrijevoza = ponudePrijevoza;
            zahtjevPonude.Vozila = vozila;

            //vrati ID prijevoza ako postoji
            if (_context.Prijevoz.Any(p => p.IdPonudaPrijevozaNavigation.IdZahtjev == zahtjev.Id))
            {
                Prijevoz prijevoz = _context.Prijevoz.Where(p => p.IdPonudaPrijevozaNavigation.IdZahtjev == zahtjev.Id).FirstOrDefault();
                ViewData["IdPrijevoz"] = prijevoz.Id;
            }
            else
            {
                ViewData["IdPrijevoz"] = null;
            }
            
            return View(zahtjevPonude);
        }


        public async Task<IActionResult> MojZahtjev(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zahtjev = _context.Zahtjev.Where(m => m.Id == id).Include(m => m.IdStatusZahtjevaNavigation)
                                                        .Include(m => m.IdNaruciteljNavigation)
                                                        .Include(m => m.IdNaruciteljNavigation.IdKorisnikNavigation).FirstOrDefault();
            if (id != zahtjev.Id)
            {
                return NotFound();
            }

            var lokacijeZahtjeva = _context.ZahtjevLokacija.Where(m => m.IdZahtjev == id)
                                        .Include(z => z.IdLokacijaNavigation)
                                        .Include(z => z.IdLokacijaNavigation.IdVrstaLokacijeNavigation)
                                        .Include(z => z.IdLokacijaNavigation.IdMjestoNavigation);

            Lokacija pocetnaLokacija = null;
            Lokacija odredisnaLokacija = null;

            foreach (var lokacija in lokacijeZahtjeva)
            {

                if (lokacija.IdLokacijaNavigation.IdVrstaLokacijeNavigation.Vrsta == "po?etna")
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

            var ponudePrijevoza = _context.PonudaPrijevoza.Where(p => p.IdZahtjev == zahtjev.Id)
                                                    .Include(p => p.IdPrijevoznikNavigation)
                                                    .Include(p => p.IdPrijevoznikNavigation.IdKorisnikNavigation)
                                                    .Include(p => p.IdStatusPonudeNavigation).ToList();

            var vozila = _context.Vozilo.Include(z => z.IdPrijevoznikNavigation).ToList();

            ZahtjevPonudeViewModel zahtjevPonude = new ZahtjevPonudeViewModel();
            zahtjevPonude.ProsireniZahtjev = prosireniZahtjev;
            zahtjevPonude.PonudePrijevoza = ponudePrijevoza;
            zahtjevPonude.Vozila = vozila;

            //vrati ID prijevoza ako postoji
            if (_context.Prijevoz.Any(p => p.IdPonudaPrijevozaNavigation.IdZahtjev == zahtjev.Id))
            {
                Prijevoz prijevoz = _context.Prijevoz.Where(p => p.IdPonudaPrijevozaNavigation.IdZahtjev == zahtjev.Id).FirstOrDefault();
                ViewData["IdPrijevoz"] = prijevoz.Id;
            }
            else
            {
                ViewData["IdPrijevoz"] = null;
            }

            return View(zahtjevPonude);
        }


        // GET: ProsireniZahtjevs/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zahtjev = await _context.Zahtjev.SingleOrDefaultAsync(m => m.Id == id);
            if (id != zahtjev.Id)
            {
                return NotFound();
            }

            var lokacijeZahtjeva = _context.ZahtjevLokacija.Where(m => m.IdZahtjev == id)
                                        .Include(z => z.IdLokacijaNavigation)
                                        .Include(z => z.IdLokacijaNavigation.IdVrstaLokacijeNavigation)
                                        .Include (z => z.IdLokacijaNavigation.IdMjestoNavigation);

            Lokacija pocetnaLokacija = null;
            Lokacija odredisnaLokacija = null;
            
            foreach (var lokacija in lokacijeZahtjeva)
            {

                if (lokacija.IdLokacijaNavigation.IdVrstaLokacijeNavigation.Vrsta == "po?etna")
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
            

            ViewData["IdNarucitelj"] = new SelectList(_context.Narucitelj, "IdKorisnik", "IdKorisnik", zahtjev.IdNarucitelj);
            ViewData["IdStatusZahtjeva"] = new SelectList(_context.StatusZahtjeva, "Id", "Status", zahtjev.IdStatusZahtjeva);
            return View(prosireniZahtjev);
        }

        // POST: ProsireniZahtjevs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,VrijemePocetka,VrijemeZavrsetka,CijenaNeizvrsenja,Sirina,Visina,Duiljina,Masa,Opis,IdStatusZahtjeva,IdNarucitelj,PocetnaLokacijaUlica,PocetnaLokacijaKucniBroj,PocetnoMjesto,PocetnoMjestoPbr,OdredisnaLokacijaUlica,OdredisnaLokacijaKucniBroj,OdredisnoMjesto,OdredisnoMjestoPbr")] ProsireniZahtjevViewModel prosireniZahtjev)
        {
            var zahtjev = await _context.Zahtjev.SingleOrDefaultAsync(m => m.Id == id);
            if (id != zahtjev.Id)
            {
                return NotFound();
            }

            var lokacijeZahtjeva = _context.ZahtjevLokacija.Where(m => m.IdZahtjev == id)
                                        .Include(z => z.IdLokacijaNavigation)
                                        .Include(z => z.IdLokacijaNavigation.IdVrstaLokacijeNavigation)
                                        .Include(z => z.IdLokacijaNavigation.IdMjestoNavigation);

            Lokacija pocetnaLokacija = null;
            Lokacija odredisnaLokacija = null;


            foreach (var lokacija in lokacijeZahtjeva)
            {
                if (lokacija.IdLokacijaNavigation.IdVrstaLokacijeNavigation.Vrsta == "po?etna")
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


            if (ModelState.IsValid)
            {
                try
                {
                    zahtjev.VrijemePocetka = prosireniZahtjev.VrijemePocetka;
                    zahtjev.VrijemeZavrsetka = prosireniZahtjev.VrijemeZavrsetka;
                    zahtjev.CijenaNeizvrsenja = prosireniZahtjev.CijenaNeizvrsenja;
                    zahtjev.Sirina = prosireniZahtjev.Sirina;
                    zahtjev.Visina = prosireniZahtjev.Visina;
                    zahtjev.Duiljina = prosireniZahtjev.Duiljina;
                    zahtjev.Opis = prosireniZahtjev.Opis;
                    zahtjev.IdNarucitelj = prosireniZahtjev.IdNarucitelj;
                    zahtjev.IdStatusZahtjeva = prosireniZahtjev.IdStatusZahtjeva;
                    _context.Zahtjev.Update(zahtjev);


                    //ako po?etno mjesto postoji u bazi, u lokaciju stavi njegov id, ako ne, dodaj ga u bazu
                    if (_context.Mjesto.Any(m => m.PostanskiBroj == prosireniZahtjev.PocetnoMjestoPbr
                                    && m.Naziv == prosireniZahtjev.PocetnoMjesto))
                    {
                        pocetnoMjesto = _context.Mjesto.Where(m => m.PostanskiBroj == prosireniZahtjev.PocetnoMjestoPbr
                                                 && m.Naziv == prosireniZahtjev.PocetnoMjesto).FirstOrDefault();

                        pocetnaLokacija.Id = pocetnoMjesto.Id;
                        pocetnaLokacija.IdMjestoNavigation = pocetnoMjesto;
                    }
                    else
                    {
                        Mjesto novoMjesto = new Mjesto
                        {
                            Naziv = prosireniZahtjev.PocetnoMjesto,
                            PostanskiBroj = prosireniZahtjev.PocetnoMjestoPbr
                        };
                        _context.Mjesto.Add(novoMjesto);

                        pocetnaLokacija.IdMjesto = novoMjesto.Id;
                        pocetnaLokacija.IdMjestoNavigation = novoMjesto;
                    }

                    //ako odredišno mjesto postoji u bazi, u lokaciju stavi njegov id, ako ne, dodaj ga u bazu
                    if (_context.Mjesto.Any(m => m.PostanskiBroj == prosireniZahtjev.OdredisnoMjestoPbr
                                   && m.Naziv == prosireniZahtjev.OdredisnoMjesto))
                    {
                        odredisnoMjesto = _context.Mjesto.Where(m => m.PostanskiBroj == prosireniZahtjev.OdredisnoMjestoPbr
                        && m.Naziv == prosireniZahtjev.OdredisnoMjesto).FirstOrDefault();

                        odredisnaLokacija.IdMjesto = odredisnoMjesto.Id;
                        odredisnaLokacija.IdMjestoNavigation = odredisnoMjesto;
                    }
                    else
                    {
                        Mjesto novoMjesto = new Mjesto
                        {
                            Naziv = prosireniZahtjev.OdredisnoMjesto,
                            PostanskiBroj = prosireniZahtjev.OdredisnoMjestoPbr
                        };
                        _context.Mjesto.Add(novoMjesto);

                        odredisnaLokacija.IdMjesto = novoMjesto.Id;
                        odredisnaLokacija.IdMjestoNavigation = novoMjesto;
                    }

                    pocetnaLokacija.Ulica = prosireniZahtjev.PocetnaLokacijaUlica;
                    pocetnaLokacija.KucniBroj = prosireniZahtjev.PocetnaLokacijaKucniBroj;


                    //ako po?etna lokacija postoji u bazi, u zahtjev-lokacije stavi njezin id, ako ne, ažuriraj ovu u bazi
                    if (_context.Lokacija.Any(l => l.KucniBroj == pocetnaLokacija.KucniBroj && l.Ulica == pocetnaLokacija.Ulica
                                    && l.IdMjesto == pocetnaLokacija.IdMjesto && l.IdVrstaLokacijeNavigation.Vrsta == "po?etna"))
                    {
                        ZahtjevLokacija pocetnaLokacijaZahtjev = _context.ZahtjevLokacija.Where(m => m.IdZahtjev == zahtjev.Id 
                                        && m.IdLokacija == pocetnaLokacija.Id).FirstOrDefault();

                        pocetnaLokacija = _context.Lokacija.Where(l => l.KucniBroj == pocetnaLokacija.KucniBroj && l.Ulica == pocetnaLokacija.Ulica
                                    && l.IdMjesto == pocetnaLokacija.IdMjesto && l.IdVrstaLokacijeNavigation.Vrsta == "po?etna").FirstOrDefault();

                        pocetnaLokacijaZahtjev.Id = pocetnaLokacija.Id;
                        pocetnaLokacijaZahtjev.IdLokacijaNavigation = pocetnaLokacija;

                        _context.ZahtjevLokacija.Update(pocetnaLokacijaZahtjev);
                    }
                    else
                    {
                        _context.Lokacija.Update(pocetnaLokacija);
                    }


                    odredisnaLokacija.Ulica = prosireniZahtjev.OdredisnaLokacijaUlica;
                    odredisnaLokacija.KucniBroj = prosireniZahtjev.OdredisnaLokacijaKucniBroj;

                    //ako odredišna lokacija postoji u bazi, u zahtjev-lokacije stavi njezin id, ako ne, ažuriraj ovu u bazi
                    if (_context.Lokacija.Any(l => l.KucniBroj == odredisnaLokacija.KucniBroj && l.Ulica == odredisnaLokacija.Ulica
                                    && l.IdMjesto == odredisnaLokacija.IdMjesto && l.IdVrstaLokacijeNavigation.Vrsta == "odredišna"))
                    {

                        ZahtjevLokacija odredisnaLokacijaZahtjev = _context.ZahtjevLokacija.Where(m => m.IdZahtjev == zahtjev.Id
                                       && m.IdLokacija == odredisnaLokacija.Id).FirstOrDefault();

                        odredisnaLokacija = _context.Lokacija.Where(l => l.KucniBroj == odredisnaLokacija.KucniBroj && l.Ulica == odredisnaLokacija.Ulica
                                    && l.IdMjesto == odredisnaLokacija.IdMjesto && l.IdVrstaLokacijeNavigation.Vrsta == "odredišna").FirstOrDefault();

                        odredisnaLokacijaZahtjev.Id = odredisnaLokacija.Id;
                        odredisnaLokacijaZahtjev.IdLokacijaNavigation = odredisnaLokacija;

                        _context.ZahtjevLokacija.Update(odredisnaLokacijaZahtjev);
                    }
                    else
                    {
                        _context.Lokacija.Update(odredisnaLokacija);
                    }


                    

                    await _context.SaveChangesAsync();
                    TempData[Constants.Message] = "Ažuriranje uspješno obavljeno";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ZahtjevExists(zahtjev.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

            }

            string urlString = "MojZahtjev/" + zahtjev.Id.ToString();
            ViewData["IdNarucitelj"] = new SelectList(_context.Narucitelj, "IdKorisnik", "IdKorisnik", zahtjev.IdNarucitelj);
            ViewData["IdStatusZahtjeva"] = new SelectList(_context.StatusZahtjeva, "Id", "Status", zahtjev.IdStatusZahtjeva);
            return RedirectToAction(urlString); ;
        }

        // GET: ProsireniZahtjevi/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zahtjev = await _context.Zahtjev
                .Include(z => z.IdNaruciteljNavigation)
                .Include(z => z.IdStatusZahtjevaNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (zahtjev == null)
            {
                return NotFound();
            }

            return View(zahtjev);
        }

        // POST: ProsireniZahtjevi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var zahtjev = await _context.Zahtjev.SingleOrDefaultAsync(m => m.Id == id);
                _context.Zahtjev.Remove(zahtjev);
                await _context.SaveChangesAsync();
                TempData[Constants.Message] = $"Zahtjev uspješno obrisan";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception exc)
            {
                TempData[Constants.Message] = "Pogreška prilikom brisanja zahtjeva: " + exc.CompleteExceptionMessage();
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(MojiZahtjevi));
        }

        private bool ZahtjevExists(int id)
        {
            return _context.Zahtjev.Any(e => e.Id == id);
        }
    }
}
