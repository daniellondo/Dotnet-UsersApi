namespace Services.QueryHandlers
{
    using Data;
    using Domain.Dtos;
    using Domain.Models;
    using MediatR;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class UsersQueryHandlers
    {
        public class GetTaskQueryHandler : IRequestHandler<GetUsersByFilterQuery, BaseResponse<IEnumerable<User>>>
        {
            private readonly IDatabaseConfig _database;

            public GetTaskQueryHandler(IDatabaseConfig database, IConfiguration configuration)
            {
                _database = database;
                _database.ConnectionString = configuration["ConnectionString"];
            }

            public async Task<BaseResponse<IEnumerable<User>>> Handle(GetUsersByFilterQuery payload, CancellationToken cancellationToken)
            {
                try
                {
                    var query = $"SELECT * FROM Users ORDER BY ID OFFSET {GetOffset(payload.PageSize, payload.PageNumber)} ROWS FETCH NEXT {payload.PageSize} ROWS ONLY";
                    if (payload.Age is not null)
                    {
                        query = $"Select * From Users Where Age = {payload.Age} ORDER BY ID OFFSET {GetOffset(payload.PageSize, payload.PageNumber)} ROWS FETCH NEXT {payload.PageSize} ROWS ONLY";
                    }

                    if (payload.Country is not null)
                    {
                        query = $"Select * From Users Where UPPER(Country) = UPPER('{payload.Country}') ORDER BY ID OFFSET {GetOffset(payload.PageSize, payload.PageNumber)} ROWS FETCH NEXT {payload.PageSize} ROWS ONLY";
                    }

                    var res = await _database.QueryAsync<User>(query);
                    return new BaseResponse<IEnumerable<User>>("", res);
                }
                catch (Exception ex)
                {
                    return new BaseResponse<IEnumerable<User>>("Error getting data", null, ex);
                }
            }

            private static int GetOffset(int pageSize, int pageNumber)
            {
                return (pageNumber - 1) * pageSize;
            }
        }
    }
}
