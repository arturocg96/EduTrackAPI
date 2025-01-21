using ApiCursos.Models;
using ApiCursos.Models.Dtos.CategoryDtos;
using AutoMapper;

namespace ApiCursos.CoursesMapper
{
    public class CoursesMapper:Profile
    {
        public CoursesMapper() { 
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Category, CreateCategoryDto>().ReverseMap();
        }
    }
}
