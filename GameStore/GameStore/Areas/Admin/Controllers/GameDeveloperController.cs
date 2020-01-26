using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Areas.Admin.Models.ViewModels;
using GameStore.Data;
using GameStore.Data.Entities;
using GameStore.Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("game-dev")]
    public class GameDeveloperController : Controller
    {
        private AppDbContext _db;
        private IHostingEnvironment _env;
        public GameDeveloperController(AppDbContext db, IHostingEnvironment env)
        {
            _db = db;
            _env = env;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index(string active = null)
        {
            GameDeveloperListViewModel model = new GameDeveloperListViewModel();

            // getting the queryString
            model.QueryString = Request.Query[nameof(active)];
            model.GameDevelopers = null;

            if (model.QueryString != null)
            {
                if (string.Compare(model.QueryString, "active", StringComparison.Ordinal) == 0)
                {
                    model.GameDevelopers = await _db.GameDevelopers.Where(t => t.IsDeleted == false).ToListAsync();
                    model.Title = "Active Gaming Companies";
                }
                else if (string.Compare(model.QueryString, "notactive", StringComparison.Ordinal) == 0)
                {
                    model.GameDevelopers = await _db.GameDevelopers.Where(t => t.IsDeleted == true).ToListAsync();
                    model.Title = "All Deleted Gaming Companies";
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                model.GameDevelopers = await _db.GameDevelopers.ToListAsync();
                model.Title = "All Gaming Companies";
            }
            return View(model);
        }


        [HttpGet]
        [Route("create-game-company")]
        public IActionResult CreateGameDevelopers() => View();


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("create-company-post")]
        public async Task<IActionResult> CreateGameDeveloperPost(GameDeveloperChangeViewModel model)
        {
            // TODO: this method is working but If image is uploaded but unable to add info to the database. 

            try
            {
                if (!ModelState.IsValid) return View(nameof(CreateGameDevelopers), model);
                GameDeveloper developer = new GameDeveloper();
                if (model.Logo != null)
                {
                    if (model.Logo.IsImageIsValidContentType() && model.Logo.IsImageHasValidExtension() &&
                        model.Logo.IsImageHasValidSize(512) && model.Logo.IsImageCsrfFree(512) &&
                        model.Logo.IsImageByteReadable())
                    {
                        var imagePath = Path.Combine(_env.WebRootPath, "uploads", model.Logo.FileName);
                        var stream = new FileStream(imagePath, FileMode.Create);
                        await model.Logo.CopyToAsync(stream);
                        developer.Logo = Path.Combine("uploads", model.Logo.FileName);
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(model.Logo), "Image is not valid type or can not be readable.");
                        return View(nameof(CreateGameDevelopers), model);
                    }
                }
                else
                {
                    developer.Logo = Path.Combine("uploads", "noimage.jpg");
                }

                developer.Name = model.Name;
                developer.Description = model.Description;
                developer.WebUrl = model.WebUrl;
                developer.CTime = DateTime.Now;
                developer.UTime = DateTime.Now;
                developer.GuidValue =Guid.NewGuid();
                developer.IsDeleted = false;
                developer.Slug = model.Name.ToSlug();
                 _db.GameDevelopers.Add(developer);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new {active = "active"});
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ModelState.AddModelError("", e.InnerException.Message);
                return View(nameof(CreateGameDevelopers),model);
            }
        }


        [HttpGet]
        [Route("details/{slug}/{guid}")]
        public async Task<IActionResult> Details(string slug, string guid)
        {
            if (guid == null) return NotFound();
            if (!Guid.TryParse(guid, out Guid parsedGuid)) return NotFound();
            var gameDeveloper = await _db.GameDevelopers.FirstOrDefaultAsync(t => t.GuidValue == parsedGuid);
            if ((gameDeveloper != null) && (string.Equals(gameDeveloper.Slug, slug)))
                return View(gameDeveloper);
            return NotFound();
        }

        [HttpGet]
        [Route("edit-game-developer/{slug}/{guid}")]
        public async Task<IActionResult> EditGameDeveloper(string slug, string guid)
        {
            if (guid == null) return NotFound();
            if (!Guid.TryParse(guid, out Guid parsedGuid)) return NotFound();
            var gameDeveloper = await _db.GameDevelopers.FirstOrDefaultAsync(t => t.GuidValue == parsedGuid);
            if ((gameDeveloper == null) || (!string.Equals(gameDeveloper.Slug, slug))) return NotFound();
            var gameDeveloperModel = new GameDeveloperChangeViewModel()
            {
                Name = gameDeveloper.Name, Description = gameDeveloper.Description, WebUrl = gameDeveloper.WebUrl, OldLogo = gameDeveloper.Logo, GuidValue = gameDeveloper.GuidValue.ToString()
            };
            return View(gameDeveloperModel);

        }

        [HttpPost]
        [Route("update-game-developer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGameDeveloperPost(GameDeveloperChangeViewModel model)
        {
            try
            {
                if (!ModelState.IsValid) return View(nameof(EditGameDeveloper), model);
                if (!Guid.TryParse(model.GuidValue, out Guid parsedGuid))
                {
                    ModelState.AddModelError("", "Not Valid Data");
                    return View(nameof(EditGameDeveloper), model);
                }
                GameDeveloper developer = _db.GameDevelopers.FirstOrDefault(g => g.GuidValue == parsedGuid);

                if(developer==null)
                {
                    ModelState.AddModelError("", "the data does not exist in storage. Create one.");
                    return View(nameof(EditGameDeveloper), model);
                }

                if (model.Logo != null)
                {
                    if (model.Logo.IsImageIsValidContentType() && model.Logo.IsImageHasValidExtension() &&
                        model.Logo.IsImageHasValidSize(512) && model.Logo.IsImageCsrfFree(512) &&
                        model.Logo.IsImageByteReadable())
                    {
                        var imagePath = Path.Combine(_env.WebRootPath, "uploads", model.Logo.FileName);
                        var stream = new FileStream(imagePath, FileMode.Create);
                        await model.Logo.CopyToAsync(stream);
                        developer.Logo = Path.Combine("uploads", model.Logo.FileName);
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(model.Logo), "Image is not valid type or can not be readable.");
                        return View(nameof(CreateGameDevelopers), model);
                    }
                }
                

                developer.Name = model.Name;
                developer.Description = model.Description;
                developer.WebUrl = model.WebUrl;
                developer.UTime = DateTime.Now;
                developer.Slug = model.Name.ToSlug();
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { active = "active" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ModelState.AddModelError("", e.InnerException.Message);
                return View(nameof(EditGameDeveloper), model);
            }
        }


        [HttpGet]
        [Route("delete-company/{guid}")]
        public async Task<IActionResult> Delete(string guid)
        {
            if (guid == null) return NotFound();
            if (!Guid.TryParse(guid, out Guid parsedGuid)) return NotFound();
            var gameDeveloper = await _db.GameDevelopers.FirstOrDefaultAsync(t => t.GuidValue == parsedGuid);
            if (gameDeveloper != null)
                return View(gameDeveloper);
            return NotFound();
        }

        [HttpPost]
        [Route("deactivate-company")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGameDeveloperPost(string guidValue)
        {
            try
            {
                if (guidValue == null) return BadRequest();
                if (!Guid.TryParse(guidValue, out Guid parsedGuid)) return BadRequest();
                var gameDeveloper = await _db.GameDevelopers.FirstOrDefaultAsync(t => t.GuidValue == parsedGuid);
                if (gameDeveloper != null)
                {
                    gameDeveloper.IsDeleted = true;
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index),new{active = "active"});
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
        [Route("delete-company")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGameDeveloperPostPermanently(string guidValue)
        {
            try
            {
                if (guidValue == null) return BadRequest();
                if (!Guid.TryParse(guidValue, out Guid parsedGuid)) return BadRequest();
                var gameDeveloper = await _db.GameDevelopers.FirstOrDefaultAsync(t => t.GuidValue == parsedGuid);
                if (gameDeveloper != null)
                {
                    _db.GameDevelopers.Remove(gameDeveloper);
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
        [ValidateAntiForgeryToken]
        [Route("activate-company")]
        public async Task<IActionResult> ActivateGameDeveloper(string guidValue)
        {
            try
            {
                if (guidValue == null) return BadRequest();
                if (!Guid.TryParse(guidValue, out Guid parsedGuid)) return BadRequest();
                var gameDeveloper = await _db.GameDevelopers.FirstOrDefaultAsync(t => t.GuidValue == parsedGuid);
                if (gameDeveloper == null) return NotFound();
                gameDeveloper.IsDeleted = false;
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