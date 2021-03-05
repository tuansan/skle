using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nop.Api.Factories;
using Nop.Api.Infrastructure.Api;
using Nop.Api.Models;
using Nop.LibApi;
using System;

namespace Nop.Api.Controllers
{
    public class AuthenController : BaseRouteController
    {
        private readonly ApiSettings _settingContext;
        private readonly ICommonFactory _commonFactory;

        public AuthenController(
            ApiSettings settingContext,
            ICommonFactory commonFactory)
        {
            _settingContext = settingContext;
            _commonFactory = commonFactory;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Authen([FromBody] ClientApp model)
        {
            var _apiAuthen = _commonFactory.isAuthentication(model, _settingContext.DataConfig);
            if (!string.IsNullOrEmpty(_apiAuthen.ErrorMsg))
            {
                return BadRequest(_apiAuthen.ErrorMsg);
            }
            var time = DateTime.Now.AddMinutes(10);

            return Ok(MessageReturn.Success("Ok", new
            {
                Token = _commonFactory.GenerateJWTToken(0, time)
            }));
        }
    }
}