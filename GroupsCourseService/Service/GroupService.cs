using GroupsCourseService.Data;
using GroupsCourseService.Dtos;
using GroupsCourseService.ExternalService;
using GroupsCourseService.Model;
using Microsoft.EntityFrameworkCore;

namespace GroupsCourseService.Service
{
    public class GroupService : IGroupService
    {
        private readonly GroupsCourseDbContext _context;
        private readonly CourseClient _courseClient;
        private readonly InstructorClient _instructorClient;

        public GroupService(GroupsCourseDbContext context, CourseClient courseClient, InstructorClient instructorClient)
        {
            _context = context;
            _courseClient = courseClient;
            _instructorClient = instructorClient;
        }


        public async Task<GeneralResponse<ResponseGroupDto>> CreateNewGroup(RequestCreateNewGroup dto)
        {

            var instructor = await _instructorClient.GetByIdAsync(dto.InstructorId);
            if (!instructor.IsSuccseded)
                return GeneralResponse<ResponseGroupDto>.failed($"{instructor}");

            var course = await _courseClient.GetByIdAsync(dto.CourseId);
            if (!course.IsSuccseded)
                return GeneralResponse<ResponseGroupDto>.failed($"{course}");

            var newGroup = new Group
            {
                InstructorId = dto.InstructorId,
                CourseId = dto.CourseId,
                NumberOfStudentAllow = dto.NumberOfStudentAllow,
                Description = dto.Description,
                IsActive = dto.IsActive,
                GroupName = dto.GroupName,
                StartDate = dto.StartDate,
                Statuts = dto.Statuts,
                EndDate = dto.EndDate,
            };

            await _context.AddAsync(newGroup);
            await _context.SaveChangesAsync();

            var responseGroup = new ResponseGroupDto
            {
                Id = newGroup.Id,
                CurrentStudentCount = newGroup.CurrentStudentCount,
                InstructorId = newGroup.InstructorId,
                CourseId = newGroup.CourseId,
                NumberOfStudentAllow = newGroup.NumberOfStudentAllow,
                Description = newGroup.Description,
                IsActive = newGroup.IsActive,
                GroupName = newGroup.GroupName,
                StartDate = newGroup.StartDate,
                Statuts = newGroup.Statuts,
                EndDate = newGroup.EndDate,
            };

            return GeneralResponse<ResponseGroupDto>.Succsess(responseGroup);

        }

        public async Task<List<ResponseGroupDto>> GetAllAsync()
        {
            var groups = await _context.Groups
                .Select(group => new ResponseGroupDto
                {
                    Id = group.Id,
                    GroupName = group.GroupName,
                    Description = group.Description,
                    CourseId = group.CourseId,
                    InstructorId = group.InstructorId,
                    CurrentStudentCount = group.CurrentStudentCount,
                    NumberOfStudentAllow = group.NumberOfStudentAllow,
                    StartDate = group.StartDate,
                    EndDate = group.EndDate,
                    IsActive = group.IsActive,
                    Statuts = group.Statuts
                })
                .ToListAsync();

            if (!groups.Any())
                return new List<ResponseGroupDto>();

            return groups;
        }

