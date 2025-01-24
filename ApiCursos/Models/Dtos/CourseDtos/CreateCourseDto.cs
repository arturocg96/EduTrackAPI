namespace ApiCursos.Models.Dtos.CourseDtos
{
    public class CreateCourseDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public string ImageRoute { get; set; }
        public enum ClasificationType { Basic, Advanced, Master }
        public ClasificationType Clasification { get; set; }      
        public int categoryId { get; set; }
    }
}
