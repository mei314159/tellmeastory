using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using TellMe.DAL.Contracts.Services;
using TellMe.DAL.Types.Domain;
using TellMe.Web.DTO;

namespace TellMe.Web.Controllers
{
    [Route("api/token")]
    public class TokenController : Controller
    {
        //some config in the appsettings.json  
        private IOptions<Audience> _settings;
        //repository to handler the sqlite database  
        private IUserService _userService;
        private UserManager<ApplicationUser> _userManager;
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
                return Json(new ResponseData
                {
                    Code = "901",
                    Message = "null of parameters",
                    Data = null
                });
            }

            if (parameters.grant_type == "password")
            {
                return Json(await DoPasswordAsync(parameters));
            }
            else if (parameters.grant_type == "refresh_token")
            {
                return Json(DoRefreshToken(parameters));
            }
            else
            {
                return Json(new ResponseData
                {
                    Code = "904",
                    Message = "bad request",
                    Data = null
                });
            }
        }

        //scenario 1 get the access-token by username and password  
        private async Task<ResponseData> DoPasswordAsync(TokenAuthParams parameters)
        {
            //validate the client_id/client_secret/username/password
            var user = await _userManager.FindByEmailAsync(parameters.username);
            //todo validate clientId and client secret
            var isValidated = await _userManager.CheckPasswordAsync(user, parameters.password);

            if (!isValidated)
            {
                return new ResponseData
                {
                    Code = "902",
                    Message = "invalid user infomation",
                    Data = null
                };
            }

            var refresh_token = Guid.NewGuid().ToString().Replace("-", string.Empty);

            var rToken = new RefreshToken
            {
                ClientId = parameters.client_id,
                Token = refresh_token,
                Expired = false,
                UserId = user.Id
            };

            //store the refresh_token   
            if (_userService.AddToken(rToken))
            {
                return new ResponseData
                {
                    Code = "999",
                    Message = "OK",
                    Data = GetJwt(parameters.client_id, refresh_token, user.Id)
                };
            }
            else
            {
                return new ResponseData
                {
                    Code = "909",
                    Message = "can not add token to database",
                    Data = null
                };
            }
        }

        //scenario 2 ï¼š get the access_token by refresh_token  
        private ResponseData DoRefreshToken(TokenAuthParams parameters)
        {
            var token = _userService.GetToken(parameters.refresh_token, parameters.client_id);

            if (token == null)
            {
                return new ResponseData
                {
                    Code = "905",
                    Message = "can not refresh token",
                    Data = null
                };
            }

            if (token.Expired == true)
            {
                return new ResponseData
                {
                    Code = "906",
                    Message = "refresh token has expired",
                    Data = null
                };
            }

            var refresh_token = Guid.NewGuid().ToString().Replace("-", string.Empty);

            token.Expired = true;
            //expire the old refresh_token and add a new refresh_token  
            var updateFlag = _userService.ExpireToken(token);

            var addFlag = _userService.AddToken(new RefreshToken
            {
                ClientId = parameters.client_id,
                Token = refresh_token,
                Expired = false,
                UserId = token.UserId
            });

            if (updateFlag && addFlag)
            {
                return new ResponseData
                {
                    Code = "999",
                    Message = "OK",
                    Data = GetJwt(parameters.client_id, refresh_token, token.UserId)
                };
            }
            else
            {
                return new ResponseData
                {
                    Code = "910",
                    Message = "can not expire token or a new token",
                    Data = null
                };
            }
        }

        //get the jwt token   
        private string GetJwt(string client_id, string refresh_token, string userId)
        {
            var now = DateTime.UtcNow;

            var claims = new Claim[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, client_id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, now.ToUniversalTime().ToString(), ClaimValueTypes.Integer64)
            };

            var symmetricKeyAsBase64 = _settings.Value.Secret;
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);

            var jwt = new JwtSecurityToken(
                issuer: _settings.Value.Iss,
                audience: _settings.Value.Aud,
                claims: claims,
                notBefore: now,
                expires: now.Add(TimeSpan.FromMinutes(2)),
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                expires_in = (int)TimeSpan.FromMinutes(2).TotalSeconds,
                refresh_token = refresh_token,
                user_id = userId
            };

            return JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented });
        }
    }
}