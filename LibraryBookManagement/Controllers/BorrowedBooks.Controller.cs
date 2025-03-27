using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryBookManagement.Models;
using LibraryBookManagement.AppDbContext;

namespace LibraryManagementSystem.Controllers
{
    public class BorrowController : Controller
    {
        private readonly DBContext _context;

        public BorrowController(DBContext context)
        {
            _context = context;
        }

        
        public  IActionResult Index()
        {
            var borrowedBooks = _context.Borroweds
                .Include(b => b.Book)
                .Include(b => b.Member)
                .Where(b => b.ReturnDate == null);
            return View(borrowedBooks);
        }
        public async Task<IActionResult> BorrowBook()
        {
            ViewBag.Books = await _context.Books.Where(b => b.AvailabilityStatus).ToListAsync();
            ViewBag.Members = await _context.Members.ToListAsync();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> BorrowBook(int bookId, int memberId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null || !book.AvailabilityStatus)
            {
                return NotFound();
            }

            var member = await _context.Members.FindAsync(memberId);
            if (member == null)
            {
                return NotFound();
            }

            var borrow = new Borrowed
            {
                BookId = bookId,
                MemberId = memberId,
                BorrowedDate = DateTime.Now
            };

            book.AvailabilityStatus = false;

            _context.Add(borrow);
            _context.Update(book);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ReturnBook()
        {
            var borrowedBooks = await _context.Borroweds
                .Include(b => b.Book)
                .Include(b => b.Member)
                .Where(b => b.ReturnDate == null)
                .ToListAsync();

            return View(borrowedBooks);
        }
        [HttpPost]

        public async Task<IActionResult> ReturnBookConfirmed(int id)
        {
            var borrowedBook = await _context.Borroweds.FindAsync(id);
            if (borrowedBook == null)
            {
                return NotFound();
            }

            // Mark the book as available
            var book = await _context.Books.FindAsync(borrowedBook.BookId);
            if (book != null)
            {
                book.AvailabilityStatus = true;
                _context.Update(book);
            }

            // Set the return date
            borrowedBook.ReturnDate = DateTime.Now;
            _context.Update(borrowedBook);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));  // Refresh the list
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var borrowedBook = await _context.Borroweds
                .Include(b => b.Book)
                .Include(b => b.Member)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (borrowedBook == null)
            {
                return NotFound();
            }

            return View(borrowedBook);
            
        }


    }
}

