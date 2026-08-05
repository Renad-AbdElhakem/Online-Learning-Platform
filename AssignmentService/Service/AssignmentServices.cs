using AssignmentService.Data;
using AssignmentService.Dtos;
using AssignmentService.ExternalService;
using AssignmentService.Model;
using AssignmentService.Shared;
using Microsoft.EntityFrameworkCore;

namespace AssignmentService.Service
{
    public class AssignmentServices : IAssignmentService
    {
        private readonly GroupClient _groupClient;
        private readonly AssignmentDbContext _dbContext;

        public AssignmentServices(GroupClient groupClient, AssignmentDbContext dbContext)
        {
            _groupClient = groupClient;
            _dbContext = dbContext;
        }

     
        public async Task<GeneralResponse<AssignmentResponseDto>> UploadAssignmentAsync(UploadNewAssignment newAssignment)
        {
            if (newAssignment.AssignmentFile == null || newAssignment.AssignmentFile.Length == 0)
                return GeneralResponse<AssignmentResponseDto>.Failed("No file received.");

            if (!IsAllowedExtension(newAssignment.AssignmentFile))
                return GeneralResponse<AssignmentResponseDto>.Failed("File type not allowed.");

            var group = await _groupClient.GetByIdAsync(newAssignment.GroupId);

            if (!group.IsSuccseded)
                return GeneralResponse<AssignmentResponseDto>.Failed($"Group with id {newAssignment.GroupId} not found or not active.");

            var assignmentLocation = await SaveAssignmentAsync(
                newAssignment.AssignmentFile,
                newAssignment.AssignmentName);

            var assignment = new Assignment
            {
                AssignmentName = newAssignment.AssignmentName,
                AssignmentPath = assignmentLocation,
                Notes = newAssignment.Notes,
                Grade = newAssignment.Grade,
                StartAt = newAssignment.StartAt,
                DurationInDays = newAssignment.DurationInDays,
                EndAt = newAssignment.StartAt.AddDays(newAssignment.DurationInDays),
                IsVisible = newAssignment.IsVisible,
                GroupId = newAssignment.GroupId,
                UploadedAt = DateOnly.FromDateTime(DateTime.Now)
            };

            await _dbContext.Assignments.AddAsync(assignment);
            await _dbContext.SaveChangesAsync();


            var response = new AssignmentResponseDto
            {
                AssignmentName = assignment.AssignmentName,
             
                Notes = assignment.Notes,
                Grade = assignment.Grade,
                StartAt = assignment.StartAt,
                DurationInDays = assignment.DurationInDays,
                EndAt = assignment.EndAt,
                GroupId = assignment.GroupId,
                UploadedAt = assignment.UploadedAt,
              
            };


            return GeneralResponse<AssignmentResponseDto>.Success(response, "Assignment uploaded successfully.");
        }

