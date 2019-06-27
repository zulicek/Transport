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
    public class LokacijeController : Controller
    {
        private readonly transportContext _context;

        public LokacijeController(transportContext context)
        {
            _context = context;
        }

        // GET: Lokacije
        public async Task<IActionResult> Index()
        {
            var transportContext = _context.Lokacija.Include(l => l.IdMjestoNavigation).Include(l => l.IdVrstaLokacijeNavigation);
            return View(await transportContext.ToListAsync());
        }

        // GET: Lokacije/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lokacija = await _context.Lokacija
                .Include(l => l.IdMjestoNavigation)
                .Include(l => l.IdVrstaLokacijeNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (lokacija == null)
            {
                return NotFound();
            }

            return View(lokacija);
        }

        // GET: Lokacije/Create
        public IActionResult Create()
        {
            ViewData["IdMjesto"] = new SelectList(_context.Mjesto, "Id", "Naziv");
            ViewData["IdVrstaLokacije"] = new SelectList(_context.VrstaLokacije, "Id", "Vrsta");
            return View();
        }

        // POST: Lokacije/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdMjesto,IdVrstaLokacije,Ulica,KucniBroj")] Lokacija lokacija)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lokacija);
                await _context.SaveChangesAsync();
                TempData[Constants.Message] = $"Lokacija {lokacija.Ulica} {lokacija.KucniBroj} dodana.";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdMjesto"] = new SelectList(_context.Mjesto, "Id", "Naziv", lokacija.IdMjesto);
            ViewData["IdVrstaLokacije"] = new SelectList(_context.VrstaLokacije, "Id", "Vrsta", lokacija.IdVrstaLokacije);
            return View(lokacija);
        }

        // GET: Lokacije/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lokacija = await _context.Lokacija.SingleOrDefaultAsync(m => m.Id == id);
            if (lokacija == null)
            {
                return NotFound();
            }
            ViewData["IdMjesto"] = new SelectList(_context.Mjesto, "Id", "Naziv", lokacija.IdMjesto);
            ViewData["IdVrstaLokacije"] = new SelectList(_context.VrstaLokacije, "Id", "Vrsta", lokacija.IdVrstaLokacije);
            return View(lokacija);
        }

        // POST: Lokacije/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdMjesto,IdVrstaLokacije,Ulica,KucniBroj")] Lokacija lokacija)
        {
            if (id != lokacija.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lokacija);
                    await _context.SaveChangesAsync();
                    TempData[Constants.Message] = "Ažuriranje uspješno obavljeno";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LokacijaExists(lokacija.Id))
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
            ViewData["IdMjesto"] = new SelectList(_context.Mjesto, "Id", "Naziv", lokacija.IdMjesto);
            ViewData["IdVrstaLokacije"] = new SelectList(_context.VrstaLokacije, "Id", "Vrsta", lokacija.IdVrstaLokacije);
            return View(lokacija);
        }

        // GET: Lokacije/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lokacija = await _context.Lokacija
                .Include(l => l.IdMjestoNavigation)
                .Include(l => l.IdVrstaLokacijeNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (lokacija == null)
            {
                return NotFound();
            }

            return View(lokacija);
        }

        // POST: Lokacije/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var lokacija = await _context.Lokacija.SingleOrDefaultAsync(m => m.Id == id);
                _context.Lokacija.Remove(lokacija);
                await _context.SaveChangesAsync();
                TempData[Constants.Message] = $"Lokacija {lokacija.Ulica}  {lokacija.KucniBroj} uspješno obrisana";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception exc)
            {
                TempData[Constants.Message] = "Pogreška prilikom brisanja lokacije: " + exc.CompleteExceptionMessage();
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index));
        }

        private bool LokacijaExists(int id)
        {
            return _context.Lokacija.Any(e => e.Id == id);
        }
    }
}
