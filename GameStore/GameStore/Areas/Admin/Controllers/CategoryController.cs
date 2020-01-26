using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Areas.Admin.Models.ViewModels;
using GameStore.Data;
using GameStore.Data.Entities;
using GameStore.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("category")]
    public class CategoryController : Controller
    {
        private AppDbContext _db;

        public CategoryController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index(string active)
        {
            CategoryListViewModel model = new CategoryListViewModel();

            // getting the queryString
            model.QueryString = Request.Query[nameof(active)];
            model.Categories = null;

            if (model.QueryString != null)
            {
                if (string.Compare(model.QueryString, "active", StringComparison.Ordinal) == 0)
                {
                    model.Categories = await _db.Categories.Where(t => t.IsDeleted == false).ToListAsync();
                    model.Title = "Active Categories";
                }
                else if (string.Compare(model.QueryString, "notactive", StringComparison.Ordinal) == 0)
                {
                    model.Categories = await _db.Categories.Where(t => t.IsDeleted == true).ToListAsync();
                    model.Title = "All Deleted Categories";
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                model.Categories = await _db.Categories.ToListAsync();
                model.Title = "All Categories";
            }
            return View(model);
        }

        [HttpGet]
        [Route("create-category")]
        public IActionResult CreateCategory() => View();


    }
}