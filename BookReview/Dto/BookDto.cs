using System.ComponentModel.DataAnnotations;

namespace BookReview.Dto
{
    public class BookDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        public DateTimeOffset ReleaseDate { get; set; }
    }
}
