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
    public class PrijevozniciController : Controller
    {
        private readonly transportContext _context;

        public PrijevozniciController(transportContext context)
        {
            _context = context;
        }

        // GET: Prijevoznici
        public async Task<IActionResult> Index()
        {
            var transportContext = _context.Prijevoznik.Include(p => p.IdKorisnikNavigation);
            return View(await transportContext.ToListAsync());
        }

        // GET: Prijevoznici/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prijevoznik = await _context.Prijevoznik
                .Include(p => p.IdKorisnikNavigation)
                .SingleOrDefaultAsync(m => m.IdKorisnik == id);
            if (prijevoznik == null)
            {
                return NotFound();
            }

            return View(prijevoznik);
        }

        // GET: Prijevoznici/Create
        public IActionResult Create()
        {
            ViewData["IdKorisnik"] = new SelectList(_context.Korisnik, "Id", "Prezime");
            return View();
        }

        // POST: Prijevoznici/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdKorisnik,NazivTvrtke")] Prijevoznik prijevoznik)
        {
            if (ModelState.IsValid)
            {
                _context.Add(prijevoznik);
                await _context.SaveChangesAsync();
                TempData[Constants.Message] = $"Prijevoznik  {prijevoznik.IdKorisnik} uspješno dodan";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdKorisnik"] = new SelectList(_context.Korisnik, "Id", "Prezime", prijevoznik.IdKorisnik);
            return View(prijevoznik);
        }

        // GET: Prijevoznici/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prijevoznik = await _context.Prijevoznik.SingleOrDefaultAsync(m => m.IdKorisnik == id);
            if (prijevoznik == null)
            {
                return NotFound();
            }
            ViewData["IdKorisnik"] = new SelectList(_context.Korisnik, "Id", "Prezime", prijevoznik.IdKorisnik);
            return View(prijevoznik);
        }

        // POST: Prijevoznici/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdKorisnik,NazivTvrtke")] Prijevoznik prijevoznik)
        {
            if (id != prijevoznik.IdKorisnik)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prijevoznik);
                    await _context.SaveChangesAsync();
                    TempData[Constants.Message] = "Ažuriranje uspješno obavljeno";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrijevoznikExists(prijevoznik.IdKorisnik))
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
            ViewData["IdKorisnik"] = new SelectList(_context.Korisnik, "Id", "Prezime", prijevoznik.IdKorisnik);
            return View(prijevoznik);
        }

        // GET: Prijevoznici/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prijevoznik = await _context.Prijevoznik
                .Include(p => p.IdKorisnikNavigation)
                .SingleOrDefaultAsync(m => m.IdKorisnik == id);
            if (prijevoznik == null)
            {
                return NotFound();
            }

            return View(prijevoznik);
        }

        // POST: Prijevoznici/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var prijevoznik = await _context.Prijevoznik.SingleOrDefaultAsync(m => m.IdKorisnik == id);
                _context.Prijevoznik.Remove(prijevoznik);
                await _context.SaveChangesAsync();
                TempData[Constants.Message] = $"Prijevoznik  uspješno obrisan";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception exc)
            {
                TempData[Constants.Message] = "Pogreška prilikom brisanja prijevoznika: " + exc.CompleteExceptionMessage();
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PrijevoznikExists(int id)
        {
            return _context.Prijevoznik.Any(e => e.IdKorisnik == id);
        }
    }
}
