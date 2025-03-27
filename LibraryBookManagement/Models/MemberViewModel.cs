using System.ComponentModel.DataAnnotations;

namespace LibraryBookManagement.Models
{
    public class MemberViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string MemberShipData { get; set; } = DateTime.Now.ToString("yyyy-mm-dd");
        public List<int>? BorrowedBooks { get; set; }
    }
}
