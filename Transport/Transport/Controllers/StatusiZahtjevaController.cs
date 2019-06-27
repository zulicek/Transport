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
    public class StatusiZahtjevaController : Controller
    {
        private readonly transportContext _context;

        public StatusiZahtjevaController(transportContext context)
        {
            _context = context;
        }

        // GET: StatusiZahtjeva
        public async Task<IActionResult> Index()
        {
            return View(await _context.StatusZahtjeva.ToListAsync());
        }

        // GET: StatusiZahtjeva/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statusZahtjeva = await _context.StatusZahtjeva
                .SingleOrDefaultAsync(m => m.Id == id);
            if (statusZahtjeva == null)
            {
                return NotFound();
            }

            return View(statusZahtjeva);
        }

        // GET: StatusiZahtjeva/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: StatusiZahtjeva/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Status")] StatusZahtjeva statusZahtjeva)
        {
            if (ModelState.IsValid)
            {
                _context.Add(statusZahtjeva);
                await _context.SaveChangesAsync();
                TempData[Constants.Message] = $"Status zahtjeva  {statusZahtjeva.Status} uspješno dodan";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Index));
            }
            return View(statusZahtjeva);
        }

        // GET: StatusiZahtjeva/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statusZahtjeva = await _context.StatusZahtjeva.SingleOrDefaultAsync(m => m.Id == id);
            if (statusZahtjeva == null)
            {
                return NotFound();
            }
            return View(statusZahtjeva);
        }

        // POST: StatusiZahtjeva/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Status")] StatusZahtjeva statusZahtjeva)
        {
            if (id != statusZahtjeva.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(statusZahtjeva);
                    await _context.SaveChangesAsync();
                    TempData[Constants.Message] = $"Ažuriranje uspješno obavljeno";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StatusZahtjevaExists(statusZahtjeva.Id))
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
            return View(statusZahtjeva);
        }

        // GET: StatusiZahtjeva/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statusZahtjeva = await _context.StatusZahtjeva
                .SingleOrDefaultAsync(m => m.Id == id);
            if (statusZahtjeva == null)
            {
                return NotFound();
            }

            return View(statusZahtjeva);
        }

        // POST: StatusiZahtjeva/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var statusZahtjeva = await _context.StatusZahtjeva.SingleOrDefaultAsync(m => m.Id == id);
                _context.StatusZahtjeva.Remove(statusZahtjeva);
                await _context.SaveChangesAsync();
                TempData[Constants.Message] = $"Status zahtjeva  {statusZahtjeva.Status} uspješno obrisan";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception exc)
            {
                TempData[Constants.Message] = "Pogreška prilikom brisanja statusa zahtjeva: " + exc.CompleteExceptionMessage();
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index));
        }

        private bool StatusZahtjevaExists(int id)
        {
            return _context.StatusZahtjeva.Any(e => e.Id == id);
        }
    }
}
