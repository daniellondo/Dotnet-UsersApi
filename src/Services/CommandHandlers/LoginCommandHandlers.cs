namespace Services.CommandHandlers
{
    using Data;
    using Domain.Dtos;
    using Domain.Models;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class LoginCommandHandlers
    {
        public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, BaseResponse<bool>>
        {
            private readonly IDatabaseConfig _database;

            public RegisterUserCommandHandler(IDatabaseConfig database, IConfiguration configuration)
            {
                _database = database;
                _database.ConnectionString = configuration["ConnectionString"];
            }

            public async Task<BaseResponse<bool>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var query = $"SELECT * FROM Login where UPPER(username) = UPPER('{request.Username}')";
                    var res = await _database.QueryAsync<LoginUser>(query);
                    if (res.Any()) return new BaseResponse<bool>("User already Exist", false, StatusCodes.Status400BadRequest);

                    var insertQuery = $@"
                            INSERT INTO Login 
                                (Username, Password) 
                                VALUES ('{request.Username}','{GetPassword(request.Password)}')";
                    await _database.InsertAsync(insertQuery);
                    return new BaseResponse<bool>("Added successfully!", true);
                }
                catch (Exception ex)
                {
                    return new BaseResponse<bool>(ex.Message + " " + ex.StackTrace, false, StatusCodes.Status500InternalServerError);
                }
            }
            public static string GetPassword(string password)
            {
                return BCrypt.Net.BCrypt.HashPassword(password);
            }
        }
    }
}
