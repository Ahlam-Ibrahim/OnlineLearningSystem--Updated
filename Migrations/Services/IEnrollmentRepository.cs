using OnlineLearningSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLearningSystem.Services
{
    public interface IEnrollmentRepository
    {
        public ICollection<Course> GetMyCourses(string studentId);
        public ICollection<Enrollment> MyEnrollmentOrders(string studentId);
        public bool OrderACourse(Course course, ApplicationUser student);
        public ICollection<Enrollment> GetAllEnrollmentOrders();
        public ICollection<Enrollment> GetASpecificEnrollmentOrder(int courseId);
        public ICollection<Enrollment> GetAllEnrollmentOrdersFromAStudent(string studentId);
        public ICollection<Enrollment> GetAllWaitingForPaymentOrders();
        public ICollection<Enrollment> GetWaitingForPaymentOrdersOfACourse(int courseId);
        public ICollection<Enrollment> GetWaitingForPaymentOrdersOfAStudent(string studentId);
        public bool MarkAnEnrollmentOrderAsApproved(Enrollment order);
        public bool MarkAnEnrollmentOrderAsWaitingForPayment(Enrollment order);

    }
}
