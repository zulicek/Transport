using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Transport.Extensions;
using Transport.Models;

namespace Transport.Controllers
{
    public class VozilaController : Controller
    {
        private readonly transportContext _context;

        public VozilaController(transportContext context)
        {
            _context = context;
        }
        
        // GET: Vozila
        public async Task<IActionResult> Index()
        {
            var transportContext = _context.Vozilo.Include(v => v.IdPrijevoznikNavigation);
            return View(await transportContext.ToListAsync());
        }

        // GET: Vozila/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vozilo = await _context.Vozilo
                .Include(v => v.IdPrijevoznikNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (vozilo == null)
            {
                return NotFound();
            }

            return View(vozilo);
        }

        // GET: Vozila/Create
        public IActionResult Create()
        {
            ViewData["IdPrijevoznik"] = new SelectList(_context.Prijevoznik, "IdKorisnik", "IdKorisnik");
            return View();
        }

        // POST: Vozila/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int IdPrijevoznik, [Bind("Id,Tip,Marka,Boja,RegistarskaOznaka,Ekolosko")] Vozilo vozilo)
        {
            if (ModelState.IsValid)
            {

                Vozilo novoVozilo = new Vozilo
                {
                    Tip = vozilo.Tip,
                    Marka = vozilo.Marka,
                    Boja = vozilo.Boja,
                    RegistarskaOznaka = vozilo.RegistarskaOznaka,
                    Ekolosko = vozilo.Ekolosko,
                    IdPrijevoznik = IdPrijevoznik
                };
                _context.Add(novoVozilo);
                await _context.SaveChangesAsync();
                TempData[Constants.Message] = $"Vozilo  {vozilo.RegistarskaOznaka} uspješno dodano";
                TempData[Constants.ErrorOccurred] = false;

                string urlString = "Details/" + IdPrijevoznik.ToString();
                return RedirectToAction(urlString, "ProfiliKorisnika");
            }
            ViewData["IdPrijevoznik"] = new SelectList(_context.Prijevoznik, "IdKorisnik", "IdKorisnik", vozilo.IdPrijevoznik);
            return View(vozilo);
        }

        // GET: Vozila/Edit/5
        public async Task<IActionResult> Edit(int? id)
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
            ViewData["IdPrijevoznik"] = new SelectList(_context.Prijevoznik, "IdKorisnik", "IdKorisnik", vozilo.IdPrijevoznik);
            return View(vozilo);
        }

        // POST: Vozila/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Tip,Marka,Boja,RegistarskaOznaka,Ekolosko,IdPrijevoznik")] Vozilo vozilo)
        {
            if (id != vozilo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vozilo);
                    await _context.SaveChangesAsync();
                    TempData[Constants.Message] = "Ažuriranje uspješno obavljeno";
                    TempData[Constants.ErrorOccurred] = false;
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdPrijevoznik"] = new SelectList(_context.Prijevoznik, "IdKorisnik", "IdKorisnik", vozilo.IdPrijevoznik);
            return View(vozilo);
        }

        // GET: Vozila/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vozilo = await _context.Vozilo
                .Include(v => v.IdPrijevoznikNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (vozilo == null)
            {
                return NotFound();
            }

            return View(vozilo);
        }

        // POST: Vozila/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var vozilo = await _context.Vozilo.SingleOrDefaultAsync(m => m.Id == id);
                _context.Vozilo.Remove(vozilo);
                await _context.SaveChangesAsync();
                TempData[Constants.Message] = $"Vozilo  {vozilo.RegistarskaOznaka} uspješno obrisano";
                TempData[Constants.ErrorOccurred] = false;

                string urlString = "Details/" + vozilo.IdPrijevoznik.ToString();
                return RedirectToAction(urlString, "ProfiliKorisnika");
            }
            catch (Exception exc)
            {
                TempData[Constants.Message] = "Pogreška prilikom brisanja vozila: " + exc.CompleteExceptionMessage();
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index));
        }

        private bool VoziloExists(int id)
        {
            return _context.Vozilo.Any(e => e.Id == id);
        }
    }
}
