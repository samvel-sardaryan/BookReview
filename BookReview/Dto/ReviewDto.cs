using System.ComponentModel.DataAnnotations;

namespace BookReview.Dto
{
    public class ReviewDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(2000)]
        public string Text { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [Range(1, int.MaxValue)]
        public int BookId { get; set; }

        [Range(1, int.MaxValue)]
        public int ReviewerId { get; set; }
    }
}
