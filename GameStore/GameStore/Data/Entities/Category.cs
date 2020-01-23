using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameStore.Data.Entities
{
    [Table("category")]
    public class Category : BaseEntity
    {
        public Category()
        {
            GameCategories = new HashSet<GameCategory>();
        }

        [Display(Name = "Game Category")]
        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [Display(Name = "Category Description")]
        [StringLength(2000)]
        [DataType(DataType.Html)]
        public string Description { get; set; }

        [Display(Name = "Game Categories")]
        public ICollection<GameCategory> GameCategories { get; set; }
    }
}