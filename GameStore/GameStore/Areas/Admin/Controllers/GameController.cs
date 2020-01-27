using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Areas.Admin.Models.ViewModels;
using GameStore.Data;
using GameStore.Data.Entities;
using GameStore.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("product")]
    public class GameController : Controller
    {
        private AppDbContext _db;

        public GameController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index(string active)
        {
            var model = new GameListViewModel();

            model.QueryString = Request.Query["active"];
            model.Games = null;
            if(model.QueryString != null) 
            { 
                if(string.Equals(model.QueryString,"active"))
                {
                    model.Games =await _db.Games.Where(g => g.IsDeleted == false).Include(t=>t.Tag).ToListAsync();
                    model.Title = "All Active Games";
                }
                else if (string.Equals(model.QueryString, "notactive"))
                {
                    model.Games = await _db.Games.Where(g => g.IsDeleted == true).Include(t => t.Tag).ToListAsync();
                    model.Title = "All Deactivate Games";
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                model.Games = await _db.Games.Include(t => t.Tag).ToListAsync();
                model.Title = "All Games";
            }

            return View(model);
        }

        [HttpGet]
        [Route("create-game")]
        public IActionResult CreateGame()
        {
            GameChangeViewModel model = new GameChangeViewModel()
            {
                Title = "Create Neg Game"
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("create-game-post")]
        public async Task<IActionResult> CreateGamePost(GameChangeViewModel model)
        {
            try
            {
                if (!ModelState.IsValid) return View(nameof(CreateGame), model);
                Game game = new Game
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    WebUrl = model.WebUrl,
                    CTime = DateTime.Now,
                    UTime = DateTime.Now,
                    GuidValue = Guid.NewGuid(),
                    Slug = model.Name.ToSlug(),
                    TagId = 2,
                    GameDeveloperId = 2,
                    IsDeleted = false
                };
                _db.Games.Add(game);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new {slug = game.Slug, guid = game.GuidValue.ToString()});
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ModelState.AddModelError("", "Problem in Game Creation");
                return View(nameof(CreateGame), model);
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
                var gameInDb = await _db.Games.FirstOrDefaultAsync(g => g.GuidValue == parsedGuid);
                if (gameInDb == null) return NotFound();
                if (!string.Equals(gameInDb.Slug, slug)) return NotFound();
                return View(gameInDb);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        [HttpGet]
        [Route("edit-game/{slug}/{guid}")]
        public async Task<IActionResult> EditGame(string slug, string guid)
        {
            try
            {
                if (guid == null) return NotFound();
                if (!Guid.TryParse(guid, out Guid parsedGuid)) return NotFound();
                var gameInDb = await _db.Games.FirstOrDefaultAsync(g => g.GuidValue == parsedGuid);
                if (gameInDb == null) return NotFound();
                if (!string.Equals(gameInDb.Slug, slug)) return NotFound();
                var model = new GameChangeViewModel()
                {
                    Title = $"Edit {gameInDb.Name}",
                    Name = gameInDb.Name,
                    Description = gameInDb.Description,
                    Price = gameInDb.Price,
                    WebUrl = gameInDb.WebUrl,
                    GuidValue = gameInDb.GuidValue.ToString()
                };
                return View(model);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        [HttpPost]
        [Route("edit-game-post")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGamePost(GameChangeViewModel model)
        {
            try
            {
                if (!ModelState.IsValid) return View(nameof(EditGame), model);
                if(!Guid.TryParse(model.GuidValue, out Guid parsedGuid))
                {
                    ModelState.AddModelError("","Not Valid Data");
                    return View(nameof(EditGame), model);
                }
                var gameInDb = await _db.Games.FirstOrDefaultAsync(g => g.GuidValue == parsedGuid);
                if (gameInDb == null)
                {
                    ModelState.AddModelError("", "Unique Key is not Valid");
                    return View(nameof(EditGame), model);
                }

                gameInDb.Name = model.Name;
                gameInDb.Description = model.Description;
                gameInDb.Price = model.Price;
                gameInDb.WebUrl = model.WebUrl;
                gameInDb.UTime = DateTime.Now;
                gameInDb.Slug = model.Name.ToSlug();

                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Details),
                    new {slug = gameInDb.Slug, guid = gameInDb.GuidValue.ToString()});

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ModelState.AddModelError("", "Exception Happen");
                return View(nameof(EditGame), model);
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
                var gameInDb = await _db.Games.FirstOrDefaultAsync(g => g.GuidValue == parsedGuid);
                if (gameInDb == null) return NotFound();
                return View(gameInDb);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("deactivate-game")]
        public async Task<IActionResult> DeactivateGame(string guidValue)
        {
            try
            {
                if (guidValue == null) return BadRequest();
                if (!Guid.TryParse(guidValue, out Guid parsedGuid)) return BadRequest();
                var gameInDb = await _db.Games.FirstOrDefaultAsync(g => g.GuidValue == parsedGuid);
                if (gameInDb == null) return NotFound();
                gameInDb.IsDeleted = true;
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new {active = "active"});
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        [HttpPost]
        [Route("delete-game")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGamePost(string guidValue)
        {
            try
            {
                if (guidValue == null) return BadRequest();
                if (!Guid.TryParse(guidValue, out Guid parsedGuid)) return BadRequest();
                var gameInDb = await _db.Games.FirstOrDefaultAsync(g => g.GuidValue == parsedGuid);
                if (gameInDb == null) return NotFound();
                _db.Games.Remove(gameInDb);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { active = "notactive" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        // For Activating Games
        [HttpPost]
        [Route("activate-game")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateGame(string guidValue)
        {
            try
            {
                if (guidValue == null) return BadRequest();
                if (!Guid.TryParse(guidValue, out Guid parsedGuid)) return BadRequest();
                var gameInDb = await _db.Games.FirstOrDefaultAsync(g => g.GuidValue == parsedGuid);
                if (gameInDb == null) return NotFound();
                gameInDb.IsDeleted = false;
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