using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLearningSystem.Models
{
    public class VideoCreateModel
    {
        [Required]
        public string Title { get; set; }
        public IFormFile Video { get; set; }
        public virtual Section Section { get; set; }
        
    }
}
