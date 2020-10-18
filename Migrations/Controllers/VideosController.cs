using System;
using System.IO;
using System.Collections.Generic;
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
using System.Diagnostics;

namespace OnlineLearningSystem.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class VideosController : ControllerBase
    {
        private readonly IVideoRepository _videoRepository;
        private readonly ISectionRepository _sectionRepository;
        private readonly IHostingEnvironment _hostingEnvironment;

        public VideosController(IVideoRepository videoRepository,
            IHostingEnvironment hostingEnvironment, ISectionRepository sectionRepository)
        {
            _videoRepository = videoRepository;
            _hostingEnvironment = hostingEnvironment;
            _sectionRepository = sectionRepository;
        }

        // GET api/videos/all/{sectionId}
        [HttpGet("all/{sectionId}")]
        public ActionResult GetAllVideosFromSection(int sectionId) //re-implement
        {
            var videos = _videoRepository.GetAllVideosFromSection(sectionId).ToList();
   
            if (!ModelState.IsValid)
                return BadRequest();

            return Ok(videos);
        }

        // GET api/videos/videoId
        [HttpGet("{videoId}", Name = "GetVideo")]
        public ActionResult GetVideo(int videoId)
        {
            if (!_videoRepository.VideoExists(videoId))
                return NotFound();

            var video = _videoRepository.GetVideo(videoId);

            if (!ModelState.IsValid)
                return BadRequest();

            return Ok(video);
        }

        // POST api/videos
        [HttpPost]
        public IActionResult UploadVideoforPostman([FromForm] IFormFile video,
            [FromForm] string title, [FromForm] int sectionId)
        {
            string uniqueFileName = null;
            if (!ModelState.IsValid)
                return BadRequest();

            string uploadsfolder = Path.Combine(_hostingEnvironment.WebRootPath, "Videos");
            uniqueFileName = Guid.NewGuid().ToString() + "_" + video.FileName;

            string filePath = Path.Combine(uploadsfolder, uniqueFileName);
            using (FileStream fileStream = new FileStream(filePath,
              FileMode.Create))
            {
                video.CopyTo(fileStream);
            }

            if (!_sectionRepository.SectionExists(sectionId))
                return NotFound();

            var section = _sectionRepository.GetSection(sectionId);

            Video newVideo = new Video
            {
                Title = title,
                VideoPath = uniqueFileName,
                Section = section
            };

            if (!_videoRepository.CreateVideo(newVideo))
                return BadRequest();

            return CreatedAtRoute("GetVideo", new { videoId = newVideo.Id }, video);
        }


        // DELETE api/videos/videoId
        [HttpDelete("{videoId}")]
        [Authorize(Roles = "Admin,Mentor", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult DeleteVideo(int videoId)
        {
            if (!_videoRepository.VideoExists(videoId))
                return NotFound();

            var video = _videoRepository.GetVideo(videoId);

            if (!_videoRepository.DeleteVideo(video))
                ModelState.AddModelError("", $"Something went wrong!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return NoContent();

        }

        // PUT api/videos/sectionId/videoId
        [HttpPut("{videoId}")]
        [Authorize(Roles = "Admin,Mentor", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult UpdateVideoOfASection(int videoId, [FromForm] IFormFile video,
            [FromForm] string title)
        {
            if (!_videoRepository.VideoExists(videoId))
                return NotFound();

            var videoToUpdate = _videoRepository.GetVideo(videoId);

            string uniqueFileName = null;
            string uploadsfolder = Path.Combine(_hostingEnvironment.WebRootPath, "Videos");
            uniqueFileName = Guid.NewGuid().ToString() + "_" + video.FileName;

            string filePath = Path.Combine(uploadsfolder, uniqueFileName);
            using (FileStream fileStream = new FileStream(filePath, 
                FileMode.Create))
            {
                video.CopyTo(fileStream);
                System.IO.File.Delete(Path.Combine(uploadsfolder, videoToUpdate.VideoPath));
            }
            videoToUpdate.Title = title;
            videoToUpdate.VideoPath = uniqueFileName;

            if (!_videoRepository.UpdateVideoOfASection(videoToUpdate))
                ModelState.AddModelError("", $"Something went wrong!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return NoContent();

        }


        // Upload and Update with the VideoCreateModel - Integerate with frontend 

        //// PUT api/videos/videoId
        //[HttpPut("{videoId}")]
        //[Authorize(Roles = "Admin,Mentor", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public ActionResult UpdateVideoOfASection(int videoId, VideoCreateModel video)
        //{

        //    var videoToUpdate = _videoRepository.GetVideo(videoId);

        //    string uniqueFileName = null;
        //    string uploadsfolder = Path.Combine(_hostingEnvironment.WebRootPath, "Videos");
        //    uniqueFileName = Guid.NewGuid().ToString() + "_" + video.Video.FileName;

        //    string filePath = Path.Combine(uploadsfolder, uniqueFileName);
        //    using (FileStream fileStream = new FileStream(filePath,
        //        FileMode.Create))
        //    {
        //        video.Video.CopyTo(fileStream);
        //        System.IO.File.Delete(Path.Combine(uploadsfolder, videoToUpdate.VideoPath));
        //    }

        //    if (!_videoRepository.UpdateVideoOfASection(videoToUpdate))
        //        ModelState.AddModelError("", $"Something went wrong!");

        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    return NoContent();
        //}


        // POST api/videos
        //[HttpPost]
        //public IActionResult UploadVideo(VideoCreateModel video)
        //{
        //    string uniqueFileName = null;
        //    if (!ModelState.IsValid)
        //        return BadRequest();

        //    //brings the absolut path of wwwroot folder
        //    //we will append it with the videos folder and the video title
        //    string uploadsfolder = Path.Combine(_hostingEnvironment.WebRootPath, "Videos");

        //    //to have unique file names/titles, it uses Guid - Globally Unique Identifier
        ////    uniqueFileName = Guid.NewGuid().ToString() + "_" + video.Video.FileName;

        ////    string filePath = Path.Combine(uploadsfolder, uniqueFileName);
        //      using (FileStream fileStream = new FileStream(filePath,
        //      FileMode.Create))
        //      {
        //        video.Video.CopyTo(fileStream);
        //       }

        ////    Video newVideo = new Video
        ////    {
        ////        Title = video.Title,
        //        VideoPath = uniqueFileName,
        //        Section = video.Section
        //    };

        //    _videoRepository.CreateVideo(newVideo);
        //    return CreatedAtRoute("GetVideo", new { videoId = newVideo.Id }, newVideo);
        //}

    }
}
