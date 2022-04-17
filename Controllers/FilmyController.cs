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
    public class FilmyController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public FilmyController(DatabaseContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Filmy
        [Authorize(Roles = "Admin,Klient")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Filmy.ToListAsync());
        }

        // GET: Filmy/Details/5
        [Authorize(Roles = "Admin,Klient")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filmy = await _context.Filmy
                .FirstOrDefaultAsync(m => m.tytulFilmu == id);
            if (filmy == null)
            {
                return NotFound();
            }

            return View(filmy);
        }

        // GET: Filmy/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Filmy/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("tytulFilmu,rezyser,rokProdukcji,cena,ocena")] filmy filmy)
        {
            if (ModelState.IsValid)
            {
                var temp = await _context.Filmy.FirstOrDefaultAsync(elem => elem.tytulFilmu == filmy.tytulFilmu);
                if (temp == null)
                {
                    _context.Add(filmy);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(filmy);
        }

        // GET: Filmy/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filmy = await _context.Filmy.FindAsync(id);
            if (filmy == null)
            {
                return NotFound();
            }
            return View(filmy);
        }

        // POST: Filmy/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id, [Bind("tytulFilmu,rezyser,rokProdukcji,cena,ocena")] filmy filmy)
        {
            if (id != filmy.tytulFilmu)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(filmy);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!filmyExists(filmy.tytulFilmu))
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
            return View(filmy);
        }

        // GET: Filmy/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filmy = await _context.Filmy
                .FirstOrDefaultAsync(m => m.tytulFilmu == id);
            if (filmy == null)
            {
                return NotFound();
            }

            return View(filmy);
        }

        // POST: Filmy/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var filmy = await _context.Filmy.FindAsync(id);
            foreach (var elem in _context.Wypozyczenia)
            {
                if (elem.tytulFilmu == filmy.tytulFilmu)
                {
                    _context.Wypozyczenia.Remove(elem);
                    // await _context.SaveChangesAsync();
                }
            }
            _context.Filmy.Remove(filmy);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        [Authorize(Roles = "Klient")]
        public async Task<IActionResult> Order (string tytulFilmu)
        {
            var userName = User.Identity.Name;
            var userId = _userManager.FindByEmailAsync(userName).Result.Id;
            var temp = await _context.Wypozyczenia.FirstOrDefaultAsync(elem =>
                elem.IdentityUserId == userId && elem.tytulFilmu == tytulFilmu);
            if (temp == null)
            {
                var wypozyczenie = new wypozyczenia()
                {
                    dataWypozyczenia = DateTime.Now,
                    Waznosc = 14,
                    aktywna = true,
                    film = null,
                    tytulFilmu = tytulFilmu,
                    IdentityUser = _userManager.FindByEmailAsync(userName).Result,
                    IdentityUserId = _userManager.FindByEmailAsync(userName).Result.Id
                };
                _context.Add(wypozyczenie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Errors));
        }

        public IActionResult Errors()
        {
            return View();
        }

        private bool filmyExists(string id)
        {
            return _context.Filmy.Any(e => e.tytulFilmu == id);
        }
    }
}
