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
    public class VrsteNaplateController : Controller
    {
        private readonly transportContext _context;

        public VrsteNaplateController(transportContext context)
        {
            _context = context;
        }

        // GET: VrsteNaplate
        public async Task<IActionResult> Index()
        {
            return View(await _context.VrstaNaplate.ToListAsync());
        }

        // GET: VrsteNaplate/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vrstaNaplate = await _context.VrstaNaplate
                .SingleOrDefaultAsync(m => m.Id == id);
            if (vrstaNaplate == null)
            {
                return NotFound();
            }

            return View(vrstaNaplate);
        }

        // GET: VrsteNaplate/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: VrsteNaplate/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Vrsta")] VrstaNaplate vrstaNaplate)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vrstaNaplate);
                await _context.SaveChangesAsync();
                TempData[Constants.Message] = $"Vrsta naplate {vrstaNaplate.Vrsta} uspješno dodana";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Index));
            }
            return View(vrstaNaplate);
        }

        // GET: VrsteNaplate/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vrstaNaplate = await _context.VrstaNaplate.SingleOrDefaultAsync(m => m.Id == id);
            if (vrstaNaplate == null)
            {
                return NotFound();
            }
            return View(vrstaNaplate);
        }

        // POST: VrsteNaplate/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Vrsta")] VrstaNaplate vrstaNaplate)
        {
            if (id != vrstaNaplate.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vrstaNaplate);
                    await _context.SaveChangesAsync();
                    TempData[Constants.Message] = $"Ažuriranje uspješno obavljeno";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VrstaNaplateExists(vrstaNaplate.Id))
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
            return View(vrstaNaplate);
        }

        // GET: VrsteNaplate/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vrstaNaplate = await _context.VrstaNaplate
                .SingleOrDefaultAsync(m => m.Id == id);
            if (vrstaNaplate == null)
            {
                return NotFound();
            }

            return View(vrstaNaplate);
        }

        // POST: VrsteNaplate/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var vrstaNaplate = await _context.VrstaNaplate.SingleOrDefaultAsync(m => m.Id == id);
                _context.VrstaNaplate.Remove(vrstaNaplate);
                await _context.SaveChangesAsync();
                TempData[Constants.Message] = $"Vrsta naplate  {vrstaNaplate.Vrsta} uspješno obrisana";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception exc)
            {
                TempData[Constants.Message] = "Pogreška prilikom brisanja vrste naplate: " + exc.CompleteExceptionMessage();
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index));
        }

        private bool VrstaNaplateExists(int id)
        {
            return _context.VrstaNaplate.Any(e => e.Id == id);
        }
    }
}
