using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningSystem.Dtos;
using OnlineLearningSystem.Models;
using OnlineLearningSystem.Services;

namespace OnlineLearningSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : Controller
    {
        private ICategoryRepository _categoryRepository;
        private ICourseRepository _courseRepository;

        public CategoriesController(ICategoryRepository categoryRepository, ICourseRepository courseRepository)
        {
            _categoryRepository = categoryRepository;
            _courseRepository = courseRepository;
        }

        // GET api/categories
        [HttpGet]
        public ActionResult GetCategories()
        {
            var categories = _categoryRepository.GetCategories().ToList();

            if (!ModelState.IsValid)
                return BadRequest();

            var categoriesDto = new List<CategoryDto>();
            foreach (var category in categories)
            {
                categoriesDto.Add(new CategoryDto
                {
                    Id = category.Id,
                    Title = category.Title

                });
            }
            return Ok(categoriesDto);
        }
        // GET api/categories/categoryId
        [HttpGet("{categoryId}", Name = "GetCategory")]
        public ActionResult GetCategory(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
                return NotFound();

            var category = _categoryRepository.GetCategory(categoryId);

            if (!ModelState.IsValid)
                return BadRequest();

            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                Title = category.Title
            };

            return Ok(categoryDto);
        }

        // GET api/categories/courses/courseId
        [HttpGet("courses/{courseId}")]
        public ActionResult GetAllCategoriesOfCourse(int courseId)
        {

            var categories = _categoryRepository.GetAllCategoriesOfCourse(courseId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoriesDto = new List<CategoryDto>();
            foreach (var category in categories)
            {
                categoriesDto.Add(new CategoryDto
                {
                    Id = category.Id,
                    Title = category.Title

                });
            }

            return Ok(categoriesDto);
        }

        // POST api/catgories
        [HttpPost]
        [Authorize(Roles = "Admin,Mentor", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult CreateCategory([FromBody] Category category)
        {
            if (category == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_categoryRepository.CreateCategory(category))
                ModelState.AddModelError("", $"Something went wrong!");

            return CreatedAtRoute("GetCategory", new { categoryId = category.Id }, category);

        }

        // POST api/categories/categoryId
        [HttpPut("{categoryId}")]
        [Authorize(Roles = "Admin,Mentor", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult UpdateCategory(int categoryId, [FromBody] Category category)
        {
            if (category == null)
                return BadRequest();

            if (categoryId != category.Id)
                return BadRequest();

            if (!_categoryRepository.CategoryExists(categoryId))
                return NotFound();

            if (!_categoryRepository.UpdateCategory(category))
                ModelState.AddModelError("", $"Something went wrong!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return NoContent();

        }

        // DELETE api/categories/categoryId
        [HttpDelete("{categoryId}")]
        [Authorize(Roles = "Admin,Mentor", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult DeleteCategory(int categoryId)
        {

            if (!_categoryRepository.CategoryExists(categoryId))
                return NotFound();

            var category = _categoryRepository.GetCategory(categoryId);

            if (!_categoryRepository.DeleteCategory(category))
                ModelState.AddModelError("", $"Something went wrong!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return NoContent();
        }

        // POST api/categories/categoryId/courseId
        [HttpPost("{categoryId}/{courseId}")]
        [Authorize(Roles = "Admin,Mentor", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult AddACategoryToACourse(int courseId, int categoryId)
        {
            if (courseId == null || categoryId == null)
                return BadRequest();

            var course = _courseRepository.GetCourse(courseId);
            var category = _categoryRepository.GetCategory(categoryId);
            if (course == null)
                return BadRequest();
            
            if (category == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_categoryRepository.AddACategoryToACourse(course, category))
            {
                ModelState.AddModelError("", $"Something went wrong!");
                return BadRequest(ModelState);
            }

            return CreatedAtRoute("GetCategory", new { categoryId = category.Id }, category);

        }  
        
        // DELETE  api/categories/categoryId/courseId
        [HttpDelete("{categoryId}/{courseId}")]
        [Authorize(Roles = "Admin,Mentor", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult DeleteACategoryFromACourse(int courseId, int categoryId)
        {
            if (courseId == null || categoryId == null)
                return BadRequest();

            var course = _courseRepository.GetCourse(courseId);
            var category = _categoryRepository.GetCategory(categoryId);
            
            if (course == null)
                return BadRequest();
            
            if (category == null)
                return BadRequest();

            var categories = _categoryRepository.GetAllCategoriesOfCourse(courseId);
            foreach (var eachCategory in categories)
                if (eachCategory.Id != categoryId)
                    return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_categoryRepository.DeleteACategoryToACourse(course, category))
            {
                ModelState.AddModelError("", $"Something went wrong!");
                return BadRequest(ModelState);
            }

            return NoContent();

        }



    }
}
