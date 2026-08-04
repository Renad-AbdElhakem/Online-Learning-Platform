using Content_Service.Dtos;
using Content_Service.Model;
using LectureService.Dtos;
using LectureService.Model;

namespace Content_Service.Service
{
    public interface ILectureService
    {
        Task<GeneralResponse<Lecture>> UploadLectureVideo(UploadNewLecture newLecture);
        Task<GeneralResponse<Lecture>> UpdateLectureAsync(Guid lectureId, UpateLectureDto upateLecture);
        Task<bool> LectureExistsAsync(Guid lectureId);
        Task<bool> DeleteLecture(Guid lectureId);
        Task<GeneralResponse<byte[]>> DownloadLectureVideo(Guid lectureId);
        Task<GeneralResponse<Stream>> StreamLectureVideo(Guid lectureId);

    }
}
