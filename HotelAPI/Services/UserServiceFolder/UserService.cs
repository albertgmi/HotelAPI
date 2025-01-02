using AutoMapper;
using HotelAPI.Entities;
using HotelAPI.Exceptions;
using HotelAPI.JWTstuff;
using HotelAPI.Models.UserModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HotelAPI.Services.UserServiceFolder
{
    public class UserService : IUserService
    {
        private readonly HotelDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly AuthenticationSettings _authenticationSettings;
        public UserService(HotelDbContext dbContext, IMapper mapper, IUserContextService userContextService, IPasswordHasher<User> passwordHasher, AuthenticationSettings authenticationSettings)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _userContextService = userContextService;
            _passwordHasher = passwordHasher;
            _authenticationSettings = authenticationSettings;
        }
        public void RegisterUser(RegisterUserDto registerUserDto)
        {
            var user = new User()
            {
                FirstName = registerUserDto.FirstName,
                LastName = registerUserDto.LastName,
                Email = registerUserDto.Email,
                DateOfBirth = registerUserDto.DateOfBirth,
                RoleId = 1
            };
            var passwordHashed = _passwordHasher.HashPassword(user, registerUserDto.Password);
            user.PasswordHash = passwordHashed;
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
        }
        public string GenerateJwt(LoginUserDto loginDto)
        {
            var user = _dbContext
                .Users
                .Include(x => x.Role)
                .FirstOrDefault(x => x.Email == loginDto.Email);
            if (user is null)
                throw new BadRequestException("Invalid username or password");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
            if (result == PasswordVerificationResult.Failed)
                throw new BadRequestException("Invalid username or password");

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, $"{user.Role.Name}"),
                new Claim("DateOfBirth", user.DateOfBirth.Value.ToString("yyyy-MM-dd"))
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(_authenticationSettings.JwtExpireDays);

            var token = new JwtSecurityToken(_authenticationSettings.JwtIssuer,
                _authenticationSettings.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: cred);
            var tokenhandler = new JwtSecurityTokenHandler();
            return tokenhandler.WriteToken(token);
        }
        public void MakeAdmin(int userId)
        {
            var user = _dbContext
                .Users
                .Include(x => x.Role)
                .FirstOrDefault(x => x.Id == userId);
            if (user is null)
                throw new NotFoundException("User not found");

            var adminRole = _dbContext
                .Roles
                .FirstOrDefault(x => x.Name == "Admin");
            if (adminRole is null)
                throw new NotFoundException("Role not found");

            if (user.Role.Name != "Admin")
                user.Role = adminRole;

            _dbContext.SaveChanges();
        }
        public void MakeManager(int userId)
        {
            var user = _dbContext
                .Users
                .Include(x => x.Role)
                .FirstOrDefault(x => x.Id == userId);
            if (user is null)
                throw new NotFoundException("User not found");

            var managerRole = _dbContext
                .Roles
                .FirstOrDefault(x => x.Name == "Manager");
            if (managerRole is null)
                throw new NotFoundException("Role not found");

            if (user.Role.Name != "Manager")
                user.Role = managerRole;

            _dbContext.SaveChanges();
        }
    }
}
