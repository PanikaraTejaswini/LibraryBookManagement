using System.ComponentModel.DataAnnotations;

namespace LibraryBookManagement.Models
{
    public class BookViewModel
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        [Required]
        public string? Author { get; set; }
        [Required]
        public string? Category { get; set; }
        public bool AvailabilityStatus { get; set; }
    }
}
