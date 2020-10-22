using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLearningSystem.Models
{
    public class CategoryCreateModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public IFormFile Picture { get; set; }
        public virtual ICollection<CourseCategory> CourseCategories { get; set; }
    }
}
