using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GameStore.Areas.Admin.Models.ViewModels
{
    public class GameListViewModel
    {
        [Display(Name = "Game Name")]
        public string Name { get; set; }

        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public string QueryString { get; set; }
        public string Title { get; set; }
        public IEnumerable<Game> Games { get; set; }

        [Display(Name="Tag ")]
        public string TagName { get; set; }

        [Display(Name = "Developer")]
        public string GameDeveloperName { get; set; }

    }

    public class GameChangeViewModel
    {
        public string Title { get; set; }

        [Display(Name = "Game Name")]
        [Required, StringLength(200)]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$")]
        public string Name { get; set; }

        [Display(Name = "Website URL")]
        [StringLength(200)]
        [RegularExpression(@"^http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$")]
        public string WebUrl { get; set; }

        [Display(Name = "Game Price")]
        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Display(Name = "Game Description")]
        [DataType(DataType.Html)]
        public string Description { get; set; }

        public string GuidValue { get; set; }

        [Display(Name = "Select Tag")]
        [Required]
        public int TagId { get; set; }

        [NotMapped]
        public SelectList TagList { get; set; }

        [Display(Name = "Select Game Company")]
        [Required]
        public int GameDeveloperId { get; set; }

        [NotMapped]
        public SelectList GameDeveloperList { get; set; }

        [Display(Name="Game Category")]
        [Required]
        public IList<int> GameCategoryId { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem> GameCategoryList { get; set; }
    }


    public class GameDetailViewModel
    {
        public string Title { get; set; }

        [Display(Name = "Game Name")]
        public string Name { get; set; }

        [Display(Name = "Website URL")]
        public string WebUrl { get; set; }

        [Display(Name = "Game Price")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Display(Name = "Game Description")]
        public string Description { get; set; }

        public string GuidValue { get; set; }

        [Display(Name = "Created On")]
        public DateTime CTime { get; set; }
        [Display(Name = "Updated  On")]
        public DateTime UTime { get; set; }
        public bool IsDeleted { get; set; }
        public string Slug { get; set; }
        



        [Display(Name = "Select Tag")]
        public int TagId { get; set; }

        public Tag Tag { get; set; }

        [Display(Name = "Select Game Company")]
        public int GameDeveloperId { get; set; }

        public GameDeveloper GameDeveloper { get; set; }

        [Display(Name = "Game Category")]
        public IList<int> GameCategoryId { get; set; }

        public IEnumerable<GameCategory> GameCategoryList { get; set; }
    }
}
