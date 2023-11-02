namespace Api.Controllers
{
    using Domain.Dtos;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class LoginController : ControllerBase
    {
        private readonly IMediator _mediator;
        public LoginController(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Register User to Login
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("RegisterUser")]
        public async Task<IActionResult> Post([FromQuery] RegisterUserCommand payload)
        {
            var result = await _mediator.Send(payload);
            return result.Error is null ? Ok(result) : StatusCode((int)result.Error, result);
        }

        /// <summary>
        ///     Login User to get token
        /// </summary>
        /// <param name="payload"></param>
        [HttpGet]
        [Route("Login")]
        public async Task<IActionResult> Login([FromQuery] UserLoginQuery payload)
        {
            var result = await _mediator.Send(payload);
            return result.Error is null ? Ok(result) : StatusCode((int)result.Error, result);
        }
    }
}
