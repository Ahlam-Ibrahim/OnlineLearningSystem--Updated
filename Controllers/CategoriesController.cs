using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
        private readonly IHostingEnvironment _hostingEnvironment;


        public CategoriesController(ICategoryRepository categoryRepository, 
            ICourseRepository courseRepository,
            IHostingEnvironment hostingEnvironment)
        {
            _categoryRepository = categoryRepository;
            _courseRepository = courseRepository;
            _hostingEnvironment = hostingEnvironment;
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
                    Title = category.Title,
                    PhotoPath = category.PhotoPath

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
                Title = category.Title,
                PhotoPath = category.PhotoPath
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
                    Title = category.Title,
                    PhotoPath = category.PhotoPath
                });
            }

            return Ok(categoriesDto);
        }

        // POST api/catgories
        [HttpPost]
        [Authorize(Roles = "Admin,Mentor", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult CreateCategory([FromForm] IFormFile photo,
            [FromForm] string title)
        {
            if (title == null || photo == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string uniqueFileName = null;
            if (!ModelState.IsValid)
                return BadRequest();

            string uploadsfolder = Path.Combine(_hostingEnvironment.WebRootPath, "CategoryPhotos");
            uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;

            string filePath = Path.Combine(uploadsfolder, uniqueFileName);
            using (FileStream fileStream = new FileStream(filePath,
              FileMode.Create))
            {
                photo.CopyTo(fileStream);
            }

            Category category = new Category
            {
                Title = title,
                PhotoPath = filePath
            };

            if (!_categoryRepository.CreateCategory(category))
                ModelState.AddModelError("", $"Something went wrong!");

            return CreatedAtRoute("GetCategory", new { categoryId = category.Id }, category);

        }

        // POST api/categories/categoryId
        [HttpPut("{categoryId}")]
        [Authorize(Roles = "Admin,Mentor", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult UpdateCategory(int categoryId, [FromForm] IFormFile photo,
            [FromForm] string title)
        {
            if (title == null && photo == null)
                return BadRequest();

            if (!_categoryRepository.CategoryExists(categoryId))
                return NotFound();

            var category = _categoryRepository.GetCategory(categoryId);

            if (categoryId != category.Id)
                return BadRequest();

            string filePath = null; 

            if (photo != null)
            {
                string uniqueFileName = null;
                string uploadsfolder = Path.Combine(_hostingEnvironment.WebRootPath, "CategoryPhotos");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;

                filePath = Path.Combine(uploadsfolder, uniqueFileName);
                using (FileStream fileStream = new FileStream(filePath,
                    FileMode.Create))
                {
                    photo.CopyTo(fileStream);
                    System.IO.File.Delete(Path.Combine(uploadsfolder, category.PhotoPath));
                }
            }
            if(title != null)
            category.Title = title;
            if(filePath != null)
            category.PhotoPath = filePath;

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

            System.IO.File.Delete(Path.Combine(category.PhotoPath));

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
