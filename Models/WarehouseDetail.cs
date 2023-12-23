using System;
using System.Collections.Generic;

#nullable disable

namespace Shop.Models
{
    public partial class WarehouseDetail
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public double? InPrice { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
        public DateTime DateIn { get; set; }
        public int UserId { get; set; }

        public virtual Product Product { get; set; }
        public virtual User User { get; set; }
    }
}
