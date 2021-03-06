﻿using System;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("create-category-post")]
        public async Task<IActionResult> CreateCategoryPost(CategoryChangeViewModel model)
        {
            try
            {
                if (!ModelState.IsValid) return View(nameof(CreateCategory), model);
                var category = new Category()
                {
                    Name = model.Name,
                    Description = model.Description,
                    CTime = DateTime.Now,
                    UTime = DateTime.Now,
                    GuidValue = Guid.NewGuid(),
                    IsDeleted = false,
                    Slug = model.Name.ToSlug()
                };
                _db.Categories.Add(category);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Details),
                    new {slug = category.Slug, guid = category.GuidValue.ToString()});
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ModelState.AddModelError("","Creation of new category is not successful");
                return View(nameof(CreateCategory), model);
            }
        }

        [HttpGet]
        [Route("details/{slug}/{guid}")]
        public async Task<IActionResult> Details(string slug, string guid)
        {
            try
            {
                if (guid == null) return NotFound();
                if (!Guid.TryParse(guid, out Guid parsedGuid)) return NotFound();
                var category = await _db.Categories.FirstOrDefaultAsync(c => c.GuidValue == parsedGuid);
                if (category != null && string.Equals(slug, category.Slug))
                    return View(category);
                return NotFound();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        [HttpGet]
        [Route("edit-category/{slug}/{guid}")]
        public async Task<IActionResult> EditCategory(string slug, string guid)
        {
            try
            {
                if (guid == null) return NotFound();
                if (!Guid.TryParse(guid, out Guid parsedGuid)) return NotFound();
                var category = await _db.Categories.FirstOrDefaultAsync(c => c.GuidValue == parsedGuid);
                if (category != null && string.Equals(slug, category.Slug))
                {
                    CategoryChangeViewModel model = new CategoryChangeViewModel()
                    {
                        Name = category.Name,
                        Description = category.Description,
                        GuidValue = category.GuidValue.ToString()
                    };
                    return View(model);
                }
                return NotFound();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("update-category")]
        public async Task<IActionResult> EditCategoryPost(CategoryChangeViewModel model)
        {
            try
            {
                if (!ModelState.IsValid) return View(nameof(EditCategory), model);
                if (!Guid.TryParse(model.GuidValue, out Guid parsedGuid))
                {
                    ModelState.AddModelError("","unique Key is not set");
                    return View(nameof(EditCategory), model);
                }

                var oldCategory = await _db.Categories.FirstOrDefaultAsync(c => c.GuidValue == parsedGuid);
                if (oldCategory != null)
                {
                    oldCategory.Name = model.Name;
                    oldCategory.Description = model.Description;
                    oldCategory.UTime = DateTime.Now;
                    oldCategory.Slug = model.Name.ToSlug();
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Details),
                        new {slug = oldCategory.Slug, guid = oldCategory.GuidValue.ToString()});
                }
                ModelState.AddModelError("", "Can not be updated. Create One.");
                return View(nameof(EditCategory), model);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ModelState.AddModelError("", "Exception Happens");
                return View(nameof(EditCategory), model);
            }
        }

        [HttpGet]
        [Route("delete/{guid}")]
        public async Task<IActionResult> Delete(string guid)
        {
            try
            {
                if (guid == null) return NotFound();
                if (!Guid.TryParse(guid, out Guid parsedGuid)) return NotFound();
                var category = await _db.Categories.FirstOrDefaultAsync(c => c.GuidValue == parsedGuid);
                if (category == null) return NotFound();
                CategoryChangeViewModel model = new CategoryChangeViewModel()
                {
                    Name = category.Name,
                    GuidValue = category.GuidValue.ToString(),
                    IsDeleted = category.IsDeleted
                };
                return View(model);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("deactivate-category")]
        public async Task<IActionResult> DeleteCategoryPost(string guidValue)
        {
            try
            {
                if (guidValue == null) return BadRequest();
                if (!Guid.TryParse(guidValue, out Guid parsedGuid)) return BadRequest();
                var category = await _db.Categories.FirstOrDefaultAsync(c => c.GuidValue == parsedGuid);
                if (category == null) return NotFound();
                category.IsDeleted = true;
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { active = "active" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete-category-permanently")]
        public async Task<IActionResult> DeleteCategoryPostPermanently(string guidValue)
        {
            try
            {
                if (guidValue == null) return BadRequest();
                if (!Guid.TryParse(guidValue, out Guid parsedGuid)) return BadRequest();
                var category = await _db.Categories.FirstOrDefaultAsync(c => c.GuidValue == parsedGuid);
                if (category == null) return NotFound();
                _db.Categories.Remove(category);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { active = "notactive" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("")]
        public async Task<IActionResult> ActivateCategory(string guidValue)
        {
            try
            {
                if (guidValue == null) return BadRequest();
                if (!Guid.TryParse(guidValue, out Guid parsedGuid)) return BadRequest();
                var category = await _db.Categories.FirstOrDefaultAsync(c => c.GuidValue == parsedGuid);
                if (category == null) return NotFound();
                category.IsDeleted = false;
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { active = "active" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }


    }
}