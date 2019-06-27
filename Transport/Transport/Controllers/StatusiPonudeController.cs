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
    public class StatusiPonudeController : Controller
    {
        private readonly transportContext _context;

        public StatusiPonudeController(transportContext context)
        {
            _context = context;
        }

        // GET: StatusiPonude
        public async Task<IActionResult> Index()
        {
            return View(await _context.StatusPonude.ToListAsync());
        }

        // GET: StatusiPonude/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statusPonude = await _context.StatusPonude
                .SingleOrDefaultAsync(m => m.Id == id);
            if (statusPonude == null)
            {
                return NotFound();
            }

            return View(statusPonude);
        }

        // GET: StatusiPonude/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: StatusiPonude/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Status")] StatusPonude statusPonude)
        {
            if (ModelState.IsValid)
            {
                _context.Add(statusPonude);
                await _context.SaveChangesAsync();
                TempData[Constants.Message] = $"Status ponude  {statusPonude.Status} uspješno dodan";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Index));
            }
            return View(statusPonude);
        }

        // GET: StatusiPonude/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statusPonude = await _context.StatusPonude.SingleOrDefaultAsync(m => m.Id == id);
            if (statusPonude == null)
            {
                return NotFound();
            }
            return View(statusPonude);
        }

        // POST: StatusiPonude/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Status")] StatusPonude statusPonude)
        {
            if (id != statusPonude.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(statusPonude);
                    await _context.SaveChangesAsync();
                    TempData[Constants.Message] = "Ažuriranje uspješno obavljeno";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StatusPonudeExists(statusPonude.Id))
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
            return View(statusPonude);
        }

        // GET: StatusiPonude/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var statusPonude = await _context.StatusPonude
                .SingleOrDefaultAsync(m => m.Id == id);
            if (statusPonude == null)
            {
                return NotFound();
            }

            return View(statusPonude);
        }

        // POST: StatusiPonude/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var statusPonude = await _context.StatusPonude.SingleOrDefaultAsync(m => m.Id == id);
                _context.StatusPonude.Remove(statusPonude);
                await _context.SaveChangesAsync();
                TempData[Constants.Message] = $"Status ponude  {statusPonude.Status} uspješno obrisan";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception exc)
            {
                TempData[Constants.Message] = "Pogreška prilikom brisanja statusa ponude: " + exc.CompleteExceptionMessage();
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index));
        }

        private bool StatusPonudeExists(int id)
        {
            return _context.StatusPonude.Any(e => e.Id == id);
        }
    }
}
