using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Spice.Areas.Customer.Controllers
{
   
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public CartController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [BindProperty]
        public OrderDetailsCart OrderDetailsCart { get; set; }
        public async Task<IActionResult> Index()
        {
            OrderDetailsCart = new OrderDetailsCart()
            {
                OrderHeader = new OrderHeader()
            };
            var claimItdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimItdentity.FindFirst(ClaimTypes.NameIdentifier);

            var CartList = await dbContext.ShoppingCartTb.Where(u => u.ApplicationUserId == claim.Value).ToListAsync();
            if(CartList != null)
            {
                OrderDetailsCart.shoppingCarts = CartList;
            }

            foreach(var cart in OrderDetailsCart.shoppingCarts)
            {
                cart.MenuItem = await dbContext.MenuItemTb.Where(u => u.Id == cart.MenuItemId).FirstOrDefaultAsync();
                OrderDetailsCart.OrderHeader.OrderTotal += (cart.MenuItem.Price * cart.count);
                cart.MenuItem.Description = SD.ConvertToRawHtml(cart.MenuItem.Description);

                if(cart.MenuItem.Description.Length > 100)
                {
                    cart.MenuItem.Description = cart.MenuItem.Description.Substring(0, 99) + "...";
                }
            }
            OrderDetailsCart.OrderHeader.OrderTotalOrginal = OrderDetailsCart.OrderHeader.OrderTotal;

            if(HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                OrderDetailsCart.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponDb = await dbContext.CouponTb.Where(u => u.Name.ToLower() == OrderDetailsCart.OrderHeader.CouponCode.ToLower()).FirstOrDefaultAsync();
                OrderDetailsCart.OrderHeader.OrderTotal = SD.DiscountedPrice(couponDb, OrderDetailsCart.OrderHeader.OrderTotalOrginal);
            }



            return View(OrderDetailsCart);
        }


        public async Task<IActionResult> Summary()
        {
            OrderDetailsCart = new OrderDetailsCart()
            {
                OrderHeader = new OrderHeader()
            };
            var claimItdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimItdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = await dbContext.ApplicationUserTb.Where(w => w.Id == claim.Value).FirstOrDefaultAsync();

            var CartList = await dbContext.ShoppingCartTb.Where(u => u.ApplicationUserId == claim.Value).ToListAsync();
            if (CartList != null)
            {
                OrderDetailsCart.shoppingCarts = CartList;
            }

            foreach (var cart in OrderDetailsCart.shoppingCarts)
            {
                cart.MenuItem = await dbContext.MenuItemTb.Where(u => u.Id == cart.MenuItemId).FirstOrDefaultAsync();
                OrderDetailsCart.OrderHeader.OrderTotal += (cart.MenuItem.Price * cart.count);
                cart.MenuItem.Description = SD.ConvertToRawHtml(cart.MenuItem.Description);

            }
            OrderDetailsCart.OrderHeader.OrderTotalOrginal = OrderDetailsCart.OrderHeader.OrderTotal;
            OrderDetailsCart.OrderHeader.PickUpName = user.Name;
            OrderDetailsCart.OrderHeader.PickupNumber = user.PhoneNumber;
            OrderDetailsCart.OrderHeader.PickupDate = DateTime.Now;


            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                OrderDetailsCart.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponDb = await dbContext.CouponTb.Where(u => u.Name.ToLower() == OrderDetailsCart.OrderHeader.CouponCode.ToLower()).FirstOrDefaultAsync();
                OrderDetailsCart.OrderHeader.OrderTotal = SD.DiscountedPrice(couponDb, OrderDetailsCart.OrderHeader.OrderTotalOrginal);
            }



            return View(OrderDetailsCart);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost()
        {

            var claimItdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimItdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderDetailsCart.shoppingCarts = await dbContext.ShoppingCartTb.Where(u => u.ApplicationUserId == claim.Value).ToListAsync();

            OrderDetailsCart.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            OrderDetailsCart.OrderHeader.OrderDate = DateTime.Now;
            OrderDetailsCart.OrderHeader.UserId = claim.Value;
            OrderDetailsCart.OrderHeader.Status = SD.PaymentStatusPending;
            OrderDetailsCart.OrderHeader.PickupTime = Convert.ToDateTime(OrderDetailsCart.OrderHeader.PickupDate.ToShortDateString() + " " + OrderDetailsCart.OrderHeader.PickupTime.ToShortTimeString());


            List<OrderDetails> orderDetailsList = new List<OrderDetails>();
            dbContext.OrderHeaderTb.Add(OrderDetailsCart.OrderHeader);
            await dbContext.SaveChangesAsync();

            OrderDetailsCart.OrderHeader.OrderTotalOrginal = 0;


            foreach (var item in OrderDetailsCart.shoppingCarts)
            {
                item.MenuItem = await dbContext.MenuItemTb.FirstOrDefaultAsync(m => m.Id == item.MenuItemId);
                OrderDetails orderDetails = new OrderDetails
                {
                    MenuItemId = item.MenuItemId,
                    OrderId = OrderDetailsCart.OrderHeader.Id,
                    Description = item.MenuItem.Description,
                    Name = item.MenuItem.Name,
                    Price = item.MenuItem.Price,
                    Count = item.count
                };
                OrderDetailsCart.OrderHeader.OrderTotalOrginal += orderDetails.Count * orderDetails.Price;
                dbContext.OrderDetailsTb.Add(orderDetails);

            }

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                OrderDetailsCart.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = await dbContext.CouponTb.Where(c => c.Name.ToLower() == OrderDetailsCart.OrderHeader.CouponCode.ToLower()).FirstOrDefaultAsync();
                OrderDetailsCart.OrderHeader.OrderTotal = SD.DiscountedPrice(couponFromDb, OrderDetailsCart.OrderHeader.OrderTotalOrginal);
            }
            else
            {
                OrderDetailsCart.OrderHeader.OrderTotal = OrderDetailsCart.OrderHeader.OrderTotalOrginal;
            }
            OrderDetailsCart.OrderHeader.Discount = OrderDetailsCart.OrderHeader.OrderTotalOrginal - OrderDetailsCart.OrderHeader.OrderTotal;

            dbContext.ShoppingCartTb.RemoveRange(OrderDetailsCart.shoppingCarts);
            HttpContext.Session.SetInt32("ssCartCount", 0);
            await dbContext.SaveChangesAsync();

          /*  var options = new ChargeCreateOptions
            {
                Amount = Convert.ToInt32(detailCart.OrderHeader.OrderTotal * 100),
                Currency = "usd",
                Description = "Order ID : " + detailCart.OrderHeader.Id,
                Source = stripeToken

            };
            var service = new ChargeService();
            Charge charge = service.Create(options);

            if (charge.BalanceTransactionId == null)
            {
                OrderDetailsCart.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
            }
            else
            {
                OrderDetailsCart.OrderHeader.TransactionId = charge.BalanceTransactionId;
            }

            if (charge.Status.ToLower() == "succeeded")
            {
                OrderDetailsCart.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
                OrderDetailsCart.OrderHeader.Status = SD.StatusSubmitted;
            }
            else
            {
                OrderDetailsCart.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
            }*/

            await dbContext.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> AddCoupon()
        {
            if(OrderDetailsCart.OrderHeader.CouponCode == null)
            {
                OrderDetailsCart.OrderHeader.CouponCode = "";
            }
            HttpContext.Session.SetString(SD.ssCouponCode,OrderDetailsCart.OrderHeader.CouponCode);

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> RemoveCoupon()
        {
            HttpContext.Session.SetString(SD.ssCouponCode,string.Empty);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Plus(int cartId)
        {
            var cart =await dbContext.ShoppingCartTb.Where(u => u.Id == cartId).FirstOrDefaultAsync();
            cart.count++;
            await dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Minus(int cartId)
        {
            var cart = await dbContext.ShoppingCartTb.Where(u => u.Id == cartId).FirstOrDefaultAsync();
            
            if(cart.count == 1)
            {
                dbContext.Remove(cart);
                await dbContext.SaveChangesAsync();
                var cartList = await dbContext.ShoppingCartTb.ToListAsync();
                var cnt = cartList.Count();
                HttpContext.Session.SetInt32("ssCartCount", cnt);
            }
            else
            {
                cart.count--;
                await dbContext.SaveChangesAsync();
            }
          

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(int cartId)
        {
            var cart = await dbContext.ShoppingCartTb.Where(u => u.Id == cartId).FirstOrDefaultAsync();
            dbContext.Remove(cart);
            await dbContext.SaveChangesAsync();
            var cartList = await dbContext.ShoppingCartTb.ToListAsync();
            var cnt = cartList.Count();
            HttpContext.Session.SetInt32("ssCartCount", cnt);
            
            return RedirectToAction(nameof(Index));
        }

    }
}
