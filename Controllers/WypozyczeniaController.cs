using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WypozyczalniaFilmow.Data;
using WypozyczalniaFilmow.Models;

namespace WypozyczalniaFilmow.Controllers
{
    public class WypozyczeniaController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public WypozyczeniaController(DatabaseContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Wypozyczenia
        [Authorize(Roles = "Admin,Klient")]
        public async Task<IActionResult> Index()
        {
            var temp = await _context.Wypozyczenia.ToListAsync();
            var userName = User.Identity.Name;
            var userId = _userManager.FindByEmailAsync(userName).Result.Id;
            var role = _userManager.GetRolesAsync(_userManager.FindByEmailAsync(userName).Result).Result
                .FirstOrDefault();
            IQueryable<wypozyczenia> databaseContext;
            if (role == "Admin")
            {
                return View(await _context.Wypozyczenia.ToListAsync());
            }
            databaseContext = _context.Wypozyczenia.Where(w => w.IdentityUserId == userId);
            return View(await databaseContext.ToListAsync());
        }

        // GET: Wypozyczenia/Details/5
        [Authorize(Roles = "Admin,Klient")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wypozyczenia = await _context.Wypozyczenia
                .Include(w => w.IdentityUser)
                .FirstOrDefaultAsync(m => m.nrWypozyczenia == id);
            if (wypozyczenia == null)
            {
                return NotFound();
            }

            return View(wypozyczenia);
        }

        // GET: Wypozyczenia/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wypozyczenia = await _context.Wypozyczenia.FindAsync(id);
            if (wypozyczenia == null)
            {
                return NotFound();
            }
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", wypozyczenia.IdentityUserId);
            return View(wypozyczenia);
        }

        // POST: Wypozyczenia/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("nrWypozyczenia,dataWypozyczenia,Waznosc,aktywna,tytulFilmu,IdentityUserId")] wypozyczenia wypozyczenia)
        {
            if (id != wypozyczenia.nrWypozyczenia)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(wypozyczenia);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!wypozyczeniaExists(wypozyczenia.nrWypozyczenia))
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
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", wypozyczenia.IdentityUserId);
            return View(wypozyczenia);
        }

        // GET: Wypozyczenia/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wypozyczenia = await _context.Wypozyczenia
                .Include(w => w.IdentityUser)
                .FirstOrDefaultAsync(m => m.nrWypozyczenia == id);
            if (wypozyczenia == null)
            {
                return NotFound();
            }

            return View(wypozyczenia);
        }

        // POST: Wypozyczenia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var wypozyczenia = await _context.Wypozyczenia.FindAsync(id);
            _context.Wypozyczenia.Remove(wypozyczenia);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        [Authorize(Roles = "Klient")]
        public async Task<IActionResult> Complain (int? nrWypozyczenia)
        {
            var temp = await _context.Reklamacje.FirstOrDefaultAsync(elem =>
                elem.nrWypozyczenia == nrWypozyczenia);
            if (temp == null)
            {
                return View();
            }
            return RedirectToAction(nameof(Errors));
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Klient")]
        public async Task<IActionResult> Complain(int id, [Bind("tresc")] reklamacje reklamacje)
        {
            if (ModelState.IsValid)
            {
                reklamacje.data = DateTime.Now;
                reklamacje.status = Status.Rozpatrywana;
                reklamacje.wypozyczenie = null;
                reklamacje.nrWypozyczenia = id;
                _context.Add(reklamacje);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(reklamacje);
        }

        public IActionResult Errors()
        {
            return View();
        }

        private bool wypozyczeniaExists(int id)
        {
            return _context.Wypozyczenia.Any(e => e.nrWypozyczenia == id);
        }
    }
}
