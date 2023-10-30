using System.ComponentModel.DataAnnotations;

namespace BusinessObject
{
    public class OrderDetail
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        [Required] public decimal UnitPrice { get; set; }
        [Required] public int Quantity { get; set; }
        [Required] public decimal Discount { get; set; }

        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}
