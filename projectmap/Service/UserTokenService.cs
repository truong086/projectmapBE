using Microsoft.AspNetCore.Authentication;
using projectmap.ViewModel;
using System.Security.Claims;

namespace projectmap.Service
{
    public class UserTokenService : IUserTokenService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserTokenService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Logout()
        {
            _httpContextAccessor.HttpContext.SignOutAsync();
        }

        public string name()
        {
            string value = string.Empty;
            if (_httpContextAccessor != null)
            {
                value = _httpContextAccessor.HttpContext.User.FindFirstValue(Status.IDAUTHENTICATION);
            }

            return value;
        }
    }
}
