using ApiCursos.Models;
using ApiCursos.Models.Dtos.CategoryDtos;
using ApiCursos.Models.Dtos.CourseDtos;
using ApiCursos.Models.Dtos.UserDtos;
using AutoMapper;

namespace ApiCursos.CoursesMapper
{
    public class CoursesMapper : Profile
    {
        public CoursesMapper()
        {
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Category, CreateCategoryDto>().ReverseMap();
            CreateMap<Course, CourseDto>().ReverseMap();
            CreateMap<Course, CreateCourseDto>().ReverseMap();
            CreateMap<Course, UpdateCourseDto>().ReverseMap();
            CreateMap<UserApp, UserDataDto>().ReverseMap();
            CreateMap<UserApp, UserDto>().ReverseMap();
        }
    }
}