using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab7.DbConnection;
using Lab7.Model;

namespace Lab7.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly MyDbContext _context;

        public CategoriesController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Categories
        public IActionResult All()
        {
            return View(_context.Categories.ToList());
        }

        // GET: Categories/Details/5
        public IActionResult Details(int id)
        {
            var category = _context.Categories
                .FirstOrDefault(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Add
        public IActionResult Add()
        {
            return View();
        }

        // POST: Categories/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add([Bind("Id,Title,Description,Date")] Category category)
        {
            category.Date = category.Date.ToUniversalTime();
            _context.Add(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(All));
        }

        // GET: Categories/Edit/5
        public IActionResult Edit(int id)
        {

            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            category.Date = category.Date.ToUniversalTime();
           
            try
            {
                _context.Update(category);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(All));
        }

        // GET: Categories/Delete/5
        public IActionResult Delete(int id)
        {

            var category = _context.Categories
                .FirstOrDefault(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int id)
        {
            var category = _context.Categories.Find(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(All));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }

        public IActionResult Stat()
        {
            var categories = _context.Categories.ToList();

            ViewData["Count"] = categories.Count;

            if (categories.Any())
            {
                ViewData["Date"] = new DateTime[] { categories.Min(c => c.Date), categories.Max(c => c.Date) };
            }

            ViewData["Titles"] = categories.Select(c => c.Title).Distinct().ToArray();
            ViewData["Descriptions"] = categories.Select(c => c.Description).Distinct().ToArray();

            return View();
        }

    }
}
