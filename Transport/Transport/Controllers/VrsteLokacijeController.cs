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
    public class VrsteLokacijeController : Controller
    {
        private readonly transportContext _context;

        public VrsteLokacijeController(transportContext context)
        {
            _context = context;
        }

        // GET: VrsteLokacije
        public async Task<IActionResult> Index()
        {
            return View(await _context.VrstaLokacije.ToListAsync());
        }

        // GET: VrsteLokacije/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vrstaLokacije = await _context.VrstaLokacije
                .SingleOrDefaultAsync(m => m.Id == id);
            if (vrstaLokacije == null)
            {
                return NotFound();
            }

            return View(vrstaLokacije);
        }

        // GET: VrsteLokacije/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: VrsteLokacije/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Vrsta")] VrstaLokacije vrstaLokacije)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vrstaLokacije);
                await _context.SaveChangesAsync();
                TempData[Constants.Message] = $"Vrsta lokacije {vrstaLokacije.Vrsta} uspješno dodana";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Index));
            }
            return View(vrstaLokacije);
        }

        // GET: VrsteLokacije/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vrstaLokacije = await _context.VrstaLokacije.SingleOrDefaultAsync(m => m.Id == id);
            if (vrstaLokacije == null)
            {
                return NotFound();
            }
            return View(vrstaLokacije);
        }

        // POST: VrsteLokacije/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Vrsta")] VrstaLokacije vrstaLokacije)
        {
            if (id != vrstaLokacije.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vrstaLokacije);
                    await _context.SaveChangesAsync();
                    TempData[Constants.Message] = "Ažuriranje uspješno obavljeno";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VrstaLokacijeExists(vrstaLokacije.Id))
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
            return View(vrstaLokacije);
        }

        // GET: VrsteLokacije/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vrstaLokacije = await _context.VrstaLokacije
                .SingleOrDefaultAsync(m => m.Id == id);
            if (vrstaLokacije == null)
            {
                return NotFound();
            }

            return View(vrstaLokacije);
        }

        // POST: VrsteLokacije/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var vrstaLokacije = await _context.VrstaLokacije.SingleOrDefaultAsync(m => m.Id == id);
                _context.VrstaLokacije.Remove(vrstaLokacije);
                await _context.SaveChangesAsync();
                TempData[Constants.Message] = $"Vrsta lokacije {vrstaLokacije.Vrsta} uspješno obrisana";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception exc)
            {
                TempData[Constants.Message] = "Pogreška prilikom brisanja vrste lokacije: " + exc.CompleteExceptionMessage();
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index));
        }

        private bool VrstaLokacijeExists(int id)
        {
            return _context.VrstaLokacije.Any(e => e.Id == id);
        }
    }
}
