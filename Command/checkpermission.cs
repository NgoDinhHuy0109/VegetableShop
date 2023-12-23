using Shop.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Command
{
    public class CheckPermission
    {

        public bool HasCredential(String RoleID, String Permission, emmahopContext context)
        {
            var data = from a in context.Credentials // bang cap quyen
                       join b in context.Roles on a.RoleId equals b.Id // bang vai tro
                       join c in context.Permissions on a.PermissionId equals c.Id // bang quyen
                       where b.Id == RoleID
                       select new Credential
                       {
                           RoleId = a.RoleId,
                           PermissionId = a.PermissionId
                       };
            //if (data.Select(x => x.PermissionId).ToList().Contains(Permission) || RoleID.Equals("Admin"))
            //{

            //    return true;
            //}
            return true;
        }
    }
}
