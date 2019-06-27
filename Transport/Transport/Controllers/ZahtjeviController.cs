using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Transport.Extensions;
using Transport.Models;
using Transport.ViewModels;

namespace Transport.Controllers
{
    public class ZahtjeviController : Controller
    {
        private readonly transportContext _context;

        public ZahtjeviController(transportContext context)
        {
            _context = context;
        }

        // GET: Zahtjevi
        public async Task<IActionResult> Index()
        {
            var transportContext = _context.Zahtjev.Include(z => z.IdNaruciteljNavigation).Include(z => z.IdStatusZahtjevaNavigation);
            return View(await transportContext.ToListAsync());

           
        }

        // GET: Zahtjevi/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Zahtjevi/Create
        public IActionResult Create()
        {
            ViewData["IdNarucitelj"] = new SelectList(_context.Narucitelj, "IdKorisnik", "IdKorisnik");
            ViewData["IdStatusZahtjeva"] = new SelectList(_context.StatusZahtjeva, "Id", "Status");
            return View();
        }

        // POST: Zahtjevi/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,VrijemePocetka,VrijemeZavrsetka,CijenaNeizvrsenja,Sirina,Visina,Duiljina,Masa,Opis,IdStatusZahtjeva,IdNarucitelj")] Zahtjev zahtjev)
        {
            if (ModelState.IsValid)
            {
                _context.Add(zahtjev);
                await _context.SaveChangesAsync();
                TempData[Constants.Message] = $"Zahtjev uspješno dodan";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdNarucitelj"] = new SelectList(_context.Narucitelj, "IdKorisnik", "IdKorisnik", zahtjev.IdNarucitelj);
            ViewData["IdStatusZahtjeva"] = new SelectList(_context.StatusZahtjeva, "Id", "Status", zahtjev.IdStatusZahtjeva);
            return View(zahtjev);
        }

        // GET: Zahtjevi/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zahtjev = await _context.Zahtjev.SingleOrDefaultAsync(m => m.Id == id);
            if (zahtjev == null)
            {
                return NotFound();
            }
            ViewData["IdNarucitelj"] = new SelectList(_context.Narucitelj, "IdKorisnik", "IdKorisnik", zahtjev.IdNarucitelj);
            ViewData["IdStatusZahtjeva"] = new SelectList(_context.StatusZahtjeva, "Id", "Status", zahtjev.IdStatusZahtjeva);
            return View(zahtjev);
        }

        // POST: Zahtjevi/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,VrijemePocetka,VrijemeZavrsetka,CijenaNeizvrsenja,Sirina,Visina,Duiljina,Masa,Opis,IdStatusZahtjeva,IdNarucitelj")] Zahtjev zahtjev)
        {
            if (id != zahtjev.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(zahtjev);
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdNarucitelj"] = new SelectList(_context.Narucitelj, "IdKorisnik", "IdKorisnik", zahtjev.IdNarucitelj);
            ViewData["IdStatusZahtjeva"] = new SelectList(_context.StatusZahtjeva, "Id", "Status", zahtjev.IdStatusZahtjeva);
            return View(zahtjev);
        }

        // GET: Zahtjevi/Delete/5
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

        // POST: Zahtjevi/Delete/5
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
            return RedirectToAction(nameof(Index));
        }

        private bool ZahtjevExists(int id)
        {
            return _context.Zahtjev.Any(e => e.Id == id);
        }
    }
}
