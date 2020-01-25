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
    public class GameDeveloperController : Controller
    {
        private AppDbContext _db;

        public GameDeveloperController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index(string active = null)
        {
            GameDeveloperListViewModel model = new GameDeveloperListViewModel();

            // getting the queryString
            model.QueryString = Request.Query[nameof(active)];
            model.GameDevelopers = null;

            // fetching tags based on querystring.
            // if queryString is active then All active tags will be displayed
            // if queryString is nonactive then all non active querystring will be displayed.
            // else all the tags will be displayed.
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