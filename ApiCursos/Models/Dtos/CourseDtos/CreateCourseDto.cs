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
        public string Name { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public string? ImageRoute { get; set; }
        public IFormFile Image {  get; set; }
        public ClasificationType Clasification { get; set; }
        public int CategoryId { get; set; }
    }
}
