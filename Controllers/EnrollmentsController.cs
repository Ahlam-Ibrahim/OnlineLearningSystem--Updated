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
    public class EnrollmentsController : Controller
    {
        private IEnrollmentRepository _enrollmentRepository;
        private ISectionRepository _sectionRepository;
        private IVideoRepository _videoRepository;
        private ICourseRepository _courseRepository;
        private UserManager<ApplicationUser> _userManager;

        public EnrollmentsController(IEnrollmentRepository enrollmentRepository,
            ISectionRepository sectionRepository,
            UserManager<ApplicationUser> userManager,
            ICourseRepository courseRepository,
            IVideoRepository videoRepository)
        {
            _enrollmentRepository = enrollmentRepository;
            _sectionRepository = sectionRepository;
            _userManager = userManager;
            _courseRepository = courseRepository;
            _videoRepository = videoRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Student", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //GET: api/enrollments
        public async Task<Object> GetMyCourses()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            var myCourses = _enrollmentRepository.GetMyCourses(userId).ToList();
            //only the approved courses will be viewable
            return Ok(myCourses);
        }

        [HttpGet("student/orders")]
        [Authorize(Roles = "Student", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //GET: api/enrollments/student/orders
        public async Task<Object> MyOrderedCourses()
        {
            string studentId = User.Claims.First(c => c.Type == "UserID").Value;
            var student = await _userManager.FindByIdAsync(studentId);
            var myOrders = _enrollmentRepository.MyEnrollmentOrders(studentId).ToList();
            //only the orders made for courses will be viewable
            return Ok(myOrders);
        }

        // POST api/order/courseId
        [HttpPost("order/{courseId}")]
        [Authorize(Roles = "Student", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<Object> OrderACourse(int courseId)
        {
            if (courseId == null)
                return BadRequest();

            Course course = null;

            if (!_courseRepository.CourseExists(courseId))
                return NotFound();

                 course = _courseRepository.GetCourse(courseId);

            string studentId = User.Claims.First(c => c.Type == "UserID").Value;
            var student = await _userManager.FindByIdAsync(studentId);

            if (course == null)
                return BadRequest();

            if (student == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_enrollmentRepository.OrderACourse(course, student))
            {
                ModelState.AddModelError("", $"Something went wrong!");
                return BadRequest(ModelState);
            }

            return CreatedAtRoute("GetCourse", new { courseId = course.Id }, course);

        }

        // 3 Views For Ordered Courses 

        // #1: Get all orderes

        // GET api/enrollments/orders
        [HttpGet("orders")]
        [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult GetAllCoursesOrders()
        {
            var orders = _enrollmentRepository.GetAllEnrollmentOrders().ToList();

            if (!ModelState.IsValid)
                return BadRequest();

            var ordersDto = new List<EnrollmentDto>();
            foreach (var order in orders)
            {
                ordersDto.Add(new EnrollmentDto
                {
                    CourseID = order.CourseID,
                    StudentID = order.StudentID,
                    Status = order.Status
                });
            }
            return Ok(ordersDto);
        }

        // #2: Get all orderes For a Specific Course

        // GET api/enrollments/orders/{courseId}
        [HttpGet("orders/course/{courseId}")]
        [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        // 3 Views For Ordered Courses 
        public ActionResult GetASpecificCourseOrders(int courseId)
        {
            var courseOrders = _enrollmentRepository.GetASpecificEnrollmentOrder(courseId).ToList();

            if (!ModelState.IsValid)
                return BadRequest();

            var ordersDto = new List<EnrollmentDto>();
            foreach (var order in courseOrders)
            {
                ordersDto.Add(new EnrollmentDto
                {
                    CourseID = order.CourseID,
                    StudentID = order.StudentID,
                    Status = order.Status
                });
            }
            return Ok(ordersDto);
        }


        // #3: Get all orderes For a Specific Student

        // GET api/enrollments/orders/student/{studentId}
        [HttpGet("orders/student/{studentId}")]
        [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        // 3 Views For Ordered Courses 
        public ActionResult GetAllOrdersFromAStudent(string studentId)
        {
            var courseOrders = _enrollmentRepository.GetAllEnrollmentOrdersFromAStudent(studentId).ToList();

            if (!ModelState.IsValid)
                return BadRequest();

            var ordersDto = new List<EnrollmentDto>();
            foreach (var order in courseOrders)
            {
                ordersDto.Add(new EnrollmentDto
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

        // GET api/enrollments/orders/waiting
        [HttpGet("orders/waiting")]
        [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult GetAllWaitingForPaymentOrders()
        {
            var orders = _enrollmentRepository.GetAllWaitingForPaymentOrders().ToList();

            if (!ModelState.IsValid)
                return BadRequest();

            var ordersDto = new List<EnrollmentDto>();
            foreach (var order in orders)
            {
                ordersDto.Add(new EnrollmentDto
                {
                    CourseID = order.CourseID,
                    StudentID = order.StudentID,
                    Status = order.Status
                });
            }
            return Ok(ordersDto);
        }


        // GET api/enrollments/orders/waiting/{courseId}
        [HttpGet("orders/waiting/{courseId}")]
        [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult GetWaitingForPaymentOrdersOfACourse(int courseId)
        {
            var orders = _enrollmentRepository.GetWaitingForPaymentOrdersOfACourse(courseId).ToList();

            if (!ModelState.IsValid)
                return BadRequest();

            var ordersDto = new List<EnrollmentDto>();
            foreach (var order in orders)
            {
                ordersDto.Add(new EnrollmentDto
                {
                    CourseID = order.CourseID,
                    StudentID = order.StudentID,
                    Status = order.Status
                });
            }
            return Ok(ordersDto);
        }

        // GET api/enrollments/orders/waiting/{studentId}
        [HttpGet("orders/waiting/student/{studentId}")]
        [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult GetWaitingForPaymentOrdersOfAStudent(string studentId)
        {
            var orders = _enrollmentRepository.GetWaitingForPaymentOrdersOfAStudent(studentId).ToList();

            if (!ModelState.IsValid)
                return BadRequest();

            var ordersDto = new List<EnrollmentDto>();
            foreach (var order in orders)
            {
                ordersDto.Add(new EnrollmentDto
                {
                    CourseID = order.CourseID,
                    StudentID = order.StudentID,
                    Status = order.Status
                });
            }
            return Ok(ordersDto);
        }

        // MyCourses - Admin Marks Orders 

        // PUT api/enrollments/orders/approve
        [HttpPut("orders/approve")]
        [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<Object> MarkACourseOrderAsApproved([FromBody] Enrollment order)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _enrollmentRepository.MarkAnEnrollmentOrderAsApproved(order);

            return NoContent();

        }


        // PUT api/enrollments/orders/waiting
        [HttpPut("orders/waiting")]
        [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<Object> MarkACourseOrderAsWaitingForPayment([FromBody] Enrollment order)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _enrollmentRepository.MarkAnEnrollmentOrderAsWaitingForPayment(order);

            return NoContent();

        }



    }
}
