using System;
using System.Collections.Generic;

#nullable disable

namespace Shop.Models
{
    public partial class Voucher
    {
        public Voucher()
        {
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }
        public string VoucherCode { get; set; }
        public double? VoucherDiscount { get; set; }
        public string VoucherDescription { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? DeleteAt { get; set; }
        public int? Quantity { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
