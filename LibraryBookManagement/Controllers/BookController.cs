using LibraryBookManagement.AppDbContext;
using LibraryBookManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace LibraryBookManagement.Controllers
{
    public class BookController : Controller
    {
        private readonly DBContext _context;

        public BookController(DBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IActionResult> Creates()
        {
            if (_context.Books == null || _context.Members == null)
            {
                return NotFound("Books or Members data not found.");
            }

            ViewBag.BorrowedBooks = new SelectList(await _context.Books.ToListAsync(), "Id", "Title");
            ViewBag.Members = new SelectList(await _context.Members.ToListAsync(), "Id", "Name");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Title,Author,Category,AvailabilityStatus")] BookViewModel bookViewModel)
        {
            if (ModelState.IsValid)
            {
                Book book = new Book
                {
                    Title = bookViewModel.Title,
                    Author = bookViewModel.Author,
                    Category = bookViewModel.Category,
                    AvailabilityStatus = bookViewModel.AvailabilityStatus
                };

                _context.Books.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Detail));
            }
            return View(bookViewModel);
        }
        // GET method
        [HttpGet]
        public async Task<IActionResult> Edits(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            // Convert Book to BookViewModel
            var bookViewModel = new BookViewModel
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Category = book.Category,
                AvailabilityStatus = book.AvailabilityStatus
            };

            return View(bookViewModel);  // Pass ViewModel to the view
        }

        // POST method
        [HttpPost]
        public async Task<IActionResult> Edits(int id, [Bind("Id,Title,Author,Category,AvailabilityStatus")] BookViewModel bookViewModel)
        {
            if (id != bookViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var book = await _context.Books.FindAsync(id);
                    if (book == null)
                    {
                        return NotFound();
                    }

                    // Update book properties with values from ViewModel
                    book.Title = bookViewModel.Title;
                    book.Author = bookViewModel.Author;
                    book.Category = bookViewModel.Category;
                    book.AvailabilityStatus = bookViewModel.AvailabilityStatus;

                    _context.Entry(book).State = EntityState.Modified;  // Mark the entity as modified
                    await _context.SaveChangesAsync();  // Commit the changes

                    return RedirectToAction(nameof(Detail), new { id = book.Id });  // Redirect to detail page
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(bookViewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(bookViewModel);  // Return view if model is invalid
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);  // Check if book exists
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Detail));
        }

        public async Task<IActionResult> Detail(int? id)
        {
            List<Book> books;

            if (id == null)
            {
                books = await _context.Books.ToListAsync(); // Fetch all books
            }
            else
            {
                var book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    return NotFound();
                }
                books = new List<Book> { book }; // Wrap single book in a list
            }

            return View(books); // Send list to the view
        }

    }
}

