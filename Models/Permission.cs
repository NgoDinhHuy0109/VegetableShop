using System;
using System.Collections.Generic;

#nullable disable

namespace Shop.Models
{
    public partial class Permission
    {
        public Permission()
        {
            Credentials = new HashSet<Credential>();
        }

        public string Id { get; set; }
        public string Title { get; set; }

        public virtual ICollection<Credential> Credentials { get; set; }
    }
}
