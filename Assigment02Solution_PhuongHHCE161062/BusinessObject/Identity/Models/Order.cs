using BusinessObject.Identity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusinessObject
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public string MemberId { get; set; }
        public DateTime OrderDate { init; get; } = DateTime.Now;
        [Required] public DateTime RequiredDate { get; set; }
        public DateTime ShippedDate { get; set; }
        public decimal Freight { get; set; }
        public ApplicationUser Member { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
