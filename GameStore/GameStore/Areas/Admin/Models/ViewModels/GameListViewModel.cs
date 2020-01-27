using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Data.Entities;

namespace GameStore.Areas.Admin.Models.ViewModels
{
    public class GameListViewModel
    {
        [Display(Name = "Tag Name")]
        [Display(Name = "Game Name")]
        public string Name { get; set; }

        public string QueryString { get; set; }
        public string Title { get; set; }
        public IEnumerable<Game> Games { get; set; }
    }
}
