using System.ComponentModel.DataAnnotations;

namespace ApiCursos.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public DateTime CreationDate { get; set; }
    }
}