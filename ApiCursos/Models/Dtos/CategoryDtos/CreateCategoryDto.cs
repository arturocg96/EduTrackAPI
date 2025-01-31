using System.ComponentModel.DataAnnotations;

namespace ApiCursos.Models.Dtos.CategoryDtos
{
    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "The name is required.")]
        [MaxLength(100, ErrorMessage = "The maximum number of characters is 100.")]
        public string Name { get; set; } = string.Empty;
    }
}

