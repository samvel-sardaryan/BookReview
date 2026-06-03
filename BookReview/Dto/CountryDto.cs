using System.ComponentModel.DataAnnotations;

namespace BookReview.Dto
{
    public class CountryDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    }
}
