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

        // Index to list all borrowed books
        public IActionResult Index()
        {
            var borrowedBooks = _context.Borroweds
                .Include(b => b.Book)
                .Include(b => b.Member)
                .Where(b => b.ReturnDate == null) // Only books that are not returned
                .ToList();
            return View(borrowedBooks);
        }

        // GET: BorrowBook - to show the form for borrowing a book
        public async Task<IActionResult> BorrowBook()
        {
            // Get available books and members
            ViewBag.Books = await _context.Books.Where(b => b.AvailabilityStatus).ToListAsync();
            ViewBag.Members = await _context.Members.ToListAsync();
            return View();
        }

        // POST: BorrowBook - to process the borrowing request
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

            // Create a new Borrowed entry
            var borrow = new Borrowed
            {
                BookId = bookId,
                MemberId = memberId,
                BorrowedDate = DateTime.Now
            };

            // Mark the book as unavailable (borrowed)
            book.AvailabilityStatus = false;

            _context.Add(borrow);
            _context.Update(book);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));  // Redirect to the list of borrowed books
        }

        // Return Book View - Show the borrowed books that have not been returned yet
        public async Task<IActionResult> ReturnBook()
        {
            var borrowedBooks = await _context.Borroweds
                .Include(b => b.Book)
                .Include(b => b.Member)
                .Where(b => b.ReturnDate == null) // Not returned yet
                .ToListAsync();

            return View(borrowedBooks);
        }

        // POST: ReturnBookConfirmed - When the book is returned
        [HttpPost]
        public async Task<IActionResult> ReturnBookConfirmed(int id)
        {
            var borrowedBook = await _context.Borroweds.FindAsync(id);
            if (borrowedBook == null)
            {
                return NotFound();
            }

            // Mark the book as available again
            var book = await _context.Books.FindAsync(borrowedBook.BookId);
            if (book != null)
            {
                book.AvailabilityStatus = true; // Book is now available
                _context.Update(book);
            }

            // Set the return date for the borrowed book
            borrowedBook.ReturnDate = DateTime.Now;
            _context.Update(borrowedBook);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));  // Refresh the list
        }
    }
}
