using AuthService.Dtos;
using AuthService.Shared;

namespace AuthService.Service
{
    public interface IUserAuthService
    {
        Task<GeneralResponse<ResponseUserAuth>> RegisterationNewUser(RegisterationNewUserDto newUserDto);
        Task<GeneralResponse<string>> LoginUser(LoginUserDto loginDto);
    }
}
