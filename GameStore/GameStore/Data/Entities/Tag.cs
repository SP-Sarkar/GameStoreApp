using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameStore.Data.Entities
{
    [Table("tags")]
    public class Tag : BaseEntity
    {

        [Display(Name = "Tag Label")]
        [Required]
        [StringLength(100)]
        [Column("tagName")]
        public string Name { get; set; }
        
    }
}