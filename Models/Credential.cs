using System;
using System.Collections.Generic;

#nullable disable

namespace Shop.Models
{
    public partial class Credential
    {
        public int Id { get; set; }
        public string RoleId { get; set; }
        public string PermissionId { get; set; }

        public virtual Permission Permission { get; set; }
        public virtual Role Role { get; set; }
    }
}
