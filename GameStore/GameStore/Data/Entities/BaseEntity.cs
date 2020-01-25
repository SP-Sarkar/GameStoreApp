using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameStore.Data.Entities
{
    public abstract class BaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Guid GuidValue { get; set; }

        [Display(Name="Added On")]
        [DataType(DataType.DateTime)]
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true,DataFormatString = "{0:dd MMM yyyy HH:mm tt}")]
        public DateTime CTime { get; set; }

        [Display(Name = "Updated on")]
        [DataType(DataType.DateTime)]
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMM yyyy HH:mm tt}")]
        public DateTime UTime { get; set; }

        [Display(Name = "Active")]
        [Required]
        [Range(0,1)]
        public bool IsDeleted { get; set; }

        [Required]
        [StringLength(220)]
        public string Slug { get; set; }
    }
}