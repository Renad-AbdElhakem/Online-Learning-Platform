using GroupsCourseService.Data;
using GroupsCourseService.Dtos;
using GroupsCourseService.ExternalService;
using GroupsCourseService.Model;
using Microsoft.EntityFrameworkCore;

namespace GroupsCourseService.Service
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly GroupsCourseDbContext _context;
        private readonly IGroupService _groupService;
        private readonly StudentClient _studentClient;

        public EnrollmentService(GroupsCourseDbContext context, IGroupService groupService, StudentClient studentClient)
        {
            _context = context;
            _groupService = groupService;
            _studentClient = studentClient;
        }

        public async Task<List<EnrollmentResponseDto>> GetAllAsync()
        {
            return await _context.Enrollments
                .Select(e => new EnrollmentResponseDto
                {
                    Id = e.Id,
                    StudentId = e.StudentId,
                    GroupId = e.GroupId,
                    EnrollmentDate = e.EnrollmentDate,
                    Status = e.Status,
                    PaymentStatus = e.PaymentStatus
                })
                .ToListAsync();
        }

        public async Task<EnrollmentResponseDto?> GetByIdAsync(Guid id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null) return null;

            return new EnrollmentResponseDto
            {
                Id = enrollment.Id,
                StudentId = enrollment.StudentId,
                GroupId = enrollment.GroupId,
                EnrollmentDate = enrollment.EnrollmentDate,
                Status = enrollment.Status,
                PaymentStatus = enrollment.PaymentStatus
            };
        }

        public async Task<GeneralResponse<EnrollmentResponseDto>> CreateAsync(CreateEnrollmentDto dto)
        {
            var studentResponse = await _studentClient.GetByIdAsync(dto.StudentId);
            if (!studentResponse.IsSuccseded)
                GeneralResponse<EnrollmentResponseDto>.failed($"Student with id {dto.StudentId} not found");


            var group = await _groupService.GetByIdAsync(dto.GroupId);
            if (group is null)
                GeneralResponse<EnrollmentResponseDto>.failed($"Group  with id {dto.GroupId} not found");

            var enrollment = new Enrollment
            {

                StudentId = dto.StudentId,
                GroupId = dto.GroupId,
                EnrollmentDate = DateOnly.FromDateTime(DateTime.UtcNow),
                Status = "Active",
                PaymentStatus = dto.PaymentStatus
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            var response = new EnrollmentResponseDto
            {
                Id = enrollment.Id,
                StudentId = enrollment.StudentId,
                GroupId = enrollment.GroupId,
                EnrollmentDate = enrollment.EnrollmentDate,
                Status = enrollment.Status,
                PaymentStatus = enrollment.PaymentStatus
            };

            return GeneralResponse<EnrollmentResponseDto>.Succsess(response, "Added");
        }

        public async Task<EnrollmentResponseDto?> UpdateGroupAsync(Guid id, UpdateEnrollmentDto dto)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null) return null;

            enrollment.GroupId = dto.GroupId;
            await _context.SaveChangesAsync();

            return new EnrollmentResponseDto
            {
                Id = enrollment.Id,
                StudentId = enrollment.StudentId,
                GroupId = enrollment.GroupId,
                EnrollmentDate = enrollment.EnrollmentDate,
                Status = enrollment.Status,
                PaymentStatus = enrollment.PaymentStatus
            };
        }

        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment is null) return false;

            enrollment.Status = "Completed";
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
