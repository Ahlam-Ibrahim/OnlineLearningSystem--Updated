using OnlineLearningSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;

namespace OnlineLearningSystem.Services
{
    public class CourseRepository : ICourseRepository
    {
        private ApplicationDbContext _courseContext;
        public CourseRepository(ApplicationDbContext courseContext)
        {
            _courseContext = courseContext;
        }

        public bool CourseExists(int id)
        {
            return _courseContext.Courses.Any(c => c.Id == id);
        }

        public bool CreateCourse(Course course)
        {
            _courseContext.Add(course);
            return Save();
        }

        public bool CreateSectionOfACourse(int courseId, Section section)
        {
            var course = GetCourse(courseId);

            course.Sections = new List<Section>();
            course.Sections.Add(section);

            section.Course = course;
            _courseContext.Sections.Add(section);

            return Save();
        }

        public bool DeleteCourse(Course country)
        {
            _courseContext.Remove(country);
            return Save();
        }

        public bool DeleteSectionOfACourse(Section section, Course course)
        {
            _courseContext.Sections.Remove(section);
            return Save();
        }

        public bool UpdateSectionOfACourse(Course course, Section section, string newSection)
        {
            section.Title = newSection;

            //after adding videos this method will be updated 

            _courseContext.Sections.Update(section);

            return Save();
        }

        public ICollection<Course> GetAllCoursesOfCategory(int categoryId)
        {
            return _courseContext.CourseCategories.Where(c => c.CategoryID == categoryId)
                          .Select(c => c.Course).ToList();
        }

        public ICollection<Section> GetAllSectionOfACourse(int courseId)
        {
            return _courseContext.Sections.Where(c => c.Course.Id == courseId).ToList();
        }

        public Course GetCourse(int courseId)
        {
            return _courseContext.Courses.Where(c => c.Id == courseId).FirstOrDefault();
        }

        public ICollection<Course> GetCourses()
        {
            return _courseContext.Courses.OrderBy(c => c.Title).ToList();
        }

        public bool IsDuplicateCourseName(int id, string counrseName)
        {
            var course = _courseContext.Courses.Where(c => c.Title.Trim().ToUpper()
            == counrseName.Trim().ToUpper() && c.Id != id).FirstOrDefault();

            return course == null ? false : true;
        }

        public bool Save()
        {
            var saved = _courseContext.SaveChanges();
            return saved >= 0 ? true : false;
        }

        public bool UpdateCourse(Course course)
        {
            _courseContext.Update(course);
            return Save();
        }


        // MyCourses - Student Section 

        //approved courses
        public ICollection<Course> GetMyCourses(string studentId)
        {
             return
             _courseContext.StudentCourses
                .Where(s => (int) s.Status == 2 // 2 = approved
                && s.StudentID == studentId)
                .Select(c => c.Course).ToList();
        }

        //student can follow the status of his/her orders
        public ICollection<StudentCourse> MyOrderedCourses(string studentId)
        {
            return _courseContext.StudentCourses
            .Where(s => s.StudentID == studentId
            && (int) s.Status == 1 // 1 = ordered
            || s.StudentID == studentId 
            && (int) s.Status == 3 // 3 = WaitingForPayment
            ).ToList();
        }

        //student can order a course
        public bool OrderACourse(Course course, ApplicationUser student)
        {
          
            StudentCourse myCourses = new StudentCourse
            {
                Course = course,
                CourseID = course.Id,
                Student = student,
                StudentID = student.Id,
                Status = Status.Ordered
            };
            _courseContext.StudentCourses.Add(myCourses);
            return Save();
        }

        // MyCourses - Admin will view the orders and mark them

        // 3 Views For Ordered Courses 
        public ICollection<StudentCourse> GetAllCoursesOrders()
        {
            return _courseContext.StudentCourses
            .Where(s => (int) s.Status == 1).ToList(); // 1 = ordered
        } 
        
        public ICollection<StudentCourse> GetASpecificCourseOrders(int courseId)
        {
            return _courseContext.StudentCourses
            .Where(s => (int) s.Status == 1  // 1 = ordered
            && s.CourseID == courseId).ToList();
        } 
        public ICollection<StudentCourse> GetAllOrdersFromAStudent(string studentId)
        {
            return _courseContext.StudentCourses
            .Where(s => (int)s.Status == 1 // 1 = ordered
            && s.StudentID == studentId).ToList();
        }

        // 3 Views For WaitingForPayment Courses 
        public ICollection<StudentCourse> GetAllWaitingForPaymentOrders()
        {
            return _courseContext.StudentCourses
           .Where(s => (int)s.Status == 3).ToList(); // 1 = WaitingForPayment
        } 
        
        public ICollection<StudentCourse> GetWaitingForPaymentOrdersOfACourse(int courseId)
        {
            return _courseContext.StudentCourses
            .Where(s => (int)s.Status == 3 //WaitingForPayment
            && s.CourseID == courseId).ToList();
        } 
        public ICollection<StudentCourse> GetWaitingForPaymentOrdersOfAStudent(string studentId)
        {
            return _courseContext.StudentCourses
            .Where(s => (int)s.Status == 3 //WaitingForPayment
            && s.StudentID == studentId).ToList();
        } 
        
        public bool MarkACourseOrderAsApproved(StudentCourse courseOrder)
        {

            //The admin will set the status of this course 
            //for this particular student as => Approved
            courseOrder.Status = Status.Approved;
            _courseContext.StudentCourses.Update(courseOrder); 
            return Save();
        } 
        public bool MarkACourseOrderAsWaitingForPayment(StudentCourse courseOrder)
        {
            //The admin will set the status of this course 
            //for this particular student as => WaitingForPayment
            courseOrder.Status = Status.WaitingForPayment;
            _courseContext.StudentCourses.Update(courseOrder);
            return Save();
        }

    }
}
