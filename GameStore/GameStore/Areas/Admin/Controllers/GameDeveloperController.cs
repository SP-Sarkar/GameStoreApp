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


        public IActionResult CreateGameDevelopers()
        {
            throw new NotImplementedException();
        }
    }
}