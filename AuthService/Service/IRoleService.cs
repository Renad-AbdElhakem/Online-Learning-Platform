using AuthService.Dtos;
using AuthService.Model;

namespace AuthService.Service
{
    public  interface IRoleService
    {
        Task<List<ResponseRoleDto>> GetAllAsync();
        Task<ResponseRoleDto?> GetByIdAsync(int id);
        Task<string> CreateAsync(string roleName);
        Task<string> UpdateAsync(int id, string roleName);
        Task<bool> DeleteAsync(int id);
        Task<bool> IsRoleExistsAsync(int roleId);
    }
}
