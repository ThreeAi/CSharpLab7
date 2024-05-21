using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab7.DbConnection;
using Lab7.Model;
using Newtonsoft.Json;
using System.Text;

namespace Lab7.Controllers
{
    public class NotesController : Controller
    {
        private readonly MyDbContext _context;

        public NotesController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Notes
        public IActionResult All(int n = 0, string sort = null)
        {
            IQueryable<Lab7.Model.Note> notesQuery = _context.Notes;

            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort.ToLower())
                {
                    case "title":
                        notesQuery = notesQuery.OrderBy(n => n.Title);
                        break;
                    case "description":
                        notesQuery = notesQuery.OrderBy(n => n.Description);
                        break;
                    case "date":
                        notesQuery = notesQuery.OrderBy(n => n.Date);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                notesQuery = notesQuery.OrderBy(n => n.Id);
            }

            if (n > 0)
            {
                notesQuery = notesQuery.Take(n);
            }

            var notes = notesQuery.ToList();
            ViewData["Sort"] = sort;
            ViewData["N"] = n;

            return View(notes);
        }

        // GET: Notes/Details/5
        public IActionResult Details(int id)
        {

            var note = _context.Notes.Include(n => n.NoteCategories)
                .FirstOrDefault(m => m.Id == id);
            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }

        // GET: Notes/Add
        public IActionResult Add()
        {
            var categories = _context.Categories.ToList();
            ViewBag.Categories = categories;
            return View();
        }

        // POST: Notes/Add
        [HttpPost]
        public IActionResult Add(Note note)
        {
            note.Date = note.Date.ToUniversalTime();
            _context.Add(note);
            _context.SaveChanges();
            return RedirectToAction(nameof(All));        
        }

        public IActionResult BindCategory(int[] SelectedCategoryIds)
        {
            var selectedCategories = _context.Categories.Where(c => SelectedCategoryIds.Contains(c.Id)).ToList();

            var createdNote = _context.Notes
                                  .Include(n => n.NoteCategories)
                                  .OrderByDescending(n => n.Id)
                                  .FirstOrDefault();

            if (createdNote != null && selectedCategories.Any())
            {
                foreach (var category in selectedCategories)
                {
                    createdNote.NoteCategories.Add(category);
                }

                _context.SaveChanges();

                return RedirectToAction(nameof(All));
            }
            else
            {
                return BadRequest(new { error = "Note or categories not found." });
            }
        }

        // GET: Notes/Edit/5
        public IActionResult Edit(int id)
        {

            var note = _context.Notes.Find(id);
            if (note == null)
            {
                return NotFound();
            }
            return View(note);
        }

        // POST: Notes/Edit/5
        [HttpPost]
        public IActionResult Edit(Note note)
        {
            try
            {
                note.Date = note.Date.ToUniversalTime();
                _context.Update(note);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NoteExists(note.Id))
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

        // GET: Notes/Delete/5
        public IActionResult Delete(int id)
        {

            var note = _context.Notes
                .FirstOrDefault(m => m.Id == id);
            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }

        // POST: Notes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int id)
        {
            var note = _context.Notes.Find(id);
            if (note != null)
            {
                _context.Notes.Remove(note);
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(All));
        }

        private bool NoteExists(int id)
        {
            return _context.Notes.Any(e => e.Id == id);
        }

        public IActionResult Stat()
        {
            var notes = _context.Notes.ToList();

            ViewData["Count"] = notes.Count;

            if (notes.Any())
            {
                ViewData["Date"] = new DateTime[] { notes.Min(n => n.Date), notes.Max(n => n.Date) };
            }

            ViewData["Titles"] = notes.Select(n => n.Title).Distinct().ToArray();
            ViewData["Descriptions"] = notes.Select(n => n.Description).Distinct().ToArray();

            return View();
        }
    }
}
