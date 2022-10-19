using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    [Authorize(Roles = SD.ManagerUser)]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext context;
        public SubCategoryController(ApplicationDbContext context)
        {
            this.context = context;
        }
        // GET: SubCategoryController
        public async Task<IActionResult> Index()
        {
            return View(await context.SubCategoryTb.Include(s => s.Category).ToListAsync());
        }


        // GET: SubCategoryController/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.CategoryId = new SelectList(context.CategoryTb.OrderBy(x => x.Id), "Id", "Name");
            return View();
        }

        // POST: SubCategoryController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategory collection)
        {


            // return Content(collection.CategoryId + collection.Name);

            await context.SubCategoryTb.AddAsync(collection);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));



        }

        // GET: SubCategoryController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.CategoryId = new SelectList(context.CategoryTb.OrderBy(x => x.Id), "Id", "Name");
            if (id == null)
            {
                return NotFound();
            }
            var category = await context.SubCategoryTb.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: SubCategoryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SubCategory collection)
        {
            try
            {
                context.Update(collection);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: SubCategoryController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var category = await context.SubCategoryTb.Include(p => p.Category).FirstAsync(q => q.Id == id);

            return View(category);
        }

        // POST: SubCategoryController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(SubCategory collection)
        {
            try
            {
                // return Content(collection.CategoryId + "   " + collection.Name);
                context.RemoveRange(collection);
                context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subCategory = await context.SubCategoryTb.Include(p => p.Category).FirstAsync(q => q.Id == id);
            if (subCategory == null)
            {
                return NotFound();
            }
            return View(subCategory);
        }

        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategories(int Id)
        {
            List<SubCategory> subCategories = new List<SubCategory>();
            subCategories = await (from subCategory in context.SubCategoryTb
                                   where subCategory.CategoryId == Id
                                   select subCategory).ToListAsync();
            return Json(new SelectList(subCategories,"Id","Name"));
        }
    }
}