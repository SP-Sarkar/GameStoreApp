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
            IQueryable<Game> games = null;
            if(model.QueryString != null) 
            { 
                if(string.Equals(model.QueryString,"active"))
                {
                    games = _db.Games.Where(g => g.IsDeleted == false).Include(t=>t.Tag).Include(gd=>gd.GameDeveloper);
                    model.Title = "All Active Games";
                }
                else if (string.Equals(model.QueryString, "notactive"))
                {
                    games =  _db.Games.Where(g => g.IsDeleted == true).Include(t => t.Tag).Include(gd => gd.GameDeveloper);
                    model.Title = "All Deactivate Games";
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                games = _db.Games.Include(t => t.Tag).Include(gd => gd.GameDeveloper);
                model.Title = "All Games";
            }

            model.Games = await games.ToListAsync();
            return View(model);
        }

        [HttpGet]
        [Route("create-game")]
        public IActionResult CreateGame()
        {
            IEnumerable<Tag> tags = _db.Tags
                .Where(t=>t.IsDeleted==false)
                .ToList();
            IEnumerable<GameDeveloper> gameDevelopers = _db.GameDevelopers
                .Where(gd => gd.IsDeleted == false)
                .ToList();
            IEnumerable<SelectListItem> gameCategories = _db.Categories
                .Where(c => c.IsDeleted == false)
                .Select(c => new SelectListItem() {Text = c.Name, Value = c.Id.ToString()})
                .ToList();
            GameChangeViewModel model = new GameChangeViewModel()
            {
                Title = "Create New Game",
                TagList = new SelectList(tags, "Id","Name"),
                GameDeveloperList = new SelectList(gameDevelopers, "Id", "Name"),
                GameCategoryList = gameCategories
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

                //way to select many result based on multiple id [unsorted]
                var category = _db.Categories.Where(c => model.GameCategoryId.Contains(c.Id)).ToList();

                // Another way to select many result based on multiple id [Sorted]
                // var anotherCategory = _db.Categories.Join(model.GameCategoryId, c => c.Id, s => s, ((c, s) => c)).ToList();

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
                    TagId = model.TagId ,
                    GameDeveloperId = model.GameDeveloperId,
                    IsDeleted = false
                };

                foreach (Category cat in category)
                {
                    game.GameCategories.Add(new GameCategory()
                    {
                        Category = cat,
                        Game = game
                    });
                }

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
                var gameInDb = await _db.Games.Include(t=>t.Tag).Include(gd=>gd.GameDeveloper).FirstOrDefaultAsync(g => g.GuidValue == parsedGuid);
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
                    GuidValue = gameInDb.GuidValue.ToString(),
                    TagList = new SelectList(_db.Tags.Where(t => t.IsDeleted == false).ToList(), "Id", "Name"),
                    TagId = gameInDb.TagId,
                    GameDeveloperList = new SelectList(_db.GameDevelopers.Where(gd =>gd.IsDeleted == false).ToList(), "Id", "Name"),
                    GameDeveloperId = gameInDb.GameDeveloperId
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
                gameInDb.TagId = model.TagId;
                gameInDb.GameDeveloperId = model.GameDeveloperId;

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