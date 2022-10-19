using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =  SD.ManagerUser)]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext context;
        public CategoryController(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await context.CategoryTb.ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] Category categoryObj)
        {
            context.CategoryTb.Add(categoryObj);
            context.SaveChanges();
            return RedirectToAction("Index","Category");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category = await context.CategoryTb.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);

        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            context.Update(category);
            context.SaveChanges();
            return RedirectToAction("Index", "Category");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            Category category = await context.CategoryTb.FindAsync(id);
            if(category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Category category)
        {
            context.Remove(category);
            await context.SaveChangesAsync();
            return RedirectToAction("Index", "Category");
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Category category = await context.CategoryTb.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

       
    }
}
