#nullable disable
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
    public class ReklamacjeController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ReklamacjeController(DatabaseContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Reklamacje
        [Authorize(Roles = "Admin,Klient")]
        public async Task<IActionResult> Index()
         {
             var temp = await _context.Reklamacje.ToListAsync();
             var userName = User.Identity?.Name;
             var userId = _userManager.FindByEmailAsync(userName).Result.Id;
             var role = _userManager.GetRolesAsync(_userManager.FindByEmailAsync(userName).Result).Result
                 .FirstOrDefault();
             IQueryable<wypozyczenia> databaseContext;
             if (role == "Admin")
             {
                 return View(await _context.Reklamacje.ToListAsync());
             }
             var rekList = _context.Reklamacje.ToListAsync().Result;
             var dataBase = (from elem in rekList let idWyp = elem.nrWypozyczenia 
                 let wyp = _context.Wypozyczenia.FirstOrDefault(w => w.nrWypozyczenia == idWyp) 
                 where userId == wyp?.IdentityUserId select elem).ToList();
             return View(dataBase);
         }

        // GET: Reklamacje/Details/5
        [Authorize(Roles = "Admin,Klient")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reklamacje = await _context.Reklamacje
                .FirstOrDefaultAsync(m => m.nrReklamacji == id);
            if (reklamacje == null)
            {
                return NotFound();
            }

            return View(reklamacje);
        }

        // GET: Reklamacje/Edit/5
        [Authorize(Roles = "Admin,Klient")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reklamacje = await _context.Reklamacje.FindAsync(id);
            if (reklamacje == null)
            {
                return NotFound();
            }
            return View(reklamacje);
        }

        // POST: Reklamacje/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Klient")]
        public async Task<IActionResult> Edit(int id, [Bind("nrReklamacji,tresc,data,status,nrWypozyczenia")] reklamacje reklamacje)
        {
            if (id != reklamacje.nrReklamacji)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reklamacje);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!reklamacjeExists(reklamacje.nrReklamacji))
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
            return View(reklamacje);
        }

        // GET: Reklamacje/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reklamacje = await _context.Reklamacje
                .FirstOrDefaultAsync(m => m.nrReklamacji == id);
            if (reklamacje == null)
            {
                return NotFound();
            }

            return View(reklamacje);
        }

        // POST: Reklamacje/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reklamacje = await _context.Reklamacje.FindAsync(id);
            _context.Reklamacje.Remove(reklamacje);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool reklamacjeExists(int id)
        {
            return _context.Reklamacje.Any(e => e.nrReklamacji == id);
        }
    }
}
