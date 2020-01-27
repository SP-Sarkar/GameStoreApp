using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("product")]
    public class ProductController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }
    }
}