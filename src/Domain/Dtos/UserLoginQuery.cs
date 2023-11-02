namespace Domain.Dtos
{
    public class UserLoginQuery : QueryBase<BaseResponse<string>>
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
