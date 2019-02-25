using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ContosoUniversity.Data;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ContosoUniversity.Controllers
{
    public class StudentsController : Controller
    {
    
        private readonly SchoolContext _context;

        public StudentsController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            return View(await _context.Students.ToListAsync());
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .AsNoTracking()
                    .SingleOrDefaultAsync(m => m.ID == id);

            if (student == null) return NotFound();

            return View(student); 
                                                                                 
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EnrollmentDate,FirstMidName, LastName")] Student student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(student);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));

                }


            } catch (DbUpdateException /* ex */)
            {
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator");
            }

            return View(student);
        }


        //GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students.SingleOrDefaultAsync(m => m.ID == id);

            if (student == null) return NotFound();

            return View(student);

        }

        //POST: Students/Edit/5
        [HttpPost, ActionName("Edit")]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null) return NotFound();

            var studentToUpdate = await _context.Students.SingleOrDefaultAsync(m => m.ID == id);

            if (await TryUpdateModelAsync<Student>(studentToUpdate, "", s => s.FirstMidName, s => s.LastName, s => s.EnrollmentDate ))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }

            return View(studentToUpdate);
        }



        //GET: Students/delete/5
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null) return NotFound();

            var studentToDelete = await _context.Students.AsNoTracking().SingleOrDefaultAsync(s => s.ID == id);

            if (studentToDelete == null) return NotFound();

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }

            return View(studentToDelete);

        }

        //POST: Students/delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studentToDelete = await _context.Students
                               .AsNoTracking()
                               .SingleOrDefaultAsync(m => m.ID == id);

            if (studentToDelete == null) RedirectToAction(nameof(Index));

            try
            {
                _context.Students.Remove(studentToDelete);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            } catch (DbUpdateException /* ex */)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }   

        }
    } //END STUDENTCONTROLLER
} //END NAMESPACE
