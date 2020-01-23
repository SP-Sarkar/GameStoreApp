using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameStore.Data.Entities
{
    [Table("category")]
    public class Category : BaseEntity
    {
        [Display(Name = "Game Category")]
        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [Display(Name = "Category Description")]
        [StringLength(2000)]
        [DataType(DataType.Html)]
        public string Description { get; set; }
    }
}