        public async Task<GeneralResponse<AssignmentResponseDto>> UpdateAssignmentAsync(Guid assignmentId, UpdateAssignmentDto updateAssignment)
        {
            var assignment = await _dbContext.Assignments.FindAsync(assignmentId);

            if (assignment == null)
                return GeneralResponse<AssignmentResponseDto>.Failed($"Assignment with id {assignmentId} not found.");


            if (!string.IsNullOrWhiteSpace(updateAssignment.AssignmentName))
                assignment.AssignmentName = updateAssignment.AssignmentName;


            if (updateAssignment.Notes != null)
                assignment.Notes = updateAssignment.Notes;


            if (updateAssignment.Grade.HasValue)
                assignment.Grade = updateAssignment.Grade.Value;


            if (updateAssignment.StartAt.HasValue)
                assignment.StartAt = updateAssignment.StartAt.Value;


            if (updateAssignment.DurationInDays.HasValue)
                assignment.DurationInDays = updateAssignment.DurationInDays.Value;


        
            if (updateAssignment.StartAt.HasValue || updateAssignment.DurationInDays.HasValue)
            {
                assignment.EndAt = assignment.StartAt.AddDays(assignment.DurationInDays);
            }


            if (updateAssignment.GroupId.HasValue)
            {
                var group = await _groupClient.GetByIdAsync(updateAssignment.GroupId.Value);

                if (!group.IsSuccseded)
                    return GeneralResponse<AssignmentResponseDto>.Failed($"Group with id {updateAssignment.GroupId} not found.");

                assignment.GroupId = updateAssignment.GroupId.Value;
            }


            if (updateAssignment.AssignmentFile != null)
            {
                var assignmentLocation = await SaveAssignmentAsync(
                    updateAssignment.AssignmentFile,
                    assignment.AssignmentName);

                assignment.AssignmentPath = assignmentLocation;
            }


            assignment.ModifiedDate = DateTime.Now;

            await _dbContext.SaveChangesAsync();


            var response = new AssignmentResponseDto
            {
                AssignmentName = assignment.AssignmentName,
              
                Notes = assignment.Notes,
                Grade = assignment.Grade,
                StartAt = assignment.StartAt,
                DurationInDays = assignment.DurationInDays,
                EndAt = assignment.EndAt,
                GroupId = assignment.GroupId,
                UploadedAt = assignment.UploadedAt,
                ModifiedAt = assignment.ModifiedDate
            };


            return GeneralResponse<AssignmentResponseDto>.Success(response, "Updated successfully.");
        }
        public async Task<bool> HideAssignmentAsync(Guid assignmentId)
        {
            var assignment = await _dbContext.Assignments.FindAsync(assignmentId);

            if (assignment is null)
                return false;

            assignment.IsVisible = false;
            assignment.ModifiedDate = DateTime.Now;

            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<GeneralResponse<byte[]>> DownloadAssignmentAsync(Guid assignmentId)
        {
            var assignment = await _dbContext.Assignments.FindAsync(assignmentId);

            if (assignment == null)
                return GeneralResponse<byte[]>.Failed("Assignment not found.");

            if (!File.Exists(assignment.AssignmentPath))
                return GeneralResponse<byte[]>.Failed("File not found.");

            var bytes = await File.ReadAllBytesAsync(assignment.AssignmentPath);

            return GeneralResponse<byte[]>.Success(bytes, "File ready for download.");
        }

        public async Task<GeneralResponse<AssignmentResponseDto>> GetAssignmentNameAndTime(Guid assignmentId)
        {
            var assignment = await _dbContext.Assignments.FindAsync(assignmentId);
            if (assignment == null)
                return GeneralResponse<AssignmentResponseDto>.Failed($"Assignment with id {assignmentId} not found.");

            var assignmentResponse = new AssignmentResponseDto()
            {
                AssignmentName = assignment.AssignmentName,
                EndAt= assignment.EndAt,
                StartAt= assignment.StartAt,
            };
            return GeneralResponse<AssignmentResponseDto>.Success(assignmentResponse);



        }

        public async Task<GeneralResponse<Stream>> StreamAssignmentAsync(Guid assignmentId)
        {
            var assignment = await _dbContext.Assignments.FindAsync(assignmentId);

            if (assignment == null)
                return GeneralResponse<Stream>.Failed("Assignment not found.");

            if (!File.Exists(assignment.AssignmentPath))
                return GeneralResponse<Stream>.Failed("File not found.");

            var stream = File.OpenRead(assignment.AssignmentPath);

            return GeneralResponse<Stream>.Success(stream, "Stream ready.");
        }

        public async Task<bool> DeleteAssignmentAsync(Guid assignmentId)
        {
            var assignment = await _dbContext.Assignments.FindAsync(assignmentId);

            if (assignment == null)
                return false;

            _dbContext.Assignments.Remove(assignment);

            await _dbContext.SaveChangesAsync();

            return true;
        }
        public async Task<bool> AssignmentExistsAsync(Guid assignmentId)
        {
            return await _dbContext.Assignments
                .AnyAsync(a => a.Id == assignmentId);
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


        private async Task<string> SaveAssignmentAsync(IFormFile file, string assignmentName)
        {
            var AssignmentFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "Assignment");
            Directory.CreateDirectory(AssignmentFolderPath);

            var fileName = $"{Guid.NewGuid().ToString().Substring(0, 5)}_{assignmentName}{Path.GetExtension(file.FileName)}";

            var assignmentFilePath = Path.Combine(AssignmentFolderPath, fileName);

            using var stream = new FileStream(assignmentFilePath, FileMode.Create);

            await file.CopyToAsync(stream);
            return assignmentFilePath;
        }
    }
}
