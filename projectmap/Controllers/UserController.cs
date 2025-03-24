using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using projectmap.Common;
using projectmap.Service;
using projectmap.ViewModel;

namespace projectmap.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route(nameof(Register))]
        public async Task<PayLoad<RegisterDTO>> Register(RegisterDTO registerDTO)
        {
            return await _userService.Register(registerDTO);
        }

        [HttpPost]
        [Route(nameof(Login))]
        public async Task<PayLoad<ReturnLogin>> Login(RegisterDTO registerDTO)
        {
            return await _userService.Login(registerDTO);
        }
    }
}
