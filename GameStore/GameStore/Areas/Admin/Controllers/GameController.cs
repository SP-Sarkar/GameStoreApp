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
                    model.Games =await _db.Games.Where(g => g.IsDeleted == false).ToListAsync();
                    model.Title = "All Active Games";
                }
                else if (string.Equals(model.QueryString, "notactive"))
                {
                    model.Games = await _db.Games.Where(g => g.IsDeleted == true).ToListAsync();
                    model.Title = "All Deactivate Games";
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                model.Games = await _db.Games.ToListAsync();
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

    }
}