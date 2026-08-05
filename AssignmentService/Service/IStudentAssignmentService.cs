using AssignmentService.Dtos;
using AssignmentService.Shared;

namespace AssignmentService.Service
{
    public interface IStudentAssignmentService
    {
        Task<GeneralResponse<ResponseStudentAssignmentDto>> SubmittedNewAssignment(SubmittedAssignmentDto dto);
        Task<GeneralResponse<ResponseStudentAssignmentDto>> GradeAssignmentAsync(Guid assignmentStudentId, GradeAssignmentDto gradeAssignment);
        Task<GeneralResponse<ResponseStudentAssignmentDto>> GetStudentAssignmentByIdAsync(Guid assignmentStudentId);
        Task<GeneralResponse<byte[]>> DownloadStudentAssignmentAsync(Guid assignmentStudentId);
        Task<GeneralResponse<Stream>> StreamStudentAssignmentAsync(Guid assignmentStudentId);
    }
}
