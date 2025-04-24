using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using projectmap.Common;
using projectmap.Service;
using projectmap.ViewModel;

namespace projectmap.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route(nameof(Register))]
        public async Task<PayLoad<RegisterDTO>> Register(RegisterDTO registerDTO)
        {
            return await _userService.Register(registerDTO);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route(nameof(Login))]
        public async Task<PayLoad<ReturnLogin>> Login(RegisterDTO registerDTO)
        {
            return await _userService.Login(registerDTO);
        }

        [HttpPost]
        [Route(nameof(Logout))]
        public async Task<PayLoad<string>> Logout()
        {
            return await _userService.LogOut();
        }

        [HttpGet]
        [Route(nameof(searchName))]
        public async Task<PayLoad<object>> searchName(string? name)
        {
            return await _userService.searchName(name);
        }

        [HttpGet]
        [Route(nameof(searchId))]
        public async Task<PayLoad<object>> searchId(int id)
        {
            return await _userService.searchId(id);
        }
    }
}
