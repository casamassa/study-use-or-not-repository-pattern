public class Game : BaseEntity
{
    public required string Name { get; set; }
    public int GenreId { get; set; }
    public Genre? Genre { get; set; }
    public Decimal Price { get; set; }
    public DateOnly ReleaseDate { get; set; }
}