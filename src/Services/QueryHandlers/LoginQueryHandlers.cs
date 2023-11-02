using Data;
using Domain.Dtos;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services.QueryHandlers
{
    public class LoginQueryHandlers
    {
        public class UserLoginQueryHandler : IRequestHandler<UserLoginQuery, BaseResponse<string>>
        {
            private readonly IDatabaseConfig _database;
            private readonly IConfiguration _configuration;

            public UserLoginQueryHandler(IDatabaseConfig database, IConfiguration configuration)
            {
                _database = database;
                _configuration = configuration;
                _database.ConnectionString = _configuration["ConnectionString"];
            }

            public async Task<BaseResponse<string>> Handle(UserLoginQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var query = $"SELECT * FROM Login where UPPER(username) = UPPER('{request.Username}')";
                    var res = await _database.QueryAsync<LoginUser>(query);
                    if (!res.Any()) return new BaseResponse<string>("User not Exist", string.Empty, StatusCodes.Status400BadRequest);

                    if (VerifyPassword(request.Password, res.First().Password))
                    {
                        return new BaseResponse<string>("Token Generated", GenerateToken(res.First().Password));
                    }
                    return new BaseResponse<string>("Incorrect Password", string.Empty, StatusCodes.Status400BadRequest);
                }
                catch (Exception ex)
                {
                    return new BaseResponse<string>(ex.Message + " " + ex.StackTrace, string.Empty, StatusCodes.Status500InternalServerError);
                }
            }

            private string GenerateToken(string username)
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var claims = new[]
                {
                        new Claim(ClaimTypes.NameIdentifier,username),
                        new Claim(ClaimTypes.Role,"admin")
                    };
                var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.Now.AddMinutes(15),
                    signingCredentials: credentials);
                var response = new JwtSecurityTokenHandler().WriteToken(token);
                return response;
            }

            public static bool VerifyPassword(string password, string hashedPassword)
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
        }
    }
}
