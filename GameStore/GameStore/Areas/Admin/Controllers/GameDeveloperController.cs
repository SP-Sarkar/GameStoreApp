﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GameDeveloperController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}