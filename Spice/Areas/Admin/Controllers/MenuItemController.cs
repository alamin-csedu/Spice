using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MenuItemController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IHostingEnvironment hostingEnvironment;

        [BindProperty]
        public MenuItemViewModel MenuItemVM { get; set; }
        public MenuItemController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment)
        {
            this.context = context;
            this.hostingEnvironment = hostingEnvironment;
            MenuItemVM = new MenuItemViewModel()
            {
                Categories = context.CategoryTb,
                MenuItem = new Models.MenuItem()
            };

        }


        // GET: MenuItemController
        public async Task<IActionResult> Index()
        {
            List<String> input = new List<string>(){
                "Alamin", "Tamim", "Abir", "Jewel", "Monir" };
            ViewBag.input = input;
           // var menuItem = context.MenuItemTb.Include(m => m.Category).Include(p => p.SubCategory).ToListAsync();
            return View(await context.MenuItemTb.Include(m => m.Category).Include(p => p.SubCategory).ToListAsync());
        }

        // GET: MenuItemController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: MenuItemController/Create
        public ActionResult Create()
        {
            return View(MenuItemVM);
        }

        // POST: MenuItemController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: MenuItemController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: MenuItemController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: MenuItemController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: MenuItemController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
