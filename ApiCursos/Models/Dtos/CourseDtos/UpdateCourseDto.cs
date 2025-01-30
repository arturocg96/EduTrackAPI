namespace ApiCursos.Models.Dtos.CourseDtos
{
    public class UpdateCourseDto
    {
        
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public string? ImageRoute { get; set; }
        public string? ImageLocalRoute { get; set; }
        public IFormFile Image { get; set; }
        public enum ClasificationType { Basic, Advanced, Master }
        public ClasificationType Clasification { get; set; }
        public DateTime CreationDate { get; set; }

        public int categoryId { get; set; }
       
    }
}

