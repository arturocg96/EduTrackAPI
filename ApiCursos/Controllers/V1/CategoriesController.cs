using ApiCursos.Models;
using ApiCursos.Models.Dtos.CategoryDtos;
using ApiCursos.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;

namespace ApiCursos.Controllers.V1
{
    [Route("api/v{version:apiVersion}/categories")]
    [ApiVersion("1.0")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategory _ctRepo;
        private readonly IMapper _mapper;

        public CategoriesController(ICategory ctRepo, IMapper mapper)
        {
            _ctRepo = ctRepo;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCategories()
        {
            var categoriesListDto = _ctRepo.GetCategories()
                                           .Select(category => _mapper.Map<CategoryDto>(category))
                                           .ToList();

            return Ok(categoriesListDto);
        }

        [AllowAnonymous]
        [HttpGet("{categoryId:int}", Name = "GetCategoryById")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetCategory(int categoryId)
        {
            var itemCategory = _ctRepo.GetCategory(categoryId);

            if (itemCategory == null)
            {
                return NotFound();
            }

            var itemCategoryDto = _mapper.Map<CategoryDto>(itemCategory);

            return Ok(itemCategoryDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (createCategoryDto == null || string.IsNullOrWhiteSpace(createCategoryDto.Name))
                {
                    return BadRequest("The category data cannot be null or empty.");
                }

                if (_ctRepo.CategoryExists(createCategoryDto.Name))
                {
                    ModelState.AddModelError("", "The category already exists.");
                    return BadRequest(ModelState);
                }

                var category = _mapper.Map<Category>(createCategoryDto);

                if (!_ctRepo.CreateCategory(category))
                {
                    ModelState.AddModelError("", $"Something went wrong saving the category {category.Name}");
                    return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
                }

                return CreatedAtRoute("GetCategoryById", new { categoryId = category.Id }, category);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{categoryId:int}", Name = "PatchCategoryById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdatePatchCategory(int categoryId, [FromBody] CategoryDto categoryDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (categoryDto == null || categoryId != categoryDto.Id)
                {
                    return BadRequest("Invalid data or ID mismatch.");
                }

                var category = _mapper.Map<Category>(categoryDto);

                if (!_ctRepo.UpdateCategory(category))
                {
                    ModelState.AddModelError("", $"Something went wrong updating the category {category.Name}");
                    return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
                }

                var updatedCategory = _ctRepo.GetCategory(categoryId);

                if (updatedCategory == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to retrieve the updated category.");
                }

                var updatedCategoryDto = _mapper.Map<CategoryDto>(updatedCategory);

                return Ok(updatedCategoryDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{categoryId:int}", Name = "PutCategoryById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdatePutCategory(int categoryId, [FromBody] CategoryDto categoryDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (categoryDto == null || categoryId != categoryDto.Id)
                {
                    return BadRequest("Invalid data or ID mismatch.");
                }

                var categoryThatExists = _ctRepo.GetCategory(categoryId);
                if (categoryThatExists == null)
                {
                    return NotFound($"Category with ID {categoryId} not found.");
                }

                var category = _mapper.Map<Category>(categoryDto);

                if (!_ctRepo.UpdateCategory(category))
                {
                    ModelState.AddModelError("", $"Something went wrong updating the category {category.Name}");
                    return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
                }

                var updatedCategory = _ctRepo.GetCategory(categoryId);

                if (updatedCategory == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to retrieve the updated category.");
                }

                var updatedCategoryDto = _mapper.Map<CategoryDto>(updatedCategory);

                return Ok(updatedCategoryDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }


        //[Authorize(Roles = "Admin")]
        [HttpDelete("{categoryId:int}", Name = "DeleteCategoryById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteCategory(int categoryId)
        {
            try
            {
                if (!_ctRepo.CategoryExists(categoryId))
                {
                    return NotFound($"Category with ID {categoryId} not found.");
                }

                var category = _ctRepo.GetCategory(categoryId);
                if (category == null)
                {
                    return NotFound($"Category with ID {categoryId} could not be retrieved.");
                }

                var categoryDto = _mapper.Map<CategoryDto>(category);

                if (!_ctRepo.DeleteCategory(category))
                {
                    ModelState.AddModelError("", $"Something went wrong deleting the category {category.Name}");
                    return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
                }

                return Ok(categoryDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }


    }
}
