using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public string UserEmail { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Processing";

        public List<OrderItem> OrderItems { get; set; }
    }
}
