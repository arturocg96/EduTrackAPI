using Microsoft.AspNetCore.Http;
namespace ApiCursos.Models.Dtos.CourseDtos
{
    public enum ClasificationType
    {
        Basic,
        Advanced,
        Master
    }
    public class CreateCourseDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string? ImageRoute { get; set; }
        public required IFormFile Image { get; set; }
        public ClasificationType Clasification { get; set; }
        public int CategoryId { get; set; }
    }
}