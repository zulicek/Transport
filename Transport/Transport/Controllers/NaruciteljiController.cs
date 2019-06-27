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
    public class NaruciteljiController : Controller
    {
        private readonly transportContext _context;

        public NaruciteljiController(transportContext context)
        {
            _context = context;
        }

        // GET: Narucitelji
        public async Task<IActionResult> Index()
        {
            var transportContext = _context.Narucitelj.Include(n => n.IdKorisnikNavigation);
            return View(await transportContext.ToListAsync());
        }

        // GET: Narucitelji/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var narucitelj = await _context.Narucitelj
                .Include(n => n.IdKorisnikNavigation)
                .SingleOrDefaultAsync(m => m.IdKorisnik == id);
            if (narucitelj == null)
            {
                return NotFound();
            }

            return View(narucitelj);
        }

        // GET: Narucitelji/Create
        public IActionResult Create()
        {
            ViewData["IdKorisnik"] = new SelectList(_context.Korisnik, "Id", "Prezimes");
            return View();
        }

        // POST: Narucitelji/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdKorisnik,ZahtijevaEko")] Narucitelj narucitelj)
        {
            if (ModelState.IsValid)
            {
                _context.Add(narucitelj);
                await _context.SaveChangesAsync();
                TempData[Constants.Message] = $"Naručitelj {narucitelj.IdKorisnikNavigation} dodan.";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdKorisnik"] = new SelectList(_context.Korisnik, "Id", "Prezime", narucitelj.IdKorisnik);
            return View(narucitelj);
        }

        // GET: Narucitelji/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var narucitelj = await _context.Narucitelj.SingleOrDefaultAsync(m => m.IdKorisnik == id);
            if (narucitelj == null)
            {
                return NotFound();
            }
            ViewData["IdKorisnik"] = new SelectList(_context.Korisnik, "Id", "Prezime", narucitelj.IdKorisnik);
            return View(narucitelj);
        }

        // POST: Narucitelji/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdKorisnik,ZahtijevaEko")] Narucitelj narucitelj)
        {
            if (id != narucitelj.IdKorisnik)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(narucitelj);
                    await _context.SaveChangesAsync();
                    TempData[Constants.Message] = "Ažuriranje uspješno obavljeno";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NaruciteljExists(narucitelj.IdKorisnik))
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
            ViewData["IdKorisnik"] = new SelectList(_context.Korisnik, "Id", "Prezime", narucitelj.IdKorisnik);
            return View(narucitelj);
        }

        // GET: Narucitelji/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var narucitelj = await _context.Narucitelj
                .Include(n => n.IdKorisnikNavigation)
                .SingleOrDefaultAsync(m => m.IdKorisnik == id);
            if (narucitelj == null)
            {
                return NotFound();
            }

            return View(narucitelj);
        }

        // POST: Narucitelji/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var narucitelj = await _context.Narucitelj.SingleOrDefaultAsync(m => m.IdKorisnik == id);
                _context.Narucitelj.Remove(narucitelj);
                await _context.SaveChangesAsync();
                TempData[Constants.Message] = $"Naručitelj  uspješno obrisan";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception exc)
            {
                TempData[Constants.Message] = "Pogreška prilikom brisanja naručitelja: " + exc.CompleteExceptionMessage();
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index));
        }

        private bool NaruciteljExists(int id)
        {
            return _context.Narucitelj.Any(e => e.IdKorisnik == id);
        }
    }
}
