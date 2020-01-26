using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Data.Entities;

namespace GameStore.Areas.Admin.Models.ViewModels
{
    public class CategoryListViewModel
    {
        [Display(Name = "Category Name")]
        public string Name { get; set; }

        [Display(Name = "Number of Products ")]
        public int NumberOProducts { get; set; }

        public string QueryString { get; set; }
        public string Title { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }

    public class CategoryChangeViewModel
    {
        [Display(Name = "Category Name")]
        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [Display(Name = "Category Description")]
        public string Description { get; set; }

        [Display(Name="Active")]
        public bool IsDeleted { get; set; }

        public string GuidValue { get; set; }

    }
}
