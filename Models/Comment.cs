using System;
using System.Collections.Generic;

#nullable disable

namespace Shop.Models
{
    public partial class Comment
    {
        public int Id { get; set; }
        public int? IdPost { get; set; }
        public int? IdUser { get; set; }
        public string Title { get; set; }
        public int? Status { get; set; }

        public virtual Post IdPostNavigation { get; set; }
        public virtual User IdUserNavigation { get; set; }
    }
}
