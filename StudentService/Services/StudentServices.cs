using Microsoft.EntityFrameworkCore;
using StudentService.Data;
using StudentService.Dtos;
using StudentService.Model;

namespace StudentService.Services
{
    public class StudentServices : IStudentServices
    {
        private readonly StudentDbContext _context;

        public StudentServices(StudentDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StudentResponseDto>> GetAllAsync()
        {
            return await _context.Students
                .Select(s => new StudentResponseDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Email = s.Email,
                    Phone = s.Phone,
                    Country = s.Country,
                    Status = s.Stauts,
                    IsActive = s.IsActive
                })
                .ToListAsync();
        }

        public async Task<StudentResponseDto?> GetByIdAsync(Guid id)
        {
            var s = await _context.Students.FindAsync(id);
            if (s is null) return null;

            return new StudentResponseDto
            {
                Id = s.Id,
                Name = s.Name,
                Email = s.Email,
                Phone = s.Phone,
                Country = s.Country,
                Status = s.Stauts,
                IsActive = s.IsActive
            };
        }

        public async Task<StudentResponseDto> CreateAsync(CreateStudentDto dto)
        {
            var student = new Student
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Country = dto.Country,
                Stauts = "Active",
                IsActive = true
            };

            await _context.AddAsync(student);
            await _context.SaveChangesAsync();

            return new StudentResponseDto
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                Phone = student.Phone,
                Country = student.Country,
                Status = student.Stauts,
                IsActive = student.IsActive
            };
        }

        public async Task<StudentResponseDto?> UpdateStatusAsync(Guid id, UpdateStudentStatusDto dto)
        {
            var student = await _context.Students.FindAsync(id);
            if (student is null) return null;

            student.Stauts = dto.Status;
            student.IsActive = dto.IsActive;
            await _context.SaveChangesAsync();

            return new StudentResponseDto
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                Phone = student.Phone,
                Country = student.Country,
                Status = student.Stauts,
                IsActive = student.IsActive
            };
        }
        public async Task<GeneralResponse<StudentResponseDto>> ActivateAsync(Guid id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student is null) return GeneralResponse<StudentResponseDto>.failed($"Student with id {id} not found");

            if (student.IsActive)
                return GeneralResponse<StudentResponseDto>.failed("Student is already active");

            student.IsActive = true;
            student.Stauts = "Active";
            await _context.SaveChangesAsync();


            var response = new StudentResponseDto
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                Phone = student.Phone,
                Country = student.Country,
                Status = student.Stauts,
                IsActive = student.IsActive
            };
            return GeneralResponse<StudentResponseDto>.Succsess(response);
        }

        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student is null) return false;

            student.IsActive = false;
            student.Stauts = "Inactive";
            await _context.SaveChangesAsync();
            return true;
        }

    }

}
