using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Data.Entities;

namespace GameStore.Areas.Admin.Models.ViewModels
{
    public class TagListViewModel
    {
        [Display(Name="Tag Name")]
        public string Name { get; set; }

        [Display(Name="Number of Products in this Tag")]
        public int NumberOProducts { get; set; }

        public string QueryString { get; set; }
        public string Title { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
    }

    public class TagChangeViewModel
    {
        [Display(Name = "Tag Name")]
        [Required]
        [StringLength(200)]
        public string Name { get; set; }


        [Display(Name = "Active")]
        public bool IsDeleted { get; set; }

        public string GuidValue { get; set; }
    }
}
