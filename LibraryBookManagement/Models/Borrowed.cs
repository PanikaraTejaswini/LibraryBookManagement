using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryBookManagement.Models
{
    public class Borrowed
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Member")]
        public int MemberId { get; set; }
        [ForeignKey("Book")]
        public int BookId { get; set; }
        public DateTime BorrowedDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public Book Book { get; set; }
        public Member Member { get; set; }

    }
}
