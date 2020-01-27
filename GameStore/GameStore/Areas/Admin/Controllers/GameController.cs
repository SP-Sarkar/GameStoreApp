using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Areas.Admin.Models.ViewModels;
using GameStore.Data;
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

    }
}