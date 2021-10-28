using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DemoIdentity.Areas.Identity.Pages.Role
{
    public class IndexModel : PageModel
    {
        RoleManager<IdentityRole> roleMng;

        public IndexModel(RoleManager<IdentityRole> roleMng)
        {
            this.roleMng = roleMng;
        }

        public List<IdentityRole> roles { set; get; }

        [TempData]
        public string StatusMessage { set; get; }

        public async Task<IActionResult> OnGetAsync()
        {
            roles = await roleMng.Roles.ToListAsync();
            return Page();
        }
    }
}
