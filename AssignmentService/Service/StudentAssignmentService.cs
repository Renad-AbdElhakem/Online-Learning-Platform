using AssignmentService.Data;
using AssignmentService.Dtos;
using AssignmentService.ExternalService;
using AssignmentService.Model;
using AssignmentService.Shared;

namespace AssignmentService.Service
{
    public class StudentAssignmentService : IStudentAssignmentService
    {
        private readonly AssignmentDbContext _dbContext;
        private readonly IAssignmentService _assignmentService;
        private readonly StudentClient _studentClient;

        public StudentAssignmentService(AssignmentDbContext dbContext, IAssignmentService assignmentService, StudentClient studentClient)
        {
            _dbContext = dbContext;
            _assignmentService = assignmentService;
            _studentClient = studentClient;
        }


        public async Task<GeneralResponse<ResponseStudentAssignmentDto>> SubmittedNewAssignment(SubmittedAssignmentDto dto)
        {
            var submissionTime = DateTime.Now;

            if (dto.SubmissionFile.Length == 0 || dto.SubmissionFile is null)
                GeneralResponse<ResponseStudentAssignmentDto>.Failed("No file recived");


            var studentClientResponse = await _studentClient.GetByIdAsync(dto.StudentId);

            if (!studentClientResponse.IsSuccseded)

                return GeneralResponse<ResponseStudentAssignmentDto>.Failed("student not found");


            var assignment = await _assignmentService.GetAssignmentNameAndTime(dto.AssignmentId);
            if (assignment is null)
                return GeneralResponse<ResponseStudentAssignmentDto>.Failed("Assignment not found ");

            if (!IsAllowedExtension(dto.SubmissionFile))
                return GeneralResponse<ResponseStudentAssignmentDto>.Failed("File type not allowed.");

            var submittedAssignmentLocation = await SaveSubmittedAssignmentAsync(dto.SubmissionFile, assignment.Data.AssignmentName);

            var studentIsLater = submissionTime > assignment.Data.EndAt;
           
            var submissionStudentAssignment = new StudentAssignment
            {
                SubmissionFileUrl = submittedAssignmentLocation,
                StudentId = dto.StudentId,
                AssignmentId = dto.AssignmentId,
                IsSubmitted = true,
                IsLater = studentIsLater

            };


            await _dbContext.StudentAssignments.AddAsync(submissionStudentAssignment);
            await _dbContext.SaveChangesAsync();

            var response = new ResponseStudentAssignmentDto
            {
                SubmissionFileUrl = submittedAssignmentLocation,
                SubmittedAt = submissionTime,
                IsSubmitted = true,
                Feedback = string.Empty,
                Grade = null,
                IsLater = studentIsLater

            };
            return GeneralResponse<ResponseStudentAssignmentDto>.Success(response, "Submitted successfully");
        }


        public async Task<GeneralResponse<ResponseStudentAssignmentDto>> GradeAssignmentAsync(Guid assignmentStudentId, GradeAssignmentDto gradeAssignment)
        {
            var assignmentStudent = await _dbContext.StudentAssignments
                .FindAsync(assignmentStudentId);

            if (assignmentStudent is null)
                return GeneralResponse<ResponseStudentAssignmentDto>.Failed($"Assignment submission with id {assignmentStudentId} not found.");

            assignmentStudent.Grade = gradeAssignment.Grade;
            assignmentStudent.Feedback = gradeAssignment.Feedback;

            await _dbContext.SaveChangesAsync();

            var response = new ResponseStudentAssignmentDto
            {
                SubmissionFileUrl = assignmentStudent.SubmissionFileUrl,
                SubmittedAt = assignmentStudent.SubmittedAt,
                IsSubmitted = assignmentStudent.IsSubmitted,
                IsLater = assignmentStudent.IsLater,
                Grade = assignmentStudent.Grade,
                Feedback = assignmentStudent.Feedback
            };

            return GeneralResponse<ResponseStudentAssignmentDto>.Success(response,"Assignment graded successfully.");
        }



        public async Task<GeneralResponse<ResponseStudentAssignmentDto>> GetStudentAssignmentByIdAsync(Guid assignmentStudentId)
        {
            var assignmentStudent = await _dbContext.StudentAssignments .FindAsync(assignmentStudentId);

            if (assignmentStudent is null)
                return GeneralResponse<ResponseStudentAssignmentDto>.Failed(
                    $"Assignment submission with id {assignmentStudentId} not found.");

            var response = new ResponseStudentAssignmentDto
            {
                SubmissionFileUrl = assignmentStudent.SubmissionFileUrl,
                SubmittedAt = assignmentStudent.SubmittedAt,
                IsSubmitted = assignmentStudent.IsSubmitted,
                IsLater = assignmentStudent.IsLater,
                Grade = assignmentStudent.Grade,
                Feedback = assignmentStudent.Feedback
            };

            return GeneralResponse<ResponseStudentAssignmentDto>.Success( response );
        }


        public async Task<GeneralResponse<byte[]>> DownloadStudentAssignmentAsync(Guid assignmentStudentId)
        {
            var assignmentStudent = await _dbContext.StudentAssignments.FindAsync(assignmentStudentId);

            if (assignmentStudent == null)
                return GeneralResponse<byte[]>.Failed("Assignment submission not found.");

            if (string.IsNullOrWhiteSpace(assignmentStudent.SubmissionFileUrl))
                return GeneralResponse<byte[]>.Failed("No submitted file found.");

            if (!File.Exists(assignmentStudent.SubmissionFileUrl))
                return GeneralResponse<byte[]>.Failed("File not found.");

            var bytes = await File.ReadAllBytesAsync(assignmentStudent.SubmissionFileUrl);

            return GeneralResponse<byte[]>.Success(bytes, "File ready for download.");
        }

        public async Task<GeneralResponse<Stream>> StreamStudentAssignmentAsync(Guid assignmentStudentId)
        {
            var assignmentStudent = await _dbContext.StudentAssignments.FindAsync(assignmentStudentId);

            if (assignmentStudent == null)
                return GeneralResponse<Stream>.Failed("Assignment submission not found.");

            if (string.IsNullOrWhiteSpace(assignmentStudent.SubmissionFileUrl))
                return GeneralResponse<Stream>.Failed("No submitted file found.");

            if (!File.Exists(assignmentStudent.SubmissionFileUrl))
                return GeneralResponse<Stream>.Failed("File not found.");

            var stream = File.OpenRead(assignmentStudent.SubmissionFileUrl);

            return GeneralResponse<Stream>.Success(stream, "Stream ready.");
        }

        private bool IsAllowedExtension(IFormFile file)
        {

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf", ".sln", ".exe" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                return false;
            }

            return true;
        }


        private async Task<string> SaveSubmittedAssignmentAsync(IFormFile file, string assignmentName)
        {
            var AssignmentFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "SunimittedAssignment");
            Directory.CreateDirectory(AssignmentFolderPath);

            var fileName = $"{Guid.NewGuid().ToString().Substring(0, 3)}_{assignmentName}{Path.GetExtension(file.FileName)}";

            var assignmentFilePath = Path.Combine(AssignmentFolderPath, fileName);

            using var stream = new FileStream(assignmentFilePath, FileMode.Create);

            await file.CopyToAsync(stream);
            return assignmentFilePath;
        }







    }
}
