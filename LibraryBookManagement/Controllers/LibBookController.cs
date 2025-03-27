using LibraryBookManagement.AppDbContext;
using LibraryBookManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LibraryBookManagement.Controllers
{
    public class LibBookController : Controller
    {
        public  DBContext objname;
        public LibBookController(DBContext Dbcontext)
        {
            objname = Dbcontext;
        }
        public IActionResult Creates()
        {
            ViewBag.BorrowedBooks = new SelectList(objname.Books, "Id", "Title");
            ViewBag.Members = new SelectList(objname.Members, "Id", "Name"); // Assuming dropdown for Members

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Title,Author,Category,AvailabilityStatus")] BookViewModel bookViewModel)
        {
            if (ModelState.IsValid)
            {
                Book person = new Book();
                person.Title = bookViewModel.Title;
                person.Author = bookViewModel.Author;
                person.Category = bookViewModel.Category;
                person.AvailabilityStatus = bookViewModel.AvailabilityStatus;
                objname.Books.Add(person);
                await objname.SaveChangesAsync();
                return RedirectToAction(nameof(Creates));
            }
            return View(bookViewModel);
        }
        [HttpGet]
        public  async Task<IActionResult> Edits(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var bookViewModel = await objname.Books.FindAsync(id);
            if (bookViewModel == null)
            {
                return NotFound(bookViewModel);
            }
            return View(bookViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Edits(int id, [Bind("Title,Author,Category,AvailabilityStatus")] BookViewModel bookViewModel)
        {
            if(id != bookViewModel.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    Book book = new Book();
                    book.Id = bookViewModel.Id;
                    book.Title = bookViewModel.Title;
                    book.Author = bookViewModel.Author;
                    book.Category = bookViewModel.Category;
                    book.AvailabilityStatus = bookViewModel.AvailabilityStatus;
                    objname.Books.Update(book);
                    await objname.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonLibraryExists(bookViewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Detail));
            }
           
            return View(bookViewModel);
        }
        public bool PersonLibraryExists(int id)
        {
            return objname.Books.Any(e => e.Id == id);
        }



        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public  async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var bookViewModel = await objname.Books.FindAsync(id);
            if (bookViewModel != null)
            {
                objname.Books.Remove(bookViewModel);
                await objname.SaveChangesAsync();
            }
            return View(bookViewModel);
        }
        public async  Task<IActionResult> Detail(int? id)
        {
            var members = await objname.Books.ToListAsync(); // Get all members

            ViewBag.Books = members;
            return View();
        }

    }
}
