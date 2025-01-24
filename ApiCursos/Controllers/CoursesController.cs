﻿using ApiCursos.Models;
using ApiCursos.Models.Dtos.CourseDtos;
using ApiCursos.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiCursos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourse _courRepo;
        private readonly IMapper _mapper;

        public CoursesController(ICourse courRepo, IMapper mapper) { 
        
            _courRepo = courRepo;
            _mapper = mapper;        
        
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCourses()
        {
            var coursesListDto = _courRepo.GetCourses()
                                           .Select(course => _mapper.Map<CourseDto>(course))
                                           .ToList();

            return Ok(coursesListDto);
        }

        [HttpGet("{courseId:int}", Name = "GetCourseById")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetCourse(int courseId)
        {
            var itemCourse = _courRepo.GetCourse(courseId);

            if (itemCourse == null)
            {
                return NotFound();
            }

            var itemCourseDto = _mapper.Map<CourseDto>(itemCourse);

            return Ok(itemCourseDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult CreateCourse([FromBody] CreateCourseDto createCourseDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (createCourseDto == null || string.IsNullOrWhiteSpace(createCourseDto.Name))
                {
                    return BadRequest("The course data cannot be null or empty.");
                }

                if (_courRepo.CourseExists(createCourseDto.Name))
                {
                    ModelState.AddModelError("", "The course already exists.");
                    return BadRequest(ModelState);
                }

                var course = _mapper.Map<Course>(createCourseDto);

                if (!_courRepo.CreateCourse(course))
                {
                    ModelState.AddModelError("", $"Something went wrong saving the course {course.Name}");
                    return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
                }

                return CreatedAtRoute("GetCourseById", new { courseId = course.Id }, course);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPatch("{courseId:int}", Name = "PatchCourseById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdatePatchCourse (int courseId, [FromBody] CourseDto courseDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (courseDto == null || courseId != courseDto.Id)
                {
                    return BadRequest("Invalid data or ID mismatch.");
                }

                var course = _mapper.Map<Course>(courseDto);

                if (!_courRepo.UpdateCourse(course))
                {
                    ModelState.AddModelError("", $"Something went wrong updating the course {course.Name}");
                    return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
                }

                var updatedCourse = _courRepo.GetCourse(courseId);

                if (updatedCourse == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to retrieve the updated course.");
                }

                var updatedCourseDto = _mapper.Map<CourseDto>(updatedCourse);

                return Ok(updatedCourseDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetCoursesInCategory/{categoryId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetCoursesInCategory(int categoryId)
        {
            var coursesList = _courRepo.GetAllCoursesByCategory(categoryId);

            if (coursesList == null)
            {
                return NotFound();
            }

            var itemCourse = new List<CourseDto>();
            foreach (var course in coursesList)
            {
                itemCourse.Add(_mapper.Map<CourseDto>(course));
            }

            return Ok(itemCourse);
        }

        [HttpGet("SearchCourse")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult SearchCourse(string name)
        {
            try
            {
                var result = _courRepo.SearchCourse(name);
                if (result.Any())
                {
                    return Ok(result);
                }

                return NotFound();
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

    }
}
