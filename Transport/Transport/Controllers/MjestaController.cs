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
    public class MjestaController : Controller
    {
        private readonly transportContext _context;

        public MjestaController(transportContext context)
        {
            _context = context;
        }

        // GET: Mjesta
        public async Task<IActionResult> Index()
        {
            return View(await _context.Mjesto.ToListAsync());
        }

        // GET: Mjesta/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mjesto = await _context.Mjesto
                .SingleOrDefaultAsync(m => m.Id == id);
            if (mjesto == null)
            {
                return NotFound();
            }

            return View(mjesto);
        }

        // GET: Mjesta/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Mjesta/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Naziv,PostanskiBroj")] Mjesto mjesto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mjesto);
                await _context.SaveChangesAsync();
                TempData[Constants.Message] = $"Mjesto {mjesto.Naziv} dodano.";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Index));
            }
            return View(mjesto);
        }

        // GET: Mjesta/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mjesto = await _context.Mjesto.SingleOrDefaultAsync(m => m.Id == id);
            if (mjesto == null)
            {
                return NotFound();
            }
            return View(mjesto);
        }

        // POST: Mjesta/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Naziv,PostanskiBroj")] Mjesto mjesto)
        {
            if (id != mjesto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mjesto);
                    await _context.SaveChangesAsync();
                    TempData[Constants.Message] = "Ažuriranje uspješno obavljeno";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MjestoExists(mjesto.Id))
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
            return View(mjesto);
        }

        // GET: Mjesta/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mjesto = await _context.Mjesto
                .SingleOrDefaultAsync(m => m.Id == id);
            if (mjesto == null)
            {
                return NotFound();
            }

            return View(mjesto);
        }

        // POST: Mjesta/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var mjesto = await _context.Mjesto.SingleOrDefaultAsync(m => m.Id == id);
                _context.Mjesto.Remove(mjesto);
                await _context.SaveChangesAsync();
                TempData[Constants.Message] = $"Mjesto {mjesto.Naziv} uspješno obrisano";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception exc)
            {
                TempData[Constants.Message] = "Pogreška prilikom brisanja mjesta: " + exc.CompleteExceptionMessage();
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index));
        }

        private bool MjestoExists(int id)
        {
            return _context.Mjesto.Any(e => e.Id == id);
        }
    }
}
