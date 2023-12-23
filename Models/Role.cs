using System;
using System.Collections.Generic;

#nullable disable

namespace Shop.Models
{
    public partial class Role
    {
        public Role()
        {
            Credentials = new HashSet<Credential>();
            Users = new HashSet<User>();
        }

        public string Id { get; set; }
        public string Title { get; set; }

        public virtual ICollection<Credential> Credentials { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
