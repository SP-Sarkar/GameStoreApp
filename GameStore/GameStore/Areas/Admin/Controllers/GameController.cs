using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Data;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("product")]
    public class ProductController : Controller
    {
        private AppDbContext _db;

        public ProductController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}