using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Data.Entities;

namespace GameStore.Areas.Admin.Models.ViewModels
{
    public class GameDeveloperListViewModel
    {
        [Display(Name = "Game Company Name")]
        public string Name { get; set; }

        [Display(Name = "Number of Games")]
        public int NumberOProducts { get; set; }

        [Display(Name = "Website")]
        public string Url { get; set; }

        [Display(Name = "Company Logo")]
        public string Logo { get; set; }

        public string QueryString { get; set; }
        public string Title { get; set; }
        public IEnumerable<GameDeveloper> GameDevelopers { get; set; }
    }

    public class GameDeveloperChangeViewModel
    {
        [Display(Name = "Game Company Name")]
        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [Display(Name = "Company Description")]
        public string Description { get; set; }

        [Display(Name = "Website Url")]
        [Required]
        [RegularExpression(@"^http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$")]
        public string WebUrl { get; set; }

        [Display(Name = "Company Logo")]
        public IFormFile Logo { get; set; }
    }
}
