using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameStore.Data.Entities
{
    [Table("games")]
    public class Game : BaseEntity
    {
        [Display(Name = "Game Name")]
        [Column("gameName")]
        [Required, StringLength(200)]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$")]
        public string Name { get; set; }

        [Display(Name = "Website URL")]
        [Column("gameUrl")]
        [StringLength(200)]
        [RegularExpression(@"^http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$")]
        public string WebUrl { get; set; }

        [Display(Name = "Game Price")]
        [Column("gamePrice")]
        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Display(Name = "Game Description")]
        [Column("gameDesc")]
        [DataType(DataType.Html)]
        public string Description { get; set; }

        // 1--1 [GameTags]
        [Display(Name = "Tag Label")]
        [Column("gameTag")]
        [Required]
        public int TagId { get; set; }

        [ForeignKey(nameof(TagId))]
        public virtual Tag Tag { get; set; }


    }
}