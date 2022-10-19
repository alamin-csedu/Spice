using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Spice.ViewComponents
{
    public class UserNameViewComponent : ViewComponent
    { 
        private readonly ApplicationDbContext dbContxt;

        public UserNameViewComponent(ApplicationDbContext dbContxt)
        {
            this.dbContxt = dbContxt;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {

            var claimItdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimItdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = await dbContxt.ApplicationUserTb.Where(u => u.Id == claim.Value).FirstOrDefaultAsync();

            return View(user);
        }
    }
}
