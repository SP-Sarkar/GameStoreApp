using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameStore.Data.Entities
{
    [Table("gameImages")]
    public class GameImage : BaseEntity
    {
        [Display(Name = "Game Screenshots")]
        [StringLength(500)]
        public string ImageUrl { get; set; }

        [Display(Name="Alternative text for image")]
        [StringLength(1000)]
        public string AlternativeText { get; set; }

        
    }
}