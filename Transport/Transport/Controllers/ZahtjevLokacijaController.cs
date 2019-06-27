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
    public class ZahtjevLokacijaController : Controller
    {
        private readonly transportContext _context;

        public ZahtjevLokacijaController(transportContext context)
        {
            _context = context;
        }

        // GET: ZahtjevLokacija
        public async Task<IActionResult> Index()
        {
            var transportContext = _context.ZahtjevLokacija.Include(z => z.IdLokacijaNavigation).Include(z => z.IdZahtjevNavigation);
            return View(await transportContext.ToListAsync());
        }

        // GET: ZahtjevLokacija/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zahtjevLokacija = await _context.ZahtjevLokacija
                .Include(z => z.IdLokacijaNavigation)
                .Include(z => z.IdZahtjevNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (zahtjevLokacija == null)
            {
                return NotFound();
            }

            return View(zahtjevLokacija);
        }

        // GET: ZahtjevLokacija/Create
        public IActionResult Create()
        {
            ViewData["IdLokacija"] = new SelectList(_context.Lokacija, "Id", "Ulica");
            ViewData["IdZahtjev"] = new SelectList(_context.Zahtjev, "Id", "Opis");
            return View();
        }

        // POST: ZahtjevLokacija/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdZahtjev,IdLokacija")] ZahtjevLokacija zahtjevLokacija)
        {
            if (ModelState.IsValid)
            {
                _context.Add(zahtjevLokacija);
                await _context.SaveChangesAsync();
                TempData[Constants.Message] = $"Lokacija uspješno dodana zahtjevu";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdLokacija"] = new SelectList(_context.Lokacija, "Id", "Ulica", zahtjevLokacija.IdLokacija);
            ViewData["IdZahtjev"] = new SelectList(_context.Zahtjev, "Id", "Opis", zahtjevLokacija.IdZahtjev);
            return View(zahtjevLokacija);
        }

        // GET: ZahtjevLokacija/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zahtjevLokacija = await _context.ZahtjevLokacija.SingleOrDefaultAsync(m => m.Id == id);
            if (zahtjevLokacija == null)
            {
                return NotFound();
            }
            ViewData["IdLokacija"] = new SelectList(_context.Lokacija, "Id", "Ulica", zahtjevLokacija.IdLokacija);
            ViewData["IdZahtjev"] = new SelectList(_context.Zahtjev, "Id", "Opis", zahtjevLokacija.IdZahtjev);
            return View(zahtjevLokacija);
        }

        // POST: ZahtjevLokacija/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdZahtjev,IdLokacija")] ZahtjevLokacija zahtjevLokacija)
        {
            if (id != zahtjevLokacija.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(zahtjevLokacija);
                    await _context.SaveChangesAsync();
                    TempData[Constants.Message] = "Ažuriranje uspješno obavljeno";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ZahtjevLokacijaExists(zahtjevLokacija.Id))
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
            ViewData["IdLokacija"] = new SelectList(_context.Lokacija, "Id", "Ulica", zahtjevLokacija.IdLokacija);
            ViewData["IdZahtjev"] = new SelectList(_context.Zahtjev, "Id", "Opis", zahtjevLokacija.IdZahtjev);
            return View(zahtjevLokacija);
        }

        // GET: ZahtjevLokacija/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zahtjevLokacija = await _context.ZahtjevLokacija
                .Include(z => z.IdLokacijaNavigation)
                .Include(z => z.IdZahtjevNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (zahtjevLokacija == null)
            {
                return NotFound();
            }

            return View(zahtjevLokacija);
        }

        // POST: ZahtjevLokacija/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var zahtjevLokacija = await _context.ZahtjevLokacija.SingleOrDefaultAsync(m => m.Id == id);
                _context.ZahtjevLokacija.Remove(zahtjevLokacija);
                await _context.SaveChangesAsync();
                TempData[Constants.Message] = $"Lokacija zahtjeva uspješno obrisana";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception exc)
            {
                TempData[Constants.Message] = "Pogreška prilikom brisanja lokacije zahtjeva: " + exc.CompleteExceptionMessage();
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ZahtjevLokacijaExists(int id)
        {
            return _context.ZahtjevLokacija.Any(e => e.Id == id);
        }
    }
}
