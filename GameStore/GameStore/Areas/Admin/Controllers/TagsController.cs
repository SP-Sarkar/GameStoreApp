using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Areas.Admin.Models.ViewModels;
using GameStore.Data;
using GameStore.Data.Entities;
using GameStore.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace GameStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TagsController : Controller
    {
        private readonly AppDbContext _db;

        public TagsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [Route(template: "")]
        public IActionResult Index(string active = null)
        {
            TagListViewModel model = new TagListViewModel();

            // getting the queryString
            model.QueryString = Request.Query[nameof(active)];
            model.Tags = null;

            // fetching tags based on querystring.
            // if queryString is active then All active tags will be displayed
            // if queryString is nonactive then all non active querystring will be displayed.
            // else all the tags will be displayed.
            if (model.QueryString != null)
            {
                if (string.Compare(model.QueryString, "active", StringComparison.Ordinal) == 0)
                {
                    model.Tags = _db.Tags.Where(t => t.IsDeleted == false).ToList();
                    model.Title = "Active Tags";
                }
                else if (string.Compare(model.QueryString, "notactive", StringComparison.Ordinal) == 0)
                {
                    model.Tags = _db.Tags.Where(t => t.IsDeleted == true).ToList();
                    model.Title = "All Deleted Tags";
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                model.Tags = _db.Tags.ToList();
                model.Title = "All Tags";
            }
            return View(model);
        }

        [HttpGet]
        [Route(template: "create-tag")]
        public IActionResult CreateTag()
        {
            return View();
        }

        [HttpPost]
        [Route("create-tag-post")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTagPost(TagChangeViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("","Model Validation fails");
                    return View(nameof(CreateTag), model);
                };
                Tag tag = new Tag()
                {
                    Name = model.Name,
                    CTime = DateTime.Now,
                    UTime = DateTime.Now,
                    GuidValue = Guid.NewGuid(),
                    IsDeleted = false,
                    Slug = model.Name.ToSlug()
                };

                _db.Tags.Add(tag);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { slug = tag.Slug, guid = tag.GuidValue.ToString() });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }

        // GET:details
        [HttpGet]
        [Route("details/{slug}/{guid}")]
        public async Task<IActionResult> Details(string slug, string guid)
        {
            Tag tag = null;
            if (guid == null) return NotFound();
            if (!Guid.TryParse(guid, out Guid parsedGuid)) return NotFound();
            tag = await _db.Tags.FirstOrDefaultAsync(t => t.GuidValue == parsedGuid);

            if (tag != null) return View(tag);
            return NotFound();
        }


        //GET:edit
        [HttpGet]
        [Route(template: "edit-tag/{slug}/{guid}")]
        public async Task<IActionResult> EditTag(string slug, string guid)
        {
            Tag tag = null;
            if (guid == null) return NotFound();
            if (!Guid.TryParse(guid, out Guid parsedGuid)) return NotFound();
            else
            {
                tag = await _db.Tags.FirstOrDefaultAsync(t => t.GuidValue == parsedGuid);
            }

            if (tag == null) return NotFound();
            var model = new TagChangeViewModel()
            {
                Name = tag.Name,
                IsDeleted = tag.IsDeleted,
                GuidValue = tag.GuidValue.ToString()
            };
            return View(model);
        }

        //Post:Edit
        [HttpPost]
        [Route("edit-tag-post")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTagPost(TagChangeViewModel model)
        {
            try
            {
                if (!ModelState.IsValid) return View(nameof(EditTag), model);
                if (!Guid.TryParse(model.GuidValue, out Guid parsedGuid))
                {
                    ModelState.AddModelError("","Validation key MisMatch");
                    return View(nameof(EditTag), model);
                }
                var oldTag = await _db.Tags.FirstOrDefaultAsync(t => t.GuidValue == parsedGuid);

                oldTag.UTime = DateTime.Now;
                oldTag.Name = model.Name;
                oldTag.Slug = model.Name.ToSlug();
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Details), 
                    new { slug = oldTag.Slug, guid = oldTag.GuidValue.ToString()});
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }

        //GET:Delete
        [HttpGet]
        [Route("delete-tag/{guid}")]
        public IActionResult Delete(string guid)
        {
            Tag tag = null;
            if (guid == null) return NotFound();
            if (!Guid.TryParse(guid, out Guid parsedGuid)) return NotFound();
            else
            {
                tag = _db.Tags.FirstOrDefault(t => t.GuidValue == parsedGuid);
            }

            if (tag != null) return View(tag);
            return NotFound();
        }

        //Post:Delete
        [HttpPost]
        [Route("delete-tag-permanently")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTagPostPermanently(string guidValue)
        {
            try
            {
                if (guidValue == null) return NotFound();
                if (Guid.TryParse(guidValue, out Guid parsedGuid))
                {
                    Tag tag = await _db.Tags.FirstOrDefaultAsync(t => t.GuidValue == parsedGuid);
                    if (tag == null) return NotFound();

                    var games = await _db.Games.Where(t => t.TagId == tag.Id).ToListAsync();
                    if (games != null)
                    {
                        foreach (Game game in games)
                        {
                            game.TagId = 6;
                        }
                    }

                    _db.Tags.Remove(tag);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), new { active = "notactive" });
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
        [Route(template: "delete-tag-post")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTagPost(string guidValue)
        {
            try
            {
                if (guidValue == null) return NotFound();
                if (Guid.TryParse(guidValue, out Guid parsedGuid))
                {
                    Tag tag = await _db.Tags.FirstOrDefaultAsync(t => t.GuidValue == parsedGuid);
                    if (tag == null) return NotFound();
                    tag.IsDeleted = true;
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), new { active = "notactive" });
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
        [Route(template:"activate-tag")]
        public async Task<IActionResult> ActivateTag(string guidValue)
        {
            try
            {
                if (guidValue == null) return NotFound();
                if (!Guid.TryParse(guidValue, out Guid gValue)) return NotFound();
                var oldTag = await _db.Tags.FirstOrDefaultAsync(t => t.GuidValue == gValue);
                oldTag.IsDeleted = false;
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new {active = "active"});
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }
    }
}