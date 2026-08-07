using AuthService.Dtos;
using AuthService.Service;

namespace AuthService.Api
{
    public static class UserAuthEndPoints
    {

        public static void MapUserAuthEndPoints(this WebApplication app)
        {

            var group = app.MapGroup("/api/UserAuth").WithTags("User Authentication");

            group.MapPost("/register", async (RegisterationNewUserDto dto, IUserAuthService authService) =>
            {
                var result = await authService.RegisterationNewUser(dto);

                return result.IsSuccseded ? Results.Ok(result) : Results.BadRequest(result);
            });

            group.MapPost("/login", async (LoginUserDto dto, IUserAuthService authService) =>
            {
                var result = await authService.LoginUser(dto);

                return result.IsSuccseded ? Results.Ok(result) : Results.Unauthorized();
            });
        }
    }
}

