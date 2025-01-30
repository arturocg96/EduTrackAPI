using Microsoft.AspNetCore.Http;
namespace ApiCursos.Models.Dtos.CourseDtos
{
    public class UpdateCourseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string? ImageRoute { get; set; }
        public string? ImageLocalRoute { get; set; }
        public required IFormFile Image { get; set; }
        public enum ClasificationType { Basic, Advanced, Master }
        public ClasificationType Clasification { get; set; }
        public DateTime CreationDate { get; set; }
        public int categoryId { get; set; }
    }
}