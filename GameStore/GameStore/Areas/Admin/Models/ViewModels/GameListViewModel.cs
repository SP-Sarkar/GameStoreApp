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
        [Display(Name = "Game Name")]
        public string Name { get; set; }

        [Display(Name = "Game Price")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public string QueryString { get; set; }
        public string Title { get; set; }
        public IEnumerable<Game> Games { get; set; }
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

    }
}
