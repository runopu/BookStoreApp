using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreApp.Models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; }

        [ForeignKey("Book")]
        public int BookId { get; set; }

        public int Quantity { get; set; } = 1;

        public decimal Price { get; set; }

        public Order Order { get; set; }
        public Book Book { get; set; }
    }
}
