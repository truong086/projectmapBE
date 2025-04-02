using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using projectmap.Common;
using projectmap.Models;
using projectmap.ViewModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace projectmap.Service
{
    public class UserService : IUserService
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        private Jwt _jwt;
        private readonly IUserTokenService _userTokenService;
        public UserService(DBContext context, IOptionsMonitor<Jwt> jwt, IMapper mapper, IUserTokenService userTokenService)
        {
            _context = context;
            _jwt = jwt.CurrentValue;
            _mapper = mapper;
            _userTokenService = userTokenService;
        }
        public async Task<PayLoad<ReturnLogin>> Login(RegisterDTO registerDTO)
        {
            try
            {
                registerDTO.Password = EncryptionHelper.CreatePasswordHash(registerDTO.Password, _jwt.Key);
                var checkData = _context.users.FirstOrDefault(x => x.Name == registerDTO.Name && x.Password == registerDTO.Password && !x.deleted);
                if(checkData == null)
                    return await Task.FromResult(PayLoad<ReturnLogin>.CreatedFail(Status.DATATONTAI));


                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(Status.IDAUTHENTICATION, checkData.id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub, checkData.id.ToString())
                };

                return await Task.FromResult(PayLoad<ReturnLogin>.Successfully(new ReturnLogin
                {
                    id = checkData.id,
                    username = checkData.Name,
                    role = checkData.Identity,
                    Token = GenerateToken(claims)
                }));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<ReturnLogin>.CreatedFail(ex.Message));
            }
        }

        private string GenerateToken(List<Claim>? claim)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creadentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_jwt.Issuer,
                _jwt.Issuer,
                expires: DateTime.Now.AddMinutes(12000),
                claims: claim,
                signingCredentials: creadentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<PayLoad<RegisterDTO>> Register(RegisterDTO registerDTO)
        {
            try
            {
                var checkName = _context.users.FirstOrDefault(x => x.Name == registerDTO.Name && !x.deleted);
                if (checkName != null)
                    return await Task.FromResult(PayLoad<RegisterDTO>.CreatedFail(Status.DATATONTAI));

                registerDTO.Password = EncryptionHelper.CreatePasswordHash(registerDTO.Password, _jwt.Key);

                var mapData = _mapper.Map<User>(registerDTO);
                mapData.Identity = 3;
                mapData.UserStatus = 0;

                _context.users.Add(mapData);
                _context.SaveChanges();

                return await Task.FromResult(PayLoad<RegisterDTO>.Successfully(registerDTO));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<RegisterDTO>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<string>> LogOut()
        {
            _userTokenService.Logout();
            return await Task.FromResult(PayLoad<string>.Successfully(Status.SUCCESS));
        }
    }
}
