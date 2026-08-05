using AssignmentService.Dtos;
using AssignmentService.Model;
using AssignmentService.Shared;

namespace AssignmentService.Service
{
    public interface IAssignmentService
    {
        Task<GeneralResponse<AssignmentResponseDto>> UploadAssignmentAsync(UploadNewAssignment newAssignment);

        Task<GeneralResponse<AssignmentResponseDto>> UpdateAssignmentAsync(Guid assignmentId, UpdateAssignmentDto updateAssignment);

        Task<bool> AssignmentExistsAsync(Guid assignmentId);
        Task<bool> HideAssignmentAsync(Guid assignmentId);
        Task<bool> DeleteAssignmentAsync(Guid assignmentId);
        Task<GeneralResponse<AssignmentResponseDto>> GetAssignmentNameAndTime(Guid assignmentId);
        Task<GeneralResponse<byte[]>> DownloadAssignmentAsync(Guid assignmentId);

        Task<GeneralResponse<Stream>> StreamAssignmentAsync(Guid assignmentId);
    }
}
