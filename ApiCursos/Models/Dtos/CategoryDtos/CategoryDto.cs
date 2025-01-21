using System.ComponentModel.DataAnnotations;

namespace ApiCursos.Models.Dtos.CategoryDtos
{
    public class CategoryDto
    {
        [Key]
        public int Id { get; set; }


        [Required (ErrorMessage = "The name is required.")]
        [MaxLength (100, ErrorMessage = "The maximum number of characters is 100.")]
        public string? Name { get; set; }


        [Required]
        public DateTime CreationDate { get; set; }
    }
}
