using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Spice.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext context;
        

        public HomeController(ApplicationDbContext context)
        {
            this.context = context;
          
        }

        public async Task<IActionResult> Index()
        {
            IndexViewModel IndexVM = new IndexViewModel()
            {
                MenuItems = await context.MenuItemTb.Include(p => p.Category).Include(m => m.SubCategory).ToListAsync(),
                Categories = await context.CategoryTb.ToListAsync(),
                Coupons = await context.CouponTb.Where(p => p.isActive == true).ToListAsync()

            };
      

            var claimIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var cartList = await context.ShoppingCartTb.Where(u => u.ApplicationUserId == claim.Value).ToListAsync();
                HttpContext.Session.SetInt32("ssCartCount", cartList.Count());
            }

           
            return View(IndexVM);
        }

        public async Task<IActionResult> Details(int id)
        {
           
          var menuItem = await context.MenuItemTb.Include(p => p.Category).Include(m => m.SubCategory).Where(u => u.Id == id).FirstOrDefaultAsync();

            ShoppingCart cartObj = new ShoppingCart()
            {
                MenuItem = menuItem,
                MenuItemId = menuItem.Id
            };

     

            return View(cartObj);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCart cartObj)
        {
            cartObj.Id = 0;
            if (ModelState.IsValid)
            {

                var claimIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                cartObj.ApplicationUserId = claim.Value;

                var cartFromDb = await context.ShoppingCartTb.Where(u => u.ApplicationUserId == cartObj.ApplicationUserId &&
                                                               u.MenuItemId == cartObj.MenuItemId).FirstOrDefaultAsync();
                if(cartFromDb == null)
                {
                    await context.ShoppingCartTb.AddAsync(cartObj);
                }
                else
                {
                    cartFromDb.count += cartObj.count;
                }

               await context.SaveChangesAsync();
            }

            var objList = await context.ShoppingCartTb.Where(u => u.ApplicationUserId == cartObj.ApplicationUserId).ToListAsync();
            var count = objList.Count();
            HttpContext.Session.SetInt32("ssCartCount", count);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
