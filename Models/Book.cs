using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }

        public decimal Price { get; set; }

        public string CoverImageUrl { get; set; }

        public string? Description { get; set; }

        public DateOnly PublicationDate { get; set; }

        public string? Genre { get; set; }

        public int StockQuantity { get; set; }
    }
}
