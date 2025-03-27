using LibraryBookManagement.AppDbContext;
using LibraryBookManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace LibraryBookManagement.Controllers
{
    public class MemberController : Controller
    {
        public DBContext objname;
        public MemberController(DBContext Dbcontext)
        {
            objname = Dbcontext;
        }
        [HttpGet]
        public IActionResult Create()
        {
            var books = objname.Books.Select(b => new { b.Id, b.Title }).ToList();

            ViewBag.BorrowedBooks = new SelectList(books, "Id", "Title");
            return View();
        }
        [HttpPost]
          public async Task<IActionResult> Create([Bind("Name,Email,MembershipData,BorrowedBooks")] MemberViewModel memberViewModel)
          {
            if (ModelState.IsValid)
            {
                Member person = new Member();
                person.Name = memberViewModel.Name;
                person.Email = memberViewModel.Email;
                person.MemberShipData = memberViewModel.MemberShipData;
                if (memberViewModel.BorrowedBooks != null && memberViewModel.BorrowedBooks.Any())
                {
                    person.BorrowedBooks = string.Join(",", memberViewModel.BorrowedBooks);
                }

                objname.Members.Add(person);
                await objname.SaveChangesAsync();
                return RedirectToAction(nameof(Create));
            }
            return View(memberViewModel);
          }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var memberViewModel = await objname.Members.FindAsync(id);
            if (memberViewModel == null)
            {
                return NotFound(memberViewModel);
            }
            return View(memberViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,MembershipData,BorrowedBooks")] MemberViewModel memberViewModel) { 
            if (id != memberViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                Member person = new Member();
                    person.Id = memberViewModel.Id;
                    person.Name = memberViewModel.Name;
                    person.Email = memberViewModel.Email;
                    person.MemberShipData = memberViewModel.MemberShipData;
                if (memberViewModel.BorrowedBooks != null && memberViewModel.BorrowedBooks.Any())
                {
                    person.BorrowedBooks = string.Join(",", memberViewModel.BorrowedBooks);
                }
                objname.Members.Update(person);
                    await objname.SaveChangesAsync(); 
                return RedirectToAction(nameof(Details));
            }
            return View(memberViewModel);
        }

        public bool PersonLibraryExists(int id)
        {
            return objname.Members.Any(e => e.Id == id);
        }
  
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var person = await objname.Members.FindAsync(id);
            if (person != null)
            {
                objname.Members.Remove(person);
                await objname.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details));
        }
        [HttpGet]
        public async Task<IActionResult> Details()
        {
            var members = await objname.Members.ToListAsync();

            ViewBag.Members = members;
            return View();
        }

    }
}
