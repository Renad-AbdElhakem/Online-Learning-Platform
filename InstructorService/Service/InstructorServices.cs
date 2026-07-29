using InstructorService.Data;
using InstructorService.Dtos;
using InstructorService.Model;
using Microsoft.EntityFrameworkCore;

namespace InstructorService.Service
{
    public class InstructorServices : IInstructorService
    {
        private readonly InstructorDbContext _context;

        public InstructorServices(InstructorDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<InstructorDto>> GetAllAsync()
        {
            return await _context.Instructors
                .Where(x => x.Stauts == "Active")
                .Select(x => new InstructorDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Email = x.Email,
                    Phone = x.Phone,
                    Major = x.Major,
                    Bio = x.Bio,
                    Status = x.Stauts,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate
                })
                .ToListAsync();
        }

        public async Task<InstructorDto?> GetByIdAsync(Guid id)
        {
            return await _context.Instructors
                .Where(x => x.Id == id && x.Stauts == "Active")
                .Select(x => new InstructorDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Email = x.Email,
                    Phone = x.Phone,
                    Major = x.Major,
                    Bio = x.Bio,
                    Status = x.Stauts,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate
                })
                .FirstOrDefaultAsync();
        }

        public async Task<InstructorDto> CreateAsync(CreateInstructorDto dto)
        {
            var instructor = new Instructor
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Major = dto.Major,
                Bio = dto.Bio,
                StartDate = dto.StartDate,
                Stauts = "Active"
            };

            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();

            return new InstructorDto
            {
                Id = instructor.Id,
                Name = instructor.Name,
                Email = instructor.Email,
                Phone = instructor.Phone,
                Major = instructor.Major,
                Bio = instructor.Bio,
                Status = instructor.Stauts,
                StartDate = instructor.StartDate,
                EndDate = instructor.EndDate
            };
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateInstructorDto dto)
        {
            var instructor = await _context.Instructors.FindAsync(id);

            if (instructor == null || instructor.Stauts == "Inactive")
                return false;

            if (!string.IsNullOrEmpty(dto.Name))
                instructor.Name = dto.Name;
            
            if (!string.IsNullOrEmpty(dto.Email))
                instructor.Email = dto.Email;
            
            if (!string.IsNullOrEmpty(dto.Phone))
                instructor.Phone = dto.Phone;
            
            if (!string.IsNullOrEmpty(dto.Major))
                instructor.Major = dto.Major;
            
            if (!string.IsNullOrEmpty(dto.Bio))
                instructor.Bio = dto.Bio;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateInstructorStatusAsync(Guid id, DeleteInstructorDto dto)
        {
            var instructor = await _context.Instructors.FindAsync(id);

            if (instructor == null || instructor.Stauts == "Inactive")
                return false;

            instructor.Stauts = dto.Status;
          
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> DeleteAsync(Guid id)
        {
            var instructor = await _context.Instructors.FindAsync(id);

            if (instructor == null || instructor.Stauts == "Inactive")
                return false;

          
            instructor.EndDate = DateOnly.FromDateTime(DateTime.UtcNow);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
