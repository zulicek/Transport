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
    public class NaplateController : Controller
    {
        private readonly transportContext _context;

        public NaplateController(transportContext context)
        {
            _context = context;
        }

        // GET: Naplate
        public async Task<IActionResult> Index()
        {
            var transportContext = _context.Naplata.Include(n => n.IdPrijevozNavigation).Include(n => n.IdRazlogNavigation).Include(n => n.IdVrstaNaplateNavigation);
            return View(await transportContext.ToListAsync());
        }

        // GET: Naplate/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var naplata = await _context.Naplata
                .Include(n => n.IdPrijevozNavigation)
                .Include(n => n.IdRazlogNavigation)
                .Include(n => n.IdVrstaNaplateNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (naplata == null)
            {
                return NotFound();
            }

            return View(naplata);
        }

        // GET: Naplate/Create
        public IActionResult Create()
        {
            ViewData["IdPrijevoz"] = new SelectList(_context.Prijevoz, "Id", "Id");
            ViewData["IdRazlog"] = new SelectList(_context.RazlogNaplate, "Id", "Razlog");
            ViewData["IdVrstaNaplate"] = new SelectList(_context.VrstaNaplate, "Id", "Vrsta");
            return View();
        }

        // POST: Naplate/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdRazlog,IdVrstaNaplate,IdPrijevoz,RokIzvrsenjaNaplate")] Naplata naplata)
        {
            if (ModelState.IsValid)
            {
                _context.Add(naplata);
                await _context.SaveChangesAsync();
                TempData[Constants.Message] = $"Naplata dodana.";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdPrijevoz"] = new SelectList(_context.Prijevoz, "Id", "Id", naplata.IdPrijevoz);
            ViewData["IdRazlog"] = new SelectList(_context.RazlogNaplate, "Id", "Razlog", naplata.IdRazlog);
            ViewData["IdVrstaNaplate"] = new SelectList(_context.VrstaNaplate, "Id", "Vrsta", naplata.IdVrstaNaplate);
            return View(naplata);
        }

        // GET: Naplate/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var naplata = await _context.Naplata.SingleOrDefaultAsync(m => m.Id == id);
            if (naplata == null)
            {
                return NotFound();
            }
            ViewData["IdPrijevoz"] = new SelectList(_context.Prijevoz, "Id", "Id", naplata.IdPrijevoz);
            ViewData["IdRazlog"] = new SelectList(_context.RazlogNaplate, "Id", "Razlog", naplata.IdRazlog);
            ViewData["IdVrstaNaplate"] = new SelectList(_context.VrstaNaplate, "Id", "Vrsta", naplata.IdVrstaNaplate);
            return View(naplata);
        }

        // POST: Naplate/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdRazlog,IdVrstaNaplate,IdPrijevoz,RokIzvrsenjaNaplate")] Naplata naplata)
        {
            if (id != naplata.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(naplata);
                    await _context.SaveChangesAsync();
                    TempData[Constants.Message] = $"Ažuriranje uspješno obavljeno";
                    TempData[Constants.ErrorOccurred] = false;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NaplataExists(naplata.Id))
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
            ViewData["IdPrijevoz"] = new SelectList(_context.Prijevoz, "Id", "Id", naplata.IdPrijevoz);
            ViewData["IdRazlog"] = new SelectList(_context.RazlogNaplate, "Id", "Razlog", naplata.IdRazlog);
            ViewData["IdVrstaNaplate"] = new SelectList(_context.VrstaNaplate, "Id", "Vrsta", naplata.IdVrstaNaplate);
            return View(naplata);
        }

        // GET: Naplate/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var naplata = await _context.Naplata
                .Include(n => n.IdPrijevozNavigation)
                .Include(n => n.IdRazlogNavigation)
                .Include(n => n.IdVrstaNaplateNavigation)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (naplata == null)
            {
                return NotFound();
            }

            return View(naplata);
        }

        // POST: Naplate/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var naplata = await _context.Naplata.SingleOrDefaultAsync(m => m.Id == id);
                _context.Naplata.Remove(naplata);
                await _context.SaveChangesAsync();
                TempData[Constants.Message] = $"Naplata uspješno obrisana";
                TempData[Constants.ErrorOccurred] = false;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception exc)
            {
                TempData[Constants.Message] = "Pogreška prilikom brisanja naplate: " + exc.CompleteExceptionMessage();
                TempData[Constants.ErrorOccurred] = true;
            }
            return RedirectToAction(nameof(Index));
        }

        private bool NaplataExists(int id)
        {
            return _context.Naplata.Any(e => e.Id == id);
        }
    }
}
