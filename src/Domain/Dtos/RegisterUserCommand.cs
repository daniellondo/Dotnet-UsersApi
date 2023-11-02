namespace Domain.Dtos
{
    public class RegisterUserCommand : CommandBase<BaseResponse<bool>>
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
