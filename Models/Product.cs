using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Shop.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderDetails = new HashSet<OrderDetail>();
            WarehouseDetails = new HashSet<WarehouseDetail>();
        }

        public int Id { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int ProductType { get; set; }
        public int ProductColor { get; set; }
        public int ProductQuantity { get; set; }
        public int ProductBrand { get; set; }
        public string ProductImage { get; set; }
        public int ProductSize { get; set; }
        public string ProductDescription { get; set; }
        public double OutPrice { get; set; }
        public int Status { get; set; }
        public string ProductSpec { get; set; }
        [NotMapped]
        public List<IFormFile> ProfileImage { get; set; }

        public virtual Brand ProductBrandNavigation { get; set; }
        public virtual Color ProductColorNavigation { get; set; }
        public virtual Size ProductSizeNavigation { get; set; }
        public virtual Type ProductTypeNavigation { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<WarehouseDetail> WarehouseDetails { get; set; }
    }
}
