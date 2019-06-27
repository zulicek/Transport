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
    public class PrijevoziController : Controller
    {
        private readonly transportContext _context;

        public PrijevoziController(transportContext context)
        {
            _context = context;
        }

        // GET: Prijevozi
        public async Task<IActionResult> Index()
        {
            var transportContext = _context.Prijevoz.Include(p => p.IdPonudaPrijevozaNavigation);
            return View(await transportContext.ToListAsync());
        }

        // GET: Prijevozi/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prijevoz = await _context.Prijevoz
                .Include(p => p.IdPonudaPrijevozaNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (prijevoz == null)
            {
                return NotFound();
            }

            return View(prijevoz);
        }

        // GET: Prijevozi/Create
        public IActionResult Create()
        {
            ViewData["IdPonudaPrijevoza"] = new SelectList(_context.PonudaPrijevoza, "Id", "Id");
            return View();
        }

        // POST: Prijevozi/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OcjenaPrijevoznika,OcjenaNarucitelja,OpisUslugePrijevoznika,OpisUslugeNarucitelja,IdPonudaPrijevoza")] Prijevoz prijevoz)
        {
            if (ModelState.IsValid)
            {
                _context.Add(prijevoz);
                await _context.SaveChangesAsync();
                TempData[Constants.Message] = $"Prijevoz dodan.";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdPonudaPrijevoza"] = new SelectList(_context.PonudaPrijevoza, "Id", "Id", prijevoz.IdPonudaPrijevoza);
            return View(prijevoz);
        }

        // GET: Prijevozi/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prijevoz = await _context.Prijevoz.SingleOrDefaultAsync(m => m.Id == id);
            if (prijevoz == null)
            {
                return NotFound();
            }
            ViewData["IdPonudaPrijevoza"] = new SelectList(_context.PonudaPrijevoza, "Id", "Id", prijevoz.IdPonudaPrijevoza);
            return View(prijevoz);
        }

        // POST: Prijevozi/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OcjenaPrijevoznika,OcjenaNarucitelja,OpisUslugePrijevoznika,OpisUslugeNarucitelja,IdPonudaPrijevoza")] Prijevoz prijevoz)
        {
            if (id != prijevoz.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prijevoz);
                    await _context.SaveChangesAsync();
                    TempData[Constants.Message] = "Ažuriranje uspješno obavljeno";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrijevozExists(prijevoz.Id))
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
            ViewData["IdPonudaPrijevoza"] = new SelectList(_context.PonudaPrijevoza, "Id", "Id", prijevoz.IdPonudaPrijevoza);
            return View(prijevoz);
        }

        // GET: Prijevozi/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prijevoz = await _context.Prijevoz
                .Include(p => p.IdPonudaPrijevozaNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (prijevoz == null)
            {
                return NotFound();
            }

            return View(prijevoz);
        }

        // POST: Prijevozi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var prijevoz = await _context.Prijevoz.SingleOrDefaultAsync(m => m.Id == id);
                _context.Prijevoz.Remove(prijevoz);
                await _context.SaveChangesAsync();
                TempData[Constants.Message] = $"Prijevoz uspješno obrisan";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception exc)
            {
                TempData[Constants.Message] = "Pogreška prilikom brisanja prijevoza: " + exc.CompleteExceptionMessage();
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PrijevozExists(int id)
        {
            return _context.Prijevoz.Any(e => e.Id == id);
        }
    }
}
