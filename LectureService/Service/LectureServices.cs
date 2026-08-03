using Content_Service.Dtos;
using Content_Service.ExternalService;
using Content_Service.Model;
using Content_Service.Service;
using LectureService.Data;
using LectureService.Dtos;
using LectureService.Model;
using Microsoft.AspNetCore.Http;

namespace LectureService.Service
{
    public class LectureServices : ILectureService
    {
        private readonly ContentDbContext _dbContext;
        private readonly GroupClient _groupClient;

        public LectureServices(ContentDbContext dbContext, GroupClient groupClient)
        {
            _dbContext = dbContext;
            _groupClient = groupClient;
        }


        public async Task<GeneralResponse<Lecture>> UploadLectureVideo(UploadNewLecture newLecture)
        {

            if (newLecture.VideoLecture == null || newLecture.VideoLecture.Length == 0)
            {
                return GeneralResponse<Lecture>.Failed("No file received.");
            }


            var allowedExtension = new[] { ".mp4" };
            var extension = Path.GetExtension(newLecture.VideoLecture.FileName).ToLowerInvariant();

            if (!allowedExtension.Contains(extension))
            {
                return GeneralResponse<Lecture>.Failed("file type not allowed");
            }

            //R
            var group = await _groupClient.GetByIdAsync(newLecture.GroupId);

            if (!group.IsSuccseded)
            {
                return GeneralResponse<Lecture>.Failed($"Group with id {newLecture.GroupId} not found or not active ");
            }

            var videoLocation = await SaveLectureVideoAsync(newLecture.VideoLecture, newLecture.LectureName);


            var lecture = new Lecture
            {
                LectureTitle = newLecture.LectureName,
                Description = newLecture.Description,
                VideoLecture = videoLocation,
                GroupId = newLecture.GroupId,
            };

            await _dbContext.AddAsync(lecture);
            await _dbContext.SaveChangesAsync();

            return GeneralResponse<Lecture>.Success(lecture, "File uploaded successfully.");
        }


        public async Task<GeneralResponse<Lecture>> UpdateLectureAsync(Guid lectureId, UpateLectureDto upateLecture)
        {
            var lecture = await _dbContext.Lectures.FindAsync(lectureId);

            if (lecture is null)
                return GeneralResponse<Lecture>.Failed($"Lecture with id {lectureId} not found");

            if (!string.IsNullOrEmpty(upateLecture.LectureName))
                lecture.LectureTitle = upateLecture.LectureName;

            if (upateLecture.VideoLecture != null)
            {
                var videoLocation = await SaveLectureVideoAsync(upateLecture.VideoLecture, lecture.LectureTitle);
                lecture.VideoLecture = videoLocation;
            }
            if (!string.IsNullOrEmpty(upateLecture.Description))
                lecture.Description = upateLecture.Description;

            if (upateLecture.GroupId.HasValue)
            {
                var group = await _groupClient.GetByIdAsync(upateLecture.GroupId.Value);

                if (!group.IsSuccseded)
                    return GeneralResponse<Lecture>.Failed($"Group with id {upateLecture.GroupId} not found or not active ");

                lecture.GroupId = upateLecture.GroupId.Value;
            }
            await _dbContext.SaveChangesAsync();

            return GeneralResponse<Lecture>.Success(lecture, "Updated successfully.");
        }
        public async Task<bool> DeleteLecture(Guid lectureId)
        {
            var lecture = await _dbContext.Lectures.FindAsync(lectureId);
            if (lecture is null)
                return false;

            _dbContext.Remove(lectureId);
            await _dbContext.SaveChangesAsync();
            return true;
        }



        public async Task<GeneralResponse<byte[]>> DownloadLectureVideo(Guid lectureId)
        {
            var lecture = await _dbContext.Lectures.FindAsync(lectureId);
            if (lecture == null)
                return GeneralResponse<byte[]>.Failed("Lecture not found.");

            if (!File.Exists(lecture.VideoLecture))
                return GeneralResponse<byte[]>.Failed("File not found.");

            var bytes = await File.ReadAllBytesAsync(lecture.VideoLecture);
            return GeneralResponse<byte[]>.Success(bytes, "File ready for download.");
        }

        public async Task<GeneralResponse<Stream>> StreamLectureVideo(Guid lectureId)
        {
            var lecture = await _dbContext.Lectures.FindAsync(lectureId);
            if (lecture == null)
                return GeneralResponse<Stream>.Failed("Lecture not found.");

            if (!File.Exists(lecture.VideoLecture))
                return GeneralResponse<Stream>.Failed("File not found.");

            var stream = File.OpenRead(lecture.VideoLecture);
            return GeneralResponse<Stream>.Success(stream, "Stream ready.");
        }



        private async Task<string> SaveLectureVideoAsync(IFormFile videoLecture, string lectureName)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "VideoLectures");

            Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}_{lectureName}_{Path.GetExtension(videoLecture.FileName)}";

            var videoFilePath = Path.Combine(folderPath, fileName);

            using var stream = new FileStream(videoFilePath, FileMode.Create);

            await videoLecture.CopyToAsync(stream);

            return videoFilePath;

        }




    }
}
