using AuthService.Data;
using AuthService.Dtos;
using AuthService.Model;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Service
{
    public class RoleService : IRoleService
    {
        private readonly AuthDbContext _dbContext;

        public RoleService(AuthDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ResponseRoleDto>> GetAllAsync()
        {
            return await _dbContext.Roles
                .Select(r => new ResponseRoleDto
                {
                    Id = r.Id,
                    Name = r.Name
                })
                .ToListAsync();
        }

        public async Task<ResponseRoleDto?> GetByIdAsync(int id)
        {
            return await _dbContext.Roles
                .Where(r => r.Id == id)
                .Select(r => new ResponseRoleDto
                {
                    Id = r.Id,
                    Name = r.Name
                })
                .FirstOrDefaultAsync();
        }

        public async Task<string> CreateAsync(string roleName)
        {
            var exists = await _dbContext.Roles
                .AnyAsync(r => r.Name == roleName);

            if (exists)
                return "Role already exists.";

            var role = new Role
            {
                Name = roleName
            };

            _dbContext.Roles.Add(role);
            await _dbContext.SaveChangesAsync();

            return "Role created successfully.";
        }

        public async Task<string> UpdateAsync(int id, string roleName)
        {
            var role = await _dbContext.Roles.FindAsync(id);

            if (role is null)
                return "Role not found.";

            var exists = await _dbContext.Roles
                .AnyAsync(r => r.Name == roleName && r.Id != id);

            if (exists)
                return "Role already exists.";

            role.Name = roleName;

            await _dbContext.SaveChangesAsync();

            return "Role updated successfully.";
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var role = await _dbContext.Roles.FindAsync(id);

            if (role is null)
                return false;

            _dbContext.Roles.Remove(role);

            await _dbContext.SaveChangesAsync();

            return true;
        }
        public async Task<bool> IsRoleExistsAsync(int roleId)
        {
            return await _dbContext.Roles
                .AnyAsync(r => r.Id==roleId);
        }
    }
}
