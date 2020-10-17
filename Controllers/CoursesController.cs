using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Connections.Features;
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
        private UserManager<ApplicationUser> _userManager;

        public CoursesController(ICourseRepository courseRepository,
            ISectionRepository sectionRepository,
            UserManager<ApplicationUser> userManager,
            IVideoRepository videoRepository)
        {
            _courseRepository = courseRepository;
            _sectionRepository = sectionRepository;
            _userManager = userManager;
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
                    DateCreated = course.DateCreated.Date
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
                DateCreated = course.DateCreated.Date
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
                    DateCreated = course.DateCreated
                });
            }

            return Ok(CoursesDto);
        }

        // POST api/courses
        [HttpPost]
        [Authorize(Roles = "Admin,Mentor", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult CreateCourse([FromBody] Course course)
        {
            course.DateCreated = DateTime.Now.Date;

            if (course == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_courseRepository.CreateCourse(course))
                ModelState.AddModelError("", $"Something went wrong!");

            return CreatedAtRoute("GetCourse", new { courseId = course.Id }, course);

        }

        // POST api/courses/courseId
        [HttpPut("{courseId}")]
        [Authorize(Roles = "Admin,Mentor", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult UpdateCourse(int courseId, [FromBody] Course course)
        {
            if (course == null)
                return BadRequest();

            if (courseId != course.Id)
                return BadRequest();

            if (!_courseRepository.CourseExists(courseId))
                return NotFound();

            if (!_courseRepository.UpdateCourse(course))
                ModelState.AddModelError("", $"Something went wrong!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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

        // Sections -- Controlling the section(s) connected to a course

        //This request returns the content/sections of the course, unlocking
        //the course content is allowed for students whom orders were approved
        //by the admin.
        //admin and mentor can access the course sections w/o restrictions.

        // GET api/courses/sections/courseId
        [HttpGet("sections/{courseId}")]
        [Authorize(Roles = "Admin, Mentor, Student", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<Object> GetAllSectionOfACourse(int courseId)
        {

            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            var userRole = await _userManager.GetRolesAsync(user);

            var myCourses = _courseRepository.GetMyCourses(userId).ToList();
            bool approvedForStudent = false;

            //First: retrieve the course's sections
            var sections = new List<Section>();
            sections = (List<Section>)_courseRepository.GetAllSectionOfACourse(courseId);
            var sectionsDto = new List<SectionDto>();
            foreach (var section in sections)
            {
                sectionsDto.Add(new SectionDto
                {
                    Id = section.Id,
                    Title = section.Title,
                    Videos = section.Videos
                });
            }
            if (userRole[0].Equals("Admin") || userRole[0].Equals("Mentor"))
            {
                    return Ok(sectionsDto);
            }

                if (userRole[0].Equals("Student"))
            {

                foreach (var course in myCourses) 
                       //all the courses in myCourses are approved by the admin
                {
                        if (courseId == course.Id)
                        {
                            approvedForStudent = true;
                            break;
                        }
                }
                if (approvedForStudent)
                {
                        return Ok(sectionsDto);
                }
                else //if the course is not approved
                {
                     return Ok("You can't view the content of this course," +
                         "order the course, or follow your pre-existing order");
                }


            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            //otherwise
            return Ok("Please Sign Up!");
        }

        // GET api/courses/section/courseId/sectionId
        [HttpGet("section/{courseId}/{sectionId}", Name = "GetSection")]
        [Authorize(Roles = "Admin, Mentor, Student", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<Object> GetSection(int sectionId, int courseId)
        {
         
            if (!_sectionRepository.SectionExists(sectionId))
                return NotFound();
            if (!ModelState.IsValid)
                return BadRequest();

            var section = new Section();

            section = _sectionRepository.GetSection(sectionId);

            if (section == null)
                return BadRequest();

            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            var userRole = await _userManager.GetRolesAsync(user);

            var myCourses = _courseRepository.GetMyCourses(userId).ToList();
            bool approvedForStudent = false;

            var sectionDto = new SectionDto() { 
                    Id = section.Id,
                    Title = section.Title,
                    Videos = section.Videos
                };

            if (userRole[0].Equals("Admin") || userRole[0].Equals("Mentor"))
            {
                return Ok(sectionDto);
            }

            if (userRole[0].Equals("Student"))
            {

                foreach (var course in myCourses)
                //all the courses in myCourses are approved by the admin
                {
                    if (courseId == course.Id)
                    {
                        approvedForStudent = true;
                        break;
                    }
                }
                if (approvedForStudent)
                {
                    return Ok(sectionDto);
                }
                else //if the course is not approved
                {
                    return Ok("You can't view the content of this course," +
                        "order the course, or follow your pre-existing order");
                }


            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            //otherwise
            return Ok("Please Sign Up!");
        }

        // POST api/courses/sections/courseId
        [HttpPost("sections/{courseId}")]
        [Authorize(Roles = "Admin,Mentor", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult CreateSectionOfACourse(int courseId, [FromBody] Section section)
        {
            if (section == null)
                return BadRequest();

            if (_courseRepository.GetCourse(courseId) == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_courseRepository.CreateSectionOfACourse(courseId, section))
            {
                ModelState.AddModelError("", $"Something went wrong!");
                return BadRequest(ModelState);
            }

            return CreatedAtRoute("GetSection", new { sectionId = section.Id }, section);

        }

        // DELETE api/courses/sections/courseId/sectionId
        [HttpDelete("sections/{courseId}/{sectionId}")]
        [Authorize(Roles = "Admin,Mentor", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult DeleteSection(int courseId, int sectionId)
        {
            if (!_courseRepository.CourseExists(courseId))
                return NotFound();

            if (!_sectionRepository.SectionExists(sectionId))
                return NotFound();

            var section = _sectionRepository.GetSection(sectionId);
            var course = _courseRepository.GetCourse(courseId);

            if (_videoRepository.GetAllVideosFromSection(sectionId).Count() > 0)
            {
                var videos = _videoRepository.GetAllVideosFromSection(sectionId);
                foreach (var video in videos)
                {
                    if (!_videoRepository.DeleteVideo(video))
                        return BadRequest(ModelState);
                }
            }

            if (!_courseRepository.DeleteSectionOfACourse(section, course))
                ModelState.AddModelError("", $"Something went wrong!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return NoContent();

        }


        //after adding videos this method will be updated 
        // PUT api/courses/sections/courseId/sectionId
        [HttpPut("sections/{courseId}/{sectionId}")]
        [Authorize(Roles = "Admin,Mentor", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult UpdateSectionOfACourse(int courseId, int sectionId, 
            [FromBody] string newSection)
        {

            if (!_courseRepository.CourseExists(courseId))
                return NotFound();

            if (!_sectionRepository.SectionExists(sectionId))
                return NotFound();

            var section = _sectionRepository.GetSection(sectionId);
            var course = _courseRepository.GetCourse(courseId);

            if (!_courseRepository.UpdateSectionOfACourse(course, section, newSection))
                ModelState.AddModelError("", $"Something went wrong!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return NoContent();

        }

        // MyCourses Section - courses for a particular student

        [HttpGet("myCourses")]
        [Authorize(Roles = "Student", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //GET: api/courses/myCourses
        public async Task<Object> GetMyCourses()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            var myCourses = _courseRepository.GetMyCourses(userId).ToList();
            //only the approved courses will be viewable
            return Ok(myCourses);
        }

        [HttpGet("myorders")]
        [Authorize(Roles = "Student", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //GET: api/courses/myOrders
        public async Task<Object> MyOrderedCourses()
        {
            string studentId = User.Claims.First(c => c.Type == "UserID").Value;
            var student = await _userManager.FindByIdAsync(studentId);
            var myOrders = _courseRepository.MyOrderedCourses(studentId).ToList();
            //only the orders made for courses will be viewable
            return Ok(myOrders);
        }

        // POST api/courses/order/courseId
        [HttpPost("order/{courseId}")]
        [Authorize(Roles = "Student", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<Object> OrderACourse(int courseId)
        {
            if (courseId == null)
                return BadRequest();

            var course = _courseRepository.GetCourse(courseId);
            string studentId = User.Claims.First(c => c.Type == "UserID").Value;
            var student = await _userManager.FindByIdAsync(studentId);

            if (course == null)
                return BadRequest();

            if (student == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_courseRepository.OrderACourse(course, student))
            {
                ModelState.AddModelError("", $"Something went wrong!");
                return BadRequest(ModelState);
            }

            return CreatedAtRoute("GetCourse", new { courseId = course.Id }, course);

        }

        // 3 Views For Ordered Courses 
        
        // #1: Get all orderes

        // GET api/courses/orders
        [HttpGet("orders")]
        [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult GetAllCoursesOrders()
        {
            var orders = _courseRepository.GetAllCoursesOrders().ToList();

            if (!ModelState.IsValid)
                return BadRequest();

            var ordersDto = new List<StudentCourseDto>();
            foreach (var order in orders)
            {
                ordersDto.Add(new StudentCourseDto
                {
                    CourseID = order.CourseID,
                    StudentID = order.StudentID,
                    Status = order.Status
                });
            }
            return Ok(ordersDto);
        }

        // #2: Get all orderes For a Specific Course

        // GET api/courses/orders/{courseId}
        [HttpGet("orders/{courseId}")]
        [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        // 3 Views For Ordered Courses 
        public ActionResult GetASpecificCourseOrders(int courseId)
        {
            var courseOrders = _courseRepository.GetASpecificCourseOrders(courseId).ToList();

            if (!ModelState.IsValid)
                return BadRequest();

            var ordersDto = new List<StudentCourseDto>();
            foreach (var order in courseOrders)
            {
                ordersDto.Add(new StudentCourseDto
                {
                    CourseID = order.CourseID,
                    StudentID = order.StudentID,
                    Status = order.Status
                });
            }
            return Ok(ordersDto);
        }


        // #3: Get all orderes For a Specific Student

        // GET api/courses/orders/student/{studentId}
        [HttpGet("orders/student/{studentId}")]
        [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        // 3 Views For Ordered Courses 
        public ActionResult GetAllOrdersFromAStudent(string studentId)
        {
            var courseOrders = _courseRepository.GetAllOrdersFromAStudent(studentId).ToList();

            if (!ModelState.IsValid)
                return BadRequest();

            var ordersDto = new List<StudentCourseDto>();
            foreach (var order in courseOrders)
            {
                ordersDto.Add(new StudentCourseDto
                {
                    CourseID = order.CourseID,
                    StudentID = order.StudentID,
                    Status = order.Status
                });
            }
            return Ok(ordersDto);
        }

        // 3 Views For WaitingForPayment Courses 

        // #1: Get all orderes

        // GET api/courses/orders/waiting
        [HttpGet("orders/waiting")]
        [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult GetAllWaitingForPaymentOrders()
        {
            var orders = _courseRepository.GetAllWaitingForPaymentOrders().ToList();

            if (!ModelState.IsValid)
                return BadRequest();

            var ordersDto = new List<StudentCourseDto>();
            foreach (var order in orders)
            {
                ordersDto.Add(new StudentCourseDto
                {
                    CourseID = order.CourseID,
                    StudentID = order.StudentID,
                    Status = order.Status
                });
            }
            return Ok(ordersDto);
        }


        // GET api/courses/orders/waiting/{courseId}
        [HttpGet("orders/waiting/{courseId}")]
        [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult GetWaitingForPaymentOrdersOfACourse(int courseId)
        {
            var orders = _courseRepository.GetWaitingForPaymentOrdersOfACourse(courseId).ToList();

            if (!ModelState.IsValid)
                return BadRequest();

            var ordersDto = new List<StudentCourseDto>();
            foreach (var order in orders)
            {
                ordersDto.Add(new StudentCourseDto
                {
                    CourseID = order.CourseID,
                    StudentID = order.StudentID,
                    Status = order.Status
                });
            }
            return Ok(ordersDto);
        }
        
        // GET api/courses/orders/waiting/{studentId}
        [HttpGet("orders/waiting/student/{studentId}")]
        [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult GetWaitingForPaymentOrdersOfAStudent(string studentId)
        {
            var orders = _courseRepository.GetWaitingForPaymentOrdersOfAStudent(studentId).ToList();

            if (!ModelState.IsValid)
                return BadRequest();

            var ordersDto = new List<StudentCourseDto>();
            foreach (var order in orders)
            {
                ordersDto.Add(new StudentCourseDto
                {
                    CourseID = order.CourseID,
                    StudentID = order.StudentID,
                    Status = order.Status
                });
            }
            return Ok(ordersDto);
        }

        // MyCourses - Admin Marks Orders 

        // PUT api/courses/orders/approve
        [HttpPut("orders/approve")]
        [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<Object> MarkACourseOrderAsApproved([FromBody] StudentCourse order)
        {
           
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _courseRepository.MarkACourseOrderAsApproved(order);

            return NoContent();

        }


        // PUT api/courses/orders/waiting
        [HttpPut("orders/waiting")]
        [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<Object> MarkACourseOrderAsWaitingForPayment([FromBody] StudentCourse order)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _courseRepository.MarkACourseOrderAsWaitingForPayment(order);

            return NoContent();

        }



    }
}
