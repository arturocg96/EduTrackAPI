using ApiCursos.Models;
using ApiCursos.Models.Dtos.CategoryDtos;
using ApiCursos.Models.Dtos.CourseDtos;
using ApiCursos.Repository.IRepository;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiCursos.Controllers.V1
{
    [Route("api/v{version:apiVersion}/courses")]
    [ApiVersion("1.0")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourse _courRepo;
        private readonly IMapper _mapper;

        public CoursesController(ICourse courRepo, IMapper mapper)
        {

            _courRepo = courRepo;
            _mapper = mapper;

        }

        [AllowAnonymous]
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

        [AllowAnonymous]
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

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult CreateCourse([FromForm] CreateCourseDto createCourseDto)
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

                if (createCourseDto.Image != null)
                {
                    // Validación básica de tipo de archivo
                    var extension = Path.GetExtension(createCourseDto.Image.FileName).ToLower();
                    if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
                    {
                        return BadRequest("Only .jpg, .jpeg and .png files are allowed.");
                    }

                    string fileName = course.Id + Guid.NewGuid().ToString() + extension;
                    string fileRoute = @"wwwroot\CoursesImages\" + fileName;
                    var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), fileRoute);

                    // Asegurarnos que el directorio existe
                    var directory = Path.GetDirectoryName(directoryPath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    FileInfo file = new FileInfo(directoryPath);
                    if (file.Exists)
                    {
                        file.Delete();
                    }

                    using (var fileStream = new FileStream(directoryPath, FileMode.Create))
                    {
                        createCourseDto.Image.CopyTo(fileStream);
                    }

                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    course.ImageRoute = baseUrl + "/CoursesImages/" + fileName;
                    course.ImageLocalRoute = fileRoute;
                }
                else
                {
                    course.ImageRoute = "https://placehold.co/600x400";
                }

                _courRepo.CreateCourse(course);

                return CreatedAtRoute("GetCourseById", new { courseId = course.Id }, course);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{courseId:int}", Name = "PatchCourseById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdatePatchCourse(int courseId, [FromForm] UpdateCourseDto updateCourseDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (updateCourseDto == null || courseId != updateCourseDto.Id)
                {
                    return BadRequest("Invalid data or ID mismatch.");
                }

                var course = _mapper.Map<Course>(updateCourseDto);

                if (updateCourseDto.Image != null)
                {
                    // Validación básica de tipo de archivo
                    var extension = Path.GetExtension(updateCourseDto.Image.FileName).ToLower();
                    if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
                    {
                        return BadRequest("Only .jpg, .jpeg and .png files are allowed.");
                    }

                    string nombreArchivo = course.Id + Guid.NewGuid().ToString() + extension;
                    string rutaArchivo = @"wwwroot\CoursesImages\" + nombreArchivo;
                    var ubicacionDirectorio = Path.Combine(Directory.GetCurrentDirectory(), rutaArchivo);

                    // Asegurarnos que el directorio existe
                    var directory = Path.GetDirectoryName(ubicacionDirectorio);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    // Eliminar imagen anterior si existe
                    if (!string.IsNullOrEmpty(course.ImageLocalRoute) && System.IO.File.Exists(course.ImageLocalRoute))
                    {
                        System.IO.File.Delete(course.ImageLocalRoute);
                    }

                    FileInfo file = new FileInfo(ubicacionDirectorio);
                    if (file.Exists)
                    {
                        file.Delete();
                    }

                    using (var fileStream = new FileStream(ubicacionDirectorio, FileMode.Create))
                    {
                        updateCourseDto.Image.CopyTo(fileStream);
                    }

                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    course.ImageRoute = baseUrl + "/CourseImages/" + nombreArchivo;
                    course.ImageLocalRoute = rutaArchivo;
                }
                else
                {
                    course.ImageRoute = "https://placehold.co/600x400";
                }

                _courRepo.UpdateCourse(course);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }


        [AllowAnonymous]
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


        [AllowAnonymous]
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

        [Authorize(Roles = "Admin")]
        [HttpDelete("{courseId:int}", Name = "DeleteCourseById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult DeleteCourse(int courseId)
        {
            try
            {
                if (!_courRepo.CourseExists(courseId))
                {
                    return NotFound($"Course with ID {courseId} not found.");
                }

                var course = _courRepo.GetCourse(courseId);
                if (course == null)
                {
                    return NotFound($"Course with ID {courseId} could not be retrieved.");
                }

                var courseDto = _mapper.Map<CourseDto>(course);

                if (!_courRepo.DeleteCourse(course))
                {
                    ModelState.AddModelError("", $"Something went wrong deleting the course {course.Name}");
                    return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
                }

                return Ok(courseDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }


    }
}
