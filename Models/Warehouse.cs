using System;
using System.Collections.Generic;

#nullable disable

namespace Shop.Models
{
    public partial class Warehouse
    {
        public int Id { get; set; }
        public string ImportCode { get; set; }
        public int? UserId { get; set; }
        public DateTime? DateIn { get; set; }

        public virtual User User { get; set; }
    }
}
