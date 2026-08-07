using AuthService.Data;
using AuthService.Dtos;
using AuthService.Model;
using AuthService.Shared;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Service
{
    public class UserAuthService : IUserAuthService
    {
        private readonly AuthDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IRoleService _roleService;

        public UserAuthService(AuthDbContext dbContext, IConfiguration configuration, IRoleService roleService)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _roleService = roleService;
        }



        public async Task<GeneralResponse<ResponseUserAuth>> RegisterationNewUser(RegisterationNewUserDto newUserDto)
        {

            if (await _dbContext.Users.AnyAsync(u => u.Email == newUserDto.Email))
                return GeneralResponse<ResponseUserAuth>.Failed("Email already exists");

            if (!await _roleService.IsRoleExistsAsync(newUserDto.RoleId))
                return GeneralResponse<ResponseUserAuth>.Failed("Role not found");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newUserDto.Password);

            var newUser = new User
            {
                Email = newUserDto.Email,
                HashedPassword = hashedPassword,
                IsEmailConfirmed = newUserDto.IsEmailConfirmed,
                LoggedInAt = DateTime.Now,
                RoleId = newUserDto.RoleId,

            };

            await _dbContext.Users.AddAsync(newUser);
            await _dbContext.SaveChangesAsync();

            var response = new ResponseUserAuth { Email = newUserDto.Email, IsEmailConfirmed = newUserDto.IsEmailConfirmed, RoleId = newUserDto.RoleId };
            return GeneralResponse<ResponseUserAuth>.Success(response, "User registered successfully");
        }


        public async Task<GeneralResponse<string>> LoginUser(LoginUserDto loginDto)
        {
            var user = await _dbContext.Users.Include(r => r.Role).FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user is null)
                return GeneralResponse<string>.Failed("Invalid Data");

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.HashedPassword))
                return GeneralResponse<string>.Failed("Invalid Data");

            var jwtToken = GenerateJwt(user);

            return GeneralResponse<string>.Success(jwtToken);
        }





        private string GenerateJwt(User user)
        {


            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
            new Claim(ClaimTypes.Role,user.Role.Name)
            };


            var key = new SymmetricSecurityKey(
              Encoding.UTF8.GetBytes(
                  _configuration["jwt:SecurityKey"]
                  ?? throw new InvalidOperationException("Could not find SecurityKey")
              )
          );

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["jwt:Issuer"],
                audience: _configuration["jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: cred
            );

            return new JwtSecurityTokenHandler().WriteToken(token);


        }









    }
}
