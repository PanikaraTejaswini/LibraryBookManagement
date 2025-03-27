using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryBookManagement.Models
{
    public class Member
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string MemberShipData { get; set; } = DateTime.Now.ToString("yyyy-mm-dd");
        public string? BorrowedBooks{ get; set; }
    }
}
