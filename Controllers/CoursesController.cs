using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineLearningSystem.Dtos;
using OnlineLearningSystem.Models;
using OnlineLearningSystem.Services;

namespace OnlineLearningSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : Controller
    {
        private ICourseRepository _courseRepository;
        private ISectionRepository _sectionRepository;
        private IVideoRepository _videoRepository;
        private readonly IHostingEnvironment _hostingEnvironment;
        private UserManager<ApplicationUser> _userManager;

        public CoursesController(ICourseRepository courseRepository,
            ISectionRepository sectionRepository,
            UserManager<ApplicationUser> userManager,
            IVideoRepository videoRepository,
            IHostingEnvironment hostingEnvironment)
        {
            _courseRepository = courseRepository;
            _sectionRepository = sectionRepository;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
            _videoRepository = videoRepository;
        }

        // GET api/courses
        [HttpGet]
        public ActionResult GetCourses()
        {
            var courses = _courseRepository.GetCourses().ToList();

            if (!ModelState.IsValid)
                return BadRequest();

            var coursesDto = new List<CourseDto>();
            foreach (var course in courses)
            {
                coursesDto.Add(new CourseDto
                {
                    Id = course.Id,
                    Title = course.Title,
                    Description = course.Description,
                    Duration = course.Duration,
                    DateCreated = course.DateCreated.Date,
                    Price = course.Price,
                    Location = course.Location,
                    CoursePicturePath = course.CoursePicturePath,
                    CoursePreviewVideoPath = course.CoursePreviewVideoPath
                });
            }
            return Ok(coursesDto);
        }
        // GET api/courses/courseId
        [HttpGet("{courseId}", Name = "GetCourse")]
        public ActionResult GetCourse(int courseId)
        {
            if (!_courseRepository.CourseExists(courseId))
                return NotFound();

            var course = _courseRepository.GetCourse(courseId);

            if (!ModelState.IsValid)
                return BadRequest();

            var courseDto = new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Duration = course.Duration,
                DateCreated = course.DateCreated.Date,
                Price = course.Price,
                Location = course.Location,
                CoursePicturePath = course.CoursePicturePath,
                CoursePreviewVideoPath = course.CoursePreviewVideoPath
            };

            return Ok(courseDto);
        }

        // GET api/courses/categories/categoryId
        [HttpGet("categories/{categoryId}")]
        public ActionResult GetAllCoursesOfCategory(int categoryId)
        {

            var courses = _courseRepository.GetAllCoursesOfCategory(categoryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var CoursesDto = new List<CourseDto>();
            foreach (var course in courses)
            {
                CoursesDto.Add(new CourseDto
                {
                    Id = course.Id,
                    Title = course.Title,
                    Description = course.Description,
                    Duration = course.Duration,
                    DateCreated = course.DateCreated,
                    Price = course.Price,
                    Location = course.Location,
                    CoursePicturePath = course.CoursePicturePath,
                    CoursePreviewVideoPath = course.CoursePreviewVideoPath
                });
            }

            return Ok(CoursesDto);
        }

        // POST api/courses
        [HttpPost]
        [Authorize(Roles = "Admin,Mentor", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult CreateCourse([FromForm] string title, [FromForm] string description,
           [FromForm] double duration, [FromForm] double price,
           [FromForm] Location location, [FromForm] IFormFile coursePicture, 
           [FromForm] IFormFile coursePreviewVideo)
        {
            if (title == null || description == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest();

            string uploadsfolderPicture = Path.Combine(_hostingEnvironment.WebRootPath, "CoursePictures");
            string uniqueFileNamePicture = Guid.NewGuid().ToString() + "_" + coursePicture.FileName;
            string filePathPicture = Path.Combine(uploadsfolderPicture, uniqueFileNamePicture);

            using (FileStream fileStream = new FileStream(filePathPicture,
              FileMode.Create))
            {
                coursePicture.CopyTo(fileStream);
            }

            string uploadsfolderVideo = Path.Combine(_hostingEnvironment.WebRootPath, "CoursePreviewVideos");
            string uniqueFileNameVideo = Guid.NewGuid().ToString() + "_" + coursePreviewVideo.FileName;
            string filePathVideo = Path.Combine(uploadsfolderVideo, uniqueFileNameVideo);

            using (FileStream fileStream = new FileStream(filePathVideo,
              FileMode.Create))
            {
                coursePreviewVideo.CopyTo(fileStream);
            }

            Course course = new Course
            {
                Title = title,
                Description = description,
                Duration = duration,
                Price = price,
                Location = location,
                DateCreated = DateTime.Now.Date,
                CoursePicturePath = filePathPicture,
                CoursePreviewVideoPath = filePathVideo
            };

            if (!_courseRepository.CreateCourse(course))
                ModelState.AddModelError("", $"Something went wrong!");

            return CreatedAtRoute("GetCourse", new { courseId = course.Id }, course);

        }

        // POST api/courses/courseId
        [HttpPut("{courseId}")]
        [Authorize(Roles = "Admin,Mentor", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult UpdateCourse(int courseId, [FromForm] string title, [FromForm] string description,
           [FromForm] double duration, [FromForm] double price,
           [FromForm] Location location, [FromForm] IFormFile coursePicture,
           [FromForm] IFormFile coursePreviewVideo)
        {
            if (!_courseRepository.CourseExists(courseId))
                return NotFound();

            var course = _courseRepository.GetCourse(courseId);

            if (courseId != course.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string filePathPicture = null;
            string filePathVideo = null;

            if (!ModelState.IsValid)
                return BadRequest();

            //if the parameter vlue is null, do not assign it

            if (coursePicture != null)
            {
                string uploadsfolderPicture = Path.Combine(_hostingEnvironment.WebRootPath, "CoursePictures");
                string uniqueFileNamePicture = Guid.NewGuid().ToString() + "_" + coursePicture.FileName;
                filePathPicture = Path.Combine(uploadsfolderPicture, uniqueFileNamePicture);

                using (FileStream fileStream = new FileStream(filePathPicture,
                  FileMode.Create))
                {
                    coursePicture.CopyTo(fileStream);
                    System.IO.File.Delete(Path.Combine(uploadsfolderPicture, course.CoursePicturePath));
                }
                course.CoursePicturePath = filePathPicture;
            }

            if (coursePreviewVideo != null)
            {
                string uploadsfolderVideo = Path.Combine(_hostingEnvironment.WebRootPath, "CoursePreviewVideos");
                string uniqueFileNameVideo = Guid.NewGuid().ToString() + "_" + coursePreviewVideo.FileName;
                filePathVideo = Path.Combine(uploadsfolderVideo, uniqueFileNameVideo);

                using (FileStream fileStream = new FileStream(filePathVideo,
                  FileMode.Create))
                {
                    coursePreviewVideo.CopyTo(fileStream);
                    System.IO.File.Delete(Path.Combine(uploadsfolderVideo, course.CoursePreviewVideoPath));
                }
                course.CoursePreviewVideoPath = filePathVideo;
            }

            if (title != null)
                course.Title = title;
            if (description != null)
                course.Description = description;
            if (price != 0)
                course.Price = price;
            if (duration != 0)
                course.Duration = duration;
            if (location != 0)
                course.Location = location;
            

            if (!_courseRepository.UpdateCourse(course))
                ModelState.AddModelError("", $"Something went wrong!");


            return NoContent();

        }


        // DELETE api/courses/courseId
        [HttpDelete("{courseId}")]
        [Authorize(Roles = "Admin,Mentor", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult DeleteCourse(int courseId)
        {

            if (!_courseRepository.CourseExists(courseId))
                return NotFound();

            var course = _courseRepository.GetCourse(courseId);

            //Delete course Media
            System.IO.File.Delete(Path.Combine(course.CoursePicturePath));
            System.IO.File.Delete(Path.Combine(course.CoursePreviewVideoPath));


            //There is a conflict with the section table 
            //if the course has sections delete them too
            //The conflict arise in one-to-many relationshsips

            if (_courseRepository.GetAllSectionOfACourse(courseId).Count() > 0)
            {
                var sections = _courseRepository.GetAllSectionOfACourse(courseId);
                foreach (var section in sections)
                {
                    if (!_sectionRepository.DeleteSection(section))
                        return BadRequest(ModelState);
                }
            }
            if (!_courseRepository.DeleteCourse(course))
                ModelState.AddModelError("", $"Something went wrong!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return NoContent();
        }

    }
}