        public async Task<GeneralResponse<ResponseGroupDto>> GetByIdAsync(Guid id)
        {
            var group = await _context.Groups
                .FirstOrDefaultAsync(g => g.Id == id 
                                        && g.IsActive 
                                        && g.Statuts.Contains("Active"));

            if (group is null)
                return GeneralResponse<ResponseGroupDto>
                    .failed("Group not found.");

            var response = new ResponseGroupDto
            {
                Id = group.Id,
                GroupName = group.GroupName,
                Description = group.Description,
                CourseId = group.CourseId,
                InstructorId = group.InstructorId,
                CurrentStudentCount = group.CurrentStudentCount,
                NumberOfStudentAllow = group.NumberOfStudentAllow,
                StartDate = group.StartDate,
                EndDate = group.EndDate,
                IsActive = group.IsActive,
                Statuts = group.Statuts
            };

            return GeneralResponse<ResponseGroupDto>
                .Succsess(response);
        }
        public async Task<GeneralResponse<ResponseGroupDto>> CompleteGroupAsync(Guid id)
        {
            var group = await _context.Groups
                .FirstOrDefaultAsync(g => g.Id == id);

            if (group is null)
                return GeneralResponse<ResponseGroupDto>
                    .failed($"Group with id {id} not found.");

            group.IsActive = false;
            group.Statuts = "Completed";
            group.EndDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var response = new ResponseGroupDto
            {
                Id = group.Id,
                GroupName = group.GroupName,
                Description = group.Description,
                CourseId = group.CourseId,
                InstructorId = group.InstructorId,
                CurrentStudentCount = group.CurrentStudentCount,
                NumberOfStudentAllow = group.NumberOfStudentAllow,
                StartDate = group.StartDate,
                EndDate = group.EndDate,
                IsActive = group.IsActive,
                Statuts = group.Statuts
            };

            return GeneralResponse<ResponseGroupDto>
                .Succsess(response, "Group completed successfully.");
        }

        public async Task<GeneralResponse<ResponseGroupDto>> UpdateStatusGroupAsync(Guid id, UpdateGroupStatusDto statusDto)
        {
            var group = await _context.Groups
                .FirstOrDefaultAsync(g => g.Id == id);

            if (group == null)
                return GeneralResponse<ResponseGroupDto>.failed($"Group with id {id} not found.");

            group.IsActive = statusDto.IsActive;
            group.Statuts = statusDto.Status;
            group.EndDate = statusDto.EndDate;

            await _context.SaveChangesAsync();

            var response = new ResponseGroupDto
            {
                Id = group.Id,
                GroupName = group.GroupName,
                Description = group.Description,
                CourseId = group.CourseId,
                InstructorId = group.InstructorId,
                CurrentStudentCount = group.CurrentStudentCount,
                NumberOfStudentAllow = group.NumberOfStudentAllow,
                StartDate = group.StartDate,
                EndDate = group.EndDate,
                IsActive = group.IsActive,
                Statuts = group.Statuts
            };

            return GeneralResponse<ResponseGroupDto>
                .Succsess(response, "Group status changed to pending.");
        }

        public async Task<GeneralResponse<ResponseGroupDto>> UpdateGroupAssignmentAsync(Guid id, UpdateGroupAssignmentDto dto)
        {
            var group = await _context.Groups
                .FirstOrDefaultAsync(g => g.Id == id);

            if (group == null)
                return GeneralResponse<ResponseGroupDto>
                    .failed("Group not found");

            if (dto.InstructorId.HasValue)
            {
                var instructor = await _instructorClient
                    .GetByIdAsync(dto.InstructorId.Value);

                if (!instructor.IsSuccseded)
                    return GeneralResponse<ResponseGroupDto>
                        .failed("Instructor not found");

                group.InstructorId = dto.InstructorId.Value;
            }


            if (dto.CourseId.HasValue)
            {
                var course = await _courseClient
                    .GetByIdAsync(dto.CourseId.Value);

                if (!course.IsSuccseded)
                    return GeneralResponse<ResponseGroupDto>
                        .failed("Course not found");

                group.CourseId = dto.CourseId.Value;
            }


            await _context.SaveChangesAsync();

            var response = new ResponseGroupDto
            {
                Id = group.Id,
                GroupName = group.GroupName,
                Description = group.Description,
                CourseId = group.CourseId,
                InstructorId = group.InstructorId,
                CurrentStudentCount = group.CurrentStudentCount,
                NumberOfStudentAllow = group.NumberOfStudentAllow,
                StartDate = group.StartDate,
                EndDate = group.EndDate,
                IsActive = group.IsActive,
                Statuts = group.Statuts
            };

            return GeneralResponse<ResponseGroupDto>
                .Succsess(response, "Updated");
        }
    }
}
