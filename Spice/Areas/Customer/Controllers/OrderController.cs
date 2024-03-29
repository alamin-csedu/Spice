﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Utility;

namespace Spice.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext dbContext;
        private int PageSize = 2;
        public OrderController(ApplicationDbContext db, IEmailSender emailSender)
        {
            dbContext = db;
            _emailSender = emailSender;
        }


        [Authorize]
        public async Task<IActionResult> Confirm(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderDetailsView orderDetailsViewModel = new OrderDetailsView()
            {
                OrderHeader = await dbContext.OrderHeaderTb.Include(o => o.ApplicationUser).FirstOrDefaultAsync(o => o.Id == id && o.UserId == claim.Value),
                OrderDetailsList = await dbContext.OrderDetailsTb.Where(o => o.OrderId == id).ToListAsync()
            };

            return View(orderDetailsViewModel);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetOrderStatus(int Id)
        {
            return PartialView("_OrderStatus", dbContext.OrderHeaderTb.Where(m => m.Id == Id).FirstOrDefault().Status);

        }

      
        [Authorize]
        public async Task<IActionResult> OrderHistory()
        {
            var claimItdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimItdentity.FindFirst(ClaimTypes.NameIdentifier);

            List<OrderHeader> OrderHeaderList = await dbContext.OrderHeaderTb.Include(u => u.ApplicationUser).Where(p => p.UserId == claim.Value).ToListAsync();

            List<OrderDetailsView> orderList = new List<OrderDetailsView>();

            foreach(OrderHeader orderHeader in OrderHeaderList)
            {
                OrderDetailsView orderDetailsView = new OrderDetailsView()
                {
                    OrderHeader = orderHeader,
                    OrderDetailsList = await dbContext.OrderDetailsTb.Where(p => p.OrderId == orderHeader.Id).ToListAsync()
                };
                 orderList.Add(orderDetailsView);
            }

            return View(orderList);
        }

        public async Task<IActionResult> GetOrderDetails(int Id)
        {
            OrderDetailsView orderDetailsViewModel = new OrderDetailsView()
            {
                OrderHeader = await dbContext.OrderHeaderTb.Include(el => el.ApplicationUser).FirstOrDefaultAsync(m => m.Id == Id),
                OrderDetailsList = await dbContext.OrderDetailsTb.Where(m => m.OrderId == Id).ToListAsync()
            };
            orderDetailsViewModel.OrderHeader.ApplicationUser = await dbContext.ApplicationUserTb.FirstOrDefaultAsync(u => u.Id == orderDetailsViewModel.OrderHeader.UserId);

            return PartialView("_IndividualOrderDetails", orderDetailsViewModel);
        }

       
        [Authorize(Roles = SD.KitchenUser + "," + SD.ManagerUser)]
         public async Task<IActionResult> ManageOrder(int productPage = 1)
          {

          List<OrderDetailsView> orderDetailsVM = new List<OrderDetailsView>();

          List<OrderHeader> OrderHeaderList = await dbContext.OrderHeaderTb.Where(o => o.Status == SD.StatusSubmitted || o.Status == SD.StatusInProcess).OrderByDescending(u => u.PickupTime).ToListAsync();


          foreach (OrderHeader item in OrderHeaderList)
           {
           OrderDetailsView individual = new OrderDetailsView() { 

            OrderHeader = item,
            OrderDetailsList = await dbContext.OrderDetailsTb.Where(o => o.OrderId == item.Id).ToListAsync()
            };
            orderDetailsVM.Add(individual);
            }



            return View(orderDetailsVM.OrderBy(o => o.OrderHeader.PickupTime).ToList());
         }





        

               [Authorize(Roles = SD.KitchenUser + "," + SD.ManagerUser)]
               public async Task<IActionResult> OrderPrepare(int OrderId)
               {
                   OrderHeader orderHeader = await dbContext.OrderHeaderTb.FindAsync(OrderId);
                   orderHeader.Status = SD.StatusInProcess;
                   await dbContext.SaveChangesAsync();
                   return RedirectToAction("ManageOrder", "Order");
               }


               [Authorize(Roles = SD.KitchenUser + "," + SD.ManagerUser)]
               public async Task<IActionResult> OrderReady(int OrderId)
               {
                   OrderHeader orderHeader = await dbContext.OrderHeaderTb.FindAsync(OrderId);
                   orderHeader.Status = SD.StatusReady;
                   await dbContext.SaveChangesAsync();

                   //Email logic to notify user that order is ready for pickup
                  // await _emailSender.SendEmailAsync(dbContext.Users.Where(u => u.Id == orderHeader.UserId).FirstOrDefault().Email, "Spice - Order Ready for Pickup " + orderHeader.Id.ToString(), "Order is ready for pickup.");


                   return RedirectToAction("ManageOrder", "Order");
               }


               [Authorize(Roles = SD.KitchenUser + "," + SD.ManagerUser)]
               public async Task<IActionResult> OrderCancel(int OrderId)
               {
                   OrderHeader orderHeader = await dbContext.OrderHeaderTb.FindAsync(OrderId);
                   orderHeader.Status = SD.StatusCancelled;
                   await dbContext.SaveChangesAsync();
                   //await _emailSender.SendEmailAsync(dbContext.Users.Where(u => u.Id == orderHeader.UserId).FirstOrDefault().Email, "Spice - Order Cancelled " + orderHeader.Id.ToString(), "Order has been cancelled successfully.");

                   return RedirectToAction("ManageOrder", "Order");
               }


        /*
               [Authorize]
               public async Task<IActionResult> OrderPickup(int productPage = 1, string searchEmail = null, string searchPhone = null, string searchName = null)
               {
                   //var claimsIdentity = (ClaimsIdentity)User.Identity;
                   //var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                   OrderListViewModel orderListVM = new OrderListViewModel()
                   {
                       Orders = new List<OrderDetailsViewModel>()
                   };

                   StringBuilder param = new StringBuilder();
                   param.Append("/Customer/Order/OrderPickup?productPage=:");
                   param.Append("&searchName=");
                   if (searchName != null)
                   {
                       param.Append(searchName);
                   }
                   param.Append("&searchEmail=");
                   if (searchEmail != null)
                   {
                       param.Append(searchEmail);
                   }
                   param.Append("&searchPhone=");
                   if (searchPhone != null)
                   {
                       param.Append(searchPhone);
                   }

                   List<OrderHeader> OrderHeaderList = new List<OrderHeader>();
                   if (searchName != null || searchEmail != null || searchPhone != null)
                   {
                       var user = new ApplicationUser();

                       if (searchName != null)
                       {
                           OrderHeaderList = await _db.OrderHeader.Include(o => o.ApplicationUser)
                                                       .Where(u => u.PickupName.ToLower().Contains(searchName.ToLower()))
                                                       .OrderByDescending(o => o.OrderDate).ToListAsync();
                       }
                       else
                       {
                           if (searchEmail != null)
                           {
                               user = await _db.ApplicationUser.Where(u => u.Email.ToLower().Contains(searchEmail.ToLower())).FirstOrDefaultAsync();
                               OrderHeaderList = await _db.OrderHeader.Include(o => o.ApplicationUser)
                                                           .Where(o => o.UserId == user.Id)
                                                           .OrderByDescending(o => o.OrderDate).ToListAsync();
                           }
                           else
                           {
                               if (searchPhone != null)
                               {
                                   OrderHeaderList = await _db.OrderHeader.Include(o => o.ApplicationUser)
                                                               .Where(u => u.PhoneNumber.Contains(searchPhone))
                                                               .OrderByDescending(o => o.OrderDate).ToListAsync();
                               }
                           }
                       }
                   }
                   else
                   {
                       OrderHeaderList = await _db.OrderHeader.Include(o => o.ApplicationUser).Where(u => u.Status == SD.StatusReady).ToListAsync();
                   }

                   foreach (OrderHeader item in OrderHeaderList)
                   {
                       OrderDetailsViewModel individual = new OrderDetailsViewModel
                       {
                           OrderHeader = item,
                           OrderDetails = await _db.OrderDetails.Where(o => o.OrderId == item.Id).ToListAsync()
                       };
                       orderListVM.Orders.Add(individual);
                   }



                   var count = orderListVM.Orders.Count;
                   orderListVM.Orders = orderListVM.Orders.OrderByDescending(p => p.OrderHeader.Id)
                                        .Skip((productPage - 1) * PageSize)
                                        .Take(PageSize).ToList();

                   orderListVM.PagingInfo = new PagingInfo
                   {
                       CurrentPage = productPage,
                       ItemsPerPage = PageSize,
                       TotalItem = count,
                       urlParam = param.ToString()
                   };

                   return View(orderListVM);
               }

               [Authorize(Roles = SD.FrontDeskUser + "," + SD.ManagerUser)]
               [HttpPost]
               [ActionName("OrderPickup")]
               public async Task<IActionResult> OrderPickupPost(int orderId)
               {
                   OrderHeader orderHeader = await _db.OrderHeader.FindAsync(orderId);
                   orderHeader.Status = SD.StatusCompleted;
                   await _db.SaveChangesAsync();
                   await _emailSender.SendEmailAsync(_db.Users.Where(u => u.Id == orderHeader.UserId).FirstOrDefault().Email, "Spice - Order Completed " + orderHeader.Id.ToString(), "Order has been completed successfully.");

                   return RedirectToAction("OrderPickup", "Order");
               }*/
    }
}