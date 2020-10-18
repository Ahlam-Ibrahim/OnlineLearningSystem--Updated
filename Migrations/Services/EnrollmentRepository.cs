using OnlineLearningSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLearningSystem.Services
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private ApplicationDbContext _enrollmentContext;
        public EnrollmentRepository(ApplicationDbContext enrollmentContext)
        {
            _enrollmentContext = enrollmentContext;
        }
        //approved courses
        public ICollection<Course> GetMyCourses(string studentId)
        {
            return
            _enrollmentContext.Enrollments
               .Where(s => (int)s.Status == 2 // 2 = approved
               && s.StudentID == studentId)
               .Select(c => c.Course).ToList();
        }

        //student can follow the status of his/her orders
        public ICollection<Enrollment> MyEnrollmentOrders(string studentId)
        {
            return _enrollmentContext.Enrollments
            .Where(s => s.StudentID == studentId
            && (int)s.Status == 1 // 1 = ordered
            || s.StudentID == studentId
            && (int)s.Status == 3 // 3 = WaitingForPayment
            ).ToList();
        }

        //student can order a course
        public bool OrderACourse(Course course, ApplicationUser student)
        {

            Enrollment myCourses = new Enrollment
            {
                Course = course,
                CourseID = course.Id,
                Student = student,
                StudentID = student.Id,
                Status = Status.Ordered
            };
            _enrollmentContext.Enrollments.Add(myCourses);
            return Save();
        }

        // MyCourses - Admin will view the orders and mark them

        // 3 Views For Ordered Courses 
        public ICollection<Enrollment> GetAllEnrollmentOrders()
        {
            return _enrollmentContext.Enrollments
            .Where(s => (int)s.Status == 1).ToList(); // 1 = ordered
        }

        public ICollection<Enrollment> GetASpecificEnrollmentOrder(int courseId)
        {
            return _enrollmentContext.Enrollments
            .Where(s => (int)s.Status == 1  // 1 = ordered
            && s.CourseID == courseId).ToList();
        }
        public ICollection<Enrollment> GetAllEnrollmentOrdersFromAStudent(string studentId)
        {
            return _enrollmentContext.Enrollments
            .Where(s => (int)s.Status == 1 // 1 = ordered
            && s.StudentID == studentId).ToList();
        }

        // 3 Views For WaitingForPayment Courses 
        public ICollection<Enrollment> GetAllWaitingForPaymentOrders()
        {
            return _enrollmentContext.Enrollments
           .Where(s => (int)s.Status == 3).ToList(); // 1 = WaitingForPayment
        }

        public ICollection<Enrollment> GetWaitingForPaymentOrdersOfACourse(int courseId)
        {
            return _enrollmentContext.Enrollments
            .Where(s => (int)s.Status == 3 //WaitingForPayment
            && s.CourseID == courseId).ToList();
        }
        public ICollection<Enrollment> GetWaitingForPaymentOrdersOfAStudent(string studentId)
        {
            return _enrollmentContext.Enrollments
            .Where(s => (int)s.Status == 3 //WaitingForPayment
            && s.StudentID == studentId).ToList();
        }

        public bool MarkAnEnrollmentOrderAsApproved(Enrollment courseOrder)
        {

            //The admin will set the status of this course 
            //for this particular student as => Approved
            courseOrder.Status = Status.Approved;
            _enrollmentContext.Enrollments.Update(courseOrder);
            return Save();
        }
        public bool MarkAnEnrollmentOrderAsWaitingForPayment(Enrollment courseOrder)
        {
            //The admin will set the status of this course 
            //for this particular student as => WaitingForPayment
            courseOrder.Status = Status.WaitingForPayment;
            _enrollmentContext.Enrollments.Update(courseOrder);
            return Save();
        } 
        public bool Save()
        {
            var saved = _enrollmentContext.SaveChanges();
            return saved >= 0 ? true : false;
        }
    }
}
