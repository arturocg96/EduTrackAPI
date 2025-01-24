using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCursos.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }

        public string ImageRoute { get; set; }
        public enum ClasificationType { Basic, Advanced, Master}
        public ClasificationType Clasification { get; set; }
        public  DateTime CreationDate { get; set; }

        //Category relation
        
        public int categoryId { get; set; }
        [ForeignKey("categoryId")]
        public Category Category { get; set; }
    }
}
