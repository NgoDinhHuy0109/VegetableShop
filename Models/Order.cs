using System;
using System.Collections.Generic;

#nullable disable

namespace Shop.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int Id { get; set; }
        public string OrderId { get; set; }
        public int UserId { get; set; }
        public double TotalPrice { get; set; }
        public DateTime CreateAt { get; set; }
        public int? VoucherId { get; set; }
        public int? Status { get; set; }
        public string Note { get; set; }

        public virtual User User { get; set; }
        public virtual Voucher Voucher { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
