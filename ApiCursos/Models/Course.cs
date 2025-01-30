using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ApiCursos.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string? ImageRoute { get; set; }
        public string? ImageLocalRoute { get; set; }
        public enum ClasificationType { Basic, Advanced, Master }
        public ClasificationType Clasification { get; set; }
        public DateTime CreationDate { get; set; }
        public int categoryId { get; set; }
        [ForeignKey("categoryId")]
        public required Category Category { get; set; }
    }
}