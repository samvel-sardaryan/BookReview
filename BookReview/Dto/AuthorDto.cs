using System.ComponentModel.DataAnnotations;

namespace BookReview.Dto
{
    public class AuthorDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Bio { get; set; }

        [Required]
        [StringLength(100)]
        public string CountryName { get; set; }
    }
}
