namespace API.Controllers
{
    using Domain.Dtos;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Pre Load 1000 Users
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("PreloadUsers")]
        public async Task<IActionResult> Post()
        {
            var result = await _mediator.Send(new PreloadCommand());
            return result.Error is null ? Ok(result) : StatusCode((int)result.Error, result);
        }

        /// <summary>
        /// Get Users by Age or by Country paginated
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetUsers")]
        public async Task<IActionResult> Get([FromQuery] GetUsersByFilterQuery payload)
        {
            var result = await _mediator.Send(payload);
            return result.Error is null ? Ok(result) : StatusCode((int)result.Error, result);
        }
    }
}
