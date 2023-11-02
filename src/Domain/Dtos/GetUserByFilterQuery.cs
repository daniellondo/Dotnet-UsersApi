using Domain.Models;

namespace Domain.Dtos
{
    public class GetUsersByFilterQuery : QueryBase<BaseResponse<IEnumerable<User>>>
    {
        public int? Age { get; set; }
        public string? Country { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
