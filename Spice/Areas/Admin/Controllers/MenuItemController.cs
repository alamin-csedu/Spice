using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models.ViewModels;
using Spice.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ManagerUser)]
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
            return View(await context.MenuItemTb.Include(m => m.Category).Include(p => p.SubCategory).ToListAsync());
        }

        // GET: MenuItemController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            MenuItemVM.MenuItem = await context.MenuItemTb.Include(p => p.Category).Include(s => s.SubCategory).FirstAsync(q => q.Id == id);
            return View(MenuItemVM);
        }

        // GET: MenuItemController/Create
        public ActionResult Create()
        {
            return View(MenuItemVM);
        }

        // POST: MenuItemController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async  Task<ActionResult> Create(int i)
        {
            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());
           await context.MenuItemTb.AddAsync(MenuItemVM.MenuItem);
           await context.SaveChangesAsync();

            string webRootPath = hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var menuItemFromDb = await context.MenuItemTb.FindAsync(MenuItemVM.MenuItem.Id);

            if (files.Count() > 0)
            {
                var uploads = Path.Combine(webRootPath, "images");
                var extension = Path.GetExtension(files[0].FileName);

                using (var fileStream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }
                menuItemFromDb.Image = @"/images/" + MenuItemVM.MenuItem.Id + extension;


            }
            else
            {
                var uploads = Path.Combine(webRootPath, @"/images/"+SD.DefaultFoodImage);
                System.IO.File.Copy(@"/images/" + MenuItemVM.MenuItem.Id + ".png", uploads);
                menuItemFromDb.Image = @"/images/" + MenuItemVM.MenuItem.Id + ".png";

            }

           await context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
            
        }

        // GET: MenuItemController/Edit/5
        public async  Task<ActionResult> Edit(int id)
        {
            MenuItemVM.MenuItem = await context.MenuItemTb.Include(m => m.Category).Include(p => p.SubCategory).SingleOrDefaultAsync(c => c.Id== id);
          
            if (id == null)
            {
                return NotFound();
            }
          
            return View(MenuItemVM);
        }

        // POST: MenuItemController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
        {
            if (id == null)
            {
                return NotFound();
            }
            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (!ModelState.IsValid)
            {
                MenuItemVM.SubCategories = await context.SubCategoryTb.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();
                return View(MenuItemVM);
            }

            //Work on the image saving section

            string webRootPath = hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var menuItemFromDb = await context.MenuItemTb.FindAsync(MenuItemVM.MenuItem.Id);

            if (files.Count > 0)
            {
                //New Image has been uploaded
                var uploads = Path.Combine(webRootPath, "images");
                var extension_new = Path.GetExtension(files[0].FileName);

                //Delete the original file
                var imagePath = Path.Combine(webRootPath, menuItemFromDb.Image.TrimStart('\\'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                //we will upload the new file
                using (var filesStream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension_new), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }
                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + extension_new;
            }

            menuItemFromDb.Name = MenuItemVM.MenuItem.Name;
            menuItemFromDb.Description = MenuItemVM.MenuItem.Description;
            menuItemFromDb.Price = MenuItemVM.MenuItem.Price;
            menuItemFromDb.Spicyness = MenuItemVM.MenuItem.Spicyness;
            menuItemFromDb.CategoryId = MenuItemVM.MenuItem.CategoryId;
            menuItemFromDb.SubCategoryId = MenuItemVM.MenuItem.SubCategoryId;

            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: MenuItemController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            MenuItemVM.MenuItem  = await context.MenuItemTb.Include(p => p.Category).Include(s => s.SubCategory).FirstAsync(q => q.Id == id);
            return View(MenuItemVM.MenuItem);
        }

        // POST: MenuItemController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            string webRootPath = hostingEnvironment.WebRootPath;
            var menuItemFromDb = await context.MenuItemTb.FindAsync(id);
            var imageURL = menuItemFromDb.Image;
            if(imageURL != null)
            {
                var imagePath = webRootPath + imageURL;
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }


            context.MenuItemTb.RemoveRange(menuItemFromDb);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
    }
}
