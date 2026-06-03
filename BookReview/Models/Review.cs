namespace BookReview.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTimeOffset ReviewDate { get; set; }
        public Reviewer Reviewer { get; set; } = null!;
        public Book Book { get; set; } = null!;
        public int Rating { get; set; }
    }
}
