using System;
using System.Collections.Generic;

#nullable disable

namespace Shop.Models
{
    public partial class User
    {
        public User()
        {
            Comments = new HashSet<Comment>();
            Orders = new HashSet<Order>();
            Posts = new HashSet<Post>();
            WarehouseDetails = new HashSet<WarehouseDetail>();
            Warehouses = new HashSet<Warehouse>();
        }

        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Fullname { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public int Status { get; set; }
        public string RoleId { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<WarehouseDetail> WarehouseDetails { get; set; }
        public virtual ICollection<Warehouse> Warehouses { get; set; }
    }
}
