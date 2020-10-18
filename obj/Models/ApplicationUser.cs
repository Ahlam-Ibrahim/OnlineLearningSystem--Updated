using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLearningSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Column(TypeName ="nvarchar(150)")]
        public string FullName { get; set; }
        //public string UserName { get; set; }


        public virtual ICollection<Enrollment> Enrollments { get; set; }

    }
}
