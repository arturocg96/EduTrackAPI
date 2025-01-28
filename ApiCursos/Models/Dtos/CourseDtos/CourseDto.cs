using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCursos.Models.Dtos.CourseDtos
{

    public class CourseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public string ImageRoute { get; set; }
        public enum ClasificationType { Basic, Advanced, Master }
        public ClasificationType Clasification { get; set; }
        public DateTime CreationDate { get; set; }     

        public int categoryId { get; set; }
        
    }
}
