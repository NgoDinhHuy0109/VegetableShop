using System;
using System.Collections.Generic;

#nullable disable

namespace Shop.Models
{
    public partial class Size
    {
        public Size()
        {
            Products = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string Title { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
