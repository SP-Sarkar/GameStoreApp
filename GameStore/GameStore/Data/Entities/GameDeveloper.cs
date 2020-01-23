using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameStore.Data.Entities
{
    [Table("gameDev")]
    public class GameDeveloper : BaseEntity
    {
        public GameDeveloper()
        {
            Games = new HashSet<Game>();
        }

        [Display(Name = "Developer company Name")]
        [Required]
        [StringLength(400),Column("gameDevName")]
        public string Name { get; set; }

        [Display(Name = "Developer company Website")]
        [StringLength(400), Column("gameDevUrl")]
        [RegularExpression(@"^http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$")]
        public string WebUrl { get; set; }

        [Display(Name = "Developer company Description")]
        [Column("gameDevDesc")]
        [DataType(DataType.Html)]
        public string Description { get; set; }

        [Display(Name = "Developer company Logo")]
        [StringLength(500)]
        public string Logo { get; set; }

        public ICollection<Game> Games { get; set; }
    }
}