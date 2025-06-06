﻿using AutoMapper;
using CloudinaryDotNet.Core;
using Microsoft.EntityFrameworkCore;
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
                var checkData = _context.users.Where(x => x.Name == registerDTO.Name && x.Password == registerDTO.Password && !x.deleted).FirstOrDefault();
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
                mapData.Identity = 2;
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

        public async Task<PayLoad<object>> searchName(string? name)
        {
            try
            {
                var checkData = _context.users.Select(x => new
                {
                    id = x.id,
                    name = x.Name,
                    identity = x.Identity,
                    total = x.RepairDetails.Where(x1 => x1.RepairStatus != 4 && x1.RepairStatus != 5).Select(x2 => x2.RepairRecords).Count()
                }).ToList();

                if (!string.IsNullOrEmpty(name))
                    //checkData = checkData.Where(x => x.name.ToLower().Contains(name.ToLower())).ToList();
                    checkData = checkData.Where(x => x.name.Contains(name)).ToList();

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = checkData
                }));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> searchId(int id)
        {
            try
            {
                var checkData = _context.users.Where(x => x.id == id).AsNoTracking().Select(x => new
                {
                    id = x.id,
                    name = x.Name,
                    identity = x.Identity,
                    logandlat = x.RepairDetails.Where(x1 => x1.RepairStatus != 4 && x1.RepairStatus != 5).Select(x1 => new
                    {
                        log = x1.trafficEquipment.Longitude,
                        lat = x1.trafficEquipment.Latitude
                    }).ToList()
                }).FirstOrDefault();

                return await Task.FromResult(PayLoad<object>.Successfully(checkData));
            }catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> CheckToken(string token)
        {
            try
            {
                var user = _userTokenService.name();
                var checkAccount = _context.users.Where(x => x.id == Convert.ToInt32(user) && !x.deleted).FirstOrDefault();
                if (checkAccount == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                var jwtTokenHandler = new JwtSecurityTokenHandler();
                var secretKeyBytes = Encoding.UTF8.GetBytes(_jwt.Key); // Lấy mảng byte
                var Tokenparam = new TokenValidationParameters
                {
                    // Tự cấp token
                    ValidateIssuer = false,
                    ValidateAudience = false,

                    // Ký vào token
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes), // Thuật toán mã hóa token

                    ClockSkew = TimeSpan.Zero,

                    ValidateLifetime = false // Không kiểm tra token hết hạn
                };

                var tokenInverification = jwtTokenHandler.ValidateToken(token, Tokenparam, out var validatedToken);

                var utcExpireDate = long.Parse(tokenInverification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expireDate = ConverunixTimeToDateTime(utcExpireDate);
                var timeRemaining = expireDate - DateTime.UtcNow;

                if (timeRemaining.TotalMinutes <= 10 && timeRemaining.TotalMinutes > 0) // Nếu còn dưới hoặc bằng 10 phút
                {
                    var claims = new List<Claim>
                    {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(Status.IDAUTHENTICATION, checkAccount.id.ToString())
                    };

                    return await Task.FromResult(PayLoad<object>.Successfully(new
                    {
                        token = GenerateToken(claims)
                    }));
                }

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    token = token
                }));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        private DateTime ConverunixTimeToDateTime(long utcExpireDate)
        {
            // Tính thời gian từ năm: 1970
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            dateTimeInterval.AddSeconds(utcExpireDate).ToUniversalTime();

            return dateTimeInterval;
        }

        public async Task<PayLoad<string>> AddToken(string token)
        {
            try
            {
                var user = _userTokenService.name();
                var checkAccount = _context.users.FirstOrDefault(x => x.id == Convert.ToInt32(user));
                if(checkAccount == null)
                    return await Task.FromResult(PayLoad<string>.CreatedFail(Status.DATANULL));

                if (checkAccount.Token == token)
                    return await Task.FromResult(PayLoad<string>.Successfully(Status.SUCCESS));

                checkAccount.Token = token;
                _context.users.Update(checkAccount);
                _context.SaveChanges();


                return await Task.FromResult(PayLoad<string>.Successfully(Status.SUCCESS));

            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<string>.CreatedFail(ex.Message));
            }
        }

        public async Task<PayLoad<object>> GenTokenOld()
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(Status.IDAUTHENTICATION, "1"),
                    new Claim(JwtRegisteredClaimNames.Sub, "1")
                };
                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    token = GenerateTokenOld(claims)
                }));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        private string GenerateTokenOld(List<Claim>? claim)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creadentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_jwt.Issuer,
                _jwt.Issuer,
                notBefore: DateTime.UtcNow.AddMinutes(-20),  // Token có hiệu lực từ 20 phút trước
                expires: DateTime.Now.AddMinutes(-10), // Hết hạn từ 10 phút trước
                claims: claim,
                signingCredentials: creadentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<PayLoad<object>> searchNameByUser(string? name)
        {
            try
            {
                var use = _userTokenService.name();
                var checkData = _context.users.Where(x => x.id == int.Parse(use)).Select(x => new
                {
                    id = x.id,
                    name = x.Name,
                    identity = x.Identity,
                    total = x.RepairDetails.Where(x1 => x1.RepairStatus != 4 && x1.RepairStatus != 5).Select(x2 => x2.RepairRecords).Count()
                }).ToList();

                if (!string.IsNullOrEmpty(name))
                    //checkData = checkData.Where(x => x.name.ToLower().Contains(name.ToLower())).ToList();
                    checkData = checkData.Where(x => x.name.Contains(name)).ToList();

                return await Task.FromResult(PayLoad<object>.Successfully(new
                {
                    data = checkData
                }));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }
    }
}
