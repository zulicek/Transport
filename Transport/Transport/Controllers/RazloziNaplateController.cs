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
    public class RazloziNaplateController : Controller
    {
        private readonly transportContext _context;

        public RazloziNaplateController(transportContext context)
        {
            _context = context;
        }

        // GET: RazloziNaplate
        public async Task<IActionResult> Index()
        {
            return View(await _context.RazlogNaplate.ToListAsync());
        }

        // GET: RazloziNaplate/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var razlogNaplate = await _context.RazlogNaplate
                .SingleOrDefaultAsync(m => m.Id == id);
            if (razlogNaplate == null)
            {
                return NotFound();
            }

            return View(razlogNaplate);
        }

        // GET: RazloziNaplate/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RazloziNaplate/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Razlog")] RazlogNaplate razlogNaplate)
        {
            if (ModelState.IsValid)
            {
                _context.Add(razlogNaplate);
                await _context.SaveChangesAsync();
                TempData[Constants.Message] = $"Razlog naplate  {razlogNaplate.Razlog} uspješno dodan";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Index));
            }
            return View(razlogNaplate);
        }

        // GET: RazloziNaplate/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var razlogNaplate = await _context.RazlogNaplate.SingleOrDefaultAsync(m => m.Id == id);
            if (razlogNaplate == null)
            {
                return NotFound();
            }
            return View(razlogNaplate);
        }

        // POST: RazloziNaplate/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Razlog")] RazlogNaplate razlogNaplate)
        {
            if (id != razlogNaplate.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(razlogNaplate);
                    await _context.SaveChangesAsync();
                    TempData[Constants.Message] = "Ažuriranje uspješno obavljeno";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RazlogNaplateExists(razlogNaplate.Id))
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
            return View(razlogNaplate);
        }

        // GET: RazloziNaplate/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var razlogNaplate = await _context.RazlogNaplate
                .SingleOrDefaultAsync(m => m.Id == id);
            if (razlogNaplate == null)
            {
                return NotFound();
            }

            return View(razlogNaplate);
        }

        // POST: RazloziNaplate/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var razlogNaplate = await _context.RazlogNaplate.SingleOrDefaultAsync(m => m.Id == id);
                _context.RazlogNaplate.Remove(razlogNaplate);
                await _context.SaveChangesAsync();
                TempData[Constants.Message] = $"Razlog naplate  {razlogNaplate.Razlog} uspješno obrisan";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception exc)
            {
                TempData[Constants.Message] = "Pogreška prilikom brisanja razloga naplate: " + exc.CompleteExceptionMessage();
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index));
        }

        private bool RazlogNaplateExists(int id)
        {
            return _context.RazlogNaplate.Any(e => e.Id == id);
        }
    }
}
