using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using TellMe.Web.DTO;
using AutoMapper;
using TellMe.Web.DAL.Contracts.Services;
using TellMe.Web.DAL.DTO;
using TellMe.Web.DAL.Types.Domain;

namespace TellMe.Web.Controllers
{
    [Route("api/token")]
    public class TokenController : Controller
    {
        private readonly IOptions<Audience> _settings;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        public TokenController(IOptions<Audience> settings, IUserService userService, UserManager<ApplicationUser> userManager)
        {
            this._settings = settings;
            this._userService = userService;
            this._userManager = userManager;
        }

        [HttpPost("auth")]
        public async Task<IActionResult> AuthAsync(TokenAuthParams parameters)
        {
            if (parameters == null)
            {
                return BadRequest(new ResponseData
                {
                    Code = "901",
                    Message = "null of parameters",
                    Data = null
                });
            }

            switch (parameters.grant_type)
            {
                case "password":
                    return await DoPasswordAsync(parameters);
                case "refresh_token":
                    return await DoRefreshToken(parameters);
                default:
                    return BadRequest(new ResponseData
                    {
                        Code = "904",
                        Message = "bad request",
                        Data = null
                    });
            }
        }

        private async Task<IActionResult> DoPasswordAsync(TokenAuthParams parameters)
        {
            //validate the client_id/client_secret/username/password
            var user = await _userManager.FindByEmailAsync(parameters.username);
            //todo validate clientId and client secret
            var isValidated = await _userManager.CheckPasswordAsync(user, parameters.password);

            if (!isValidated)
            {
                return BadRequest(new ResponseData
                {
                    Code = "902",
                    Message = "invalid user infomation",
                    Data = null
                });
            }

            var refreshToken = Guid.NewGuid().ToString().Replace("-", string.Empty);

            var rToken = new RefreshToken
            {
                ClientId = parameters.client_id,
                Token = refreshToken,
                Expired = false,
                UserId = user.Id
            };

            //store the refresh_token   
            var success = await _userService.AddTokenAsync(rToken);
            if (success)
            {
                return Ok(GetJwt(refreshToken, user));
            }
            
            return BadRequest(new ResponseData
            {
                Code = "909",
                Message = "can not add token to database",
                Data = null
            });
        }
        
        private async Task<IActionResult> DoRefreshToken(TokenAuthParams parameters)
        {
            var token = await _userService.GetTokenAsync(parameters.refresh_token, parameters.client_id);
            if (token == null)
            {
                return BadRequest(new ResponseData
                {
                    Code = "905",
                    Message = "can not refresh token",
                    Data = null
                });
            }

            if (token.Expired)
            {
                return BadRequest(new ResponseData
                {
                    Code = "906",
                    Message = "refresh token has expired",
                    Data = null
                });
            }

            var refreshToken = Guid.NewGuid().ToString().Replace("-", string.Empty);

            token.Expired = true;
            //expire the old refresh_token and add a new refresh_token  
            var updateFlag = await _userService.ExpireTokenAsync(token);
            var addFlag = await _userService.AddTokenAsync(new RefreshToken
            {
                ClientId = parameters.client_id,
                Token = refreshToken,
                Expired = false,
                UserId = token.UserId
            });

            if (updateFlag && addFlag)
            {
                var user = await _userManager.FindByIdAsync(token.UserId);
                return Ok(GetJwt(refreshToken, user));
            }
            
            return BadRequest(new ResponseData
            {
                Code = "910",
                Message = "can not expire token or a new token",
                Data = null
            });
        }

        //get the jwt token   
        private string GetJwt(string refreshToken, ApplicationUser user)
        {
            var utcNow = DateTime.UtcNow;

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, utcNow.ToUniversalTime().ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64)
            };

            var symmetricKeyAsBase64 = _settings.Value.Secret;
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);

            var expiresIn = utcNow.Add(TimeSpan.FromMinutes(2));
            var jwt = new JwtSecurityToken(
                issuer: _settings.Value.Iss,
                audience: _settings.Value.Aud,
                claims: claims,
                notBefore: utcNow,
                expires: expiresIn,
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                expires_in = (int)TimeSpan.FromMinutes(2).TotalSeconds,
                expires_at = expiresIn,
                refresh_token = refreshToken,
                user_id = user.Id,
                account = Mapper.Map<UserDTO>(user)
            };

            return JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented });
        }
    }
}