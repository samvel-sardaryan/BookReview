namespace BookReview.Dto
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTimeOffset ReleaseDate { get; set; }
    }
}
