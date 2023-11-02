namespace Services.CommandHandlers
{
    using Bogus;
    using Data;
    using Domain.Dtos;
    using Domain.Models;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class PreloadCommandHandlers
    {
        public class PreloadCommandHandler : IRequestHandler<PreloadCommand, BaseResponse<bool>>
        {
            private readonly IDatabaseConfig _database;

            public PreloadCommandHandler(IDatabaseConfig database, IConfiguration configuration)
            {
                _database = database;
                _database.ConnectionString = configuration["ConnectionString"];
            }

            public async Task<BaseResponse<bool>> Handle(PreloadCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var userList = new Faker<User>()
                                    .RuleFor(x => x.FirstName, faker => faker.Name.FirstName())
                                    .RuleFor(x => x.LastName, faker => faker.Name.LastName())
                                    .RuleFor(x => x.Country, faker => faker.Address.Country())
                                    .RuleFor(x => x.City, faker => faker.Address.City())
                                    .RuleFor(x => x.Province, faker => faker.Address.State())
                                    .RuleFor(x => x.Age, faker => faker.Random.Int(18, 65))
                                    .RuleFor(x => x.BirthDate, (faker, user) => DateTime.Now.AddYears(-user.Age))
                                    .GenerateLazy(1000);
                    await _database.InsertListAsync($@"
                            INSERT INTO Users 
                                (FirstName, LastName, Country, City, Province, Age, BirthDate) 
                                VALUES (@FirstName,@LastName,@Country,@City,@Province,@Age,@BirthDate)", userList);
                    return new BaseResponse<bool>($"Inserted!", true);
                }
                catch (Exception ex)
                {
                    return new BaseResponse<bool>(ex.Message + " " + ex.StackTrace, false, StatusCodes.Status500InternalServerError);
                }
            }

        }
    }
}
