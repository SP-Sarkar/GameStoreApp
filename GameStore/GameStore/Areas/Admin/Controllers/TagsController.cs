using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Areas.Admin.Models.ViewModels;
using GameStore.Data;
using GameStore.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [Route(template: "create-tag")]
        public IActionResult CreateTag()
        {
            return View();
        }

        [HttpPost]
        [Route("create-tag-post")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTagPost(Tag tag)
        {
            try
            {
                if (!ModelState.IsValid) return View("CreateTag", tag);
                tag.CTime = DateTime.Now;
                tag.UTime = DateTime.Now;
                tag.GuidValue = Guid.NewGuid();
                tag.IsDeleted = false;
                _db.Tags.Add(tag);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { name = tag.Name, guid = tag.GuidValue.ToString() });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }

        // GET:details
        [HttpGet]
        [Route("details/{name}/{guid}")]
        public async Task<IActionResult> Details(string name, string guid)
        {
            Tag tag = null;
            if (guid == null) return NotFound();
            if (!Guid.TryParse(guid, out Guid parsedGuid)) return NotFound();
            else
            {
                tag = await _db.Tags.FirstOrDefaultAsync(t => t.GuidValue == parsedGuid);
            }

            if (tag != null) return View(tag);
            return NotFound();
        }


    }
}