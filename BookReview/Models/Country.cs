namespace BookReview.Models
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<Author> Authors { get; set; } = new List<Author>();
    }
}
