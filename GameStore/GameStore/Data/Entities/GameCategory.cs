using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameStore.Data.Entities
{
    public class GameCategory
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Key]
        public int GameId { get; set; }
        [Key]
        public int CategoryId { get; set; }

        public Category Category { get; set; }
        public Game Game { get; set; }
    }
}