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
            _ctRepo = ctRepo ?? throw new ArgumentNullException(nameof(ctRepo));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (createCategoryDto == null)
            {
                return BadRequest(ModelState);
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
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetCategoryById", new { categoryId = category.Id }, category);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{categoryId:int}", Name = "PatchCategoryById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdatePatchCategory(int categoryId, [FromBody] CategoryDto categoryDto)
        {
            if (!ModelState.IsValid || categoryDto == null || categoryId != categoryDto.Id)
            {
                return BadRequest(ModelState);
            }

            var category = _mapper.Map<Category>(categoryDto);

            if (!_ctRepo.UpdateCategory(category))
            {
                ModelState.AddModelError("", $"Something went wrong updating the category {category.Name}");
                return StatusCode(500, ModelState);
            }

            return Ok(_mapper.Map<CategoryDto>(_ctRepo.GetCategory(categoryId)));
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
            if (!ModelState.IsValid || categoryDto == null || categoryId != categoryDto.Id)
            {
                return BadRequest(ModelState);
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
                return StatusCode(500, ModelState);
            }

            return Ok(_mapper.Map<CategoryDto>(_ctRepo.GetCategory(categoryId)));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{categoryId:int}", Name = "DeleteCategoryById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteCategory(int categoryId)
        {
            if (!_ctRepo.CategoryExists(categoryId))
            {
                return NotFound($"Category with ID {categoryId} not found.");
            }

            var category = _ctRepo.GetCategory(categoryId);
            if (category == null)
            {
                return NotFound($"Category with ID {categoryId} not found.");
            }

            if (!_ctRepo.DeleteCategory(category))
            {
                ModelState.AddModelError("", $"Something went wrong deleting the category {category.Name}");
                return StatusCode(500, ModelState);
            }

            return Ok(_mapper.Map<CategoryDto>(category));
        }
    }
}