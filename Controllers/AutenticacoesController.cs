using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using CredenciaisService.Controllers.Responses;
using CredenciaisService.Controllers.Requests;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using CredenciaisService.Configurations;
using Microsoft.AspNetCore.Identity;
using CredenciaisService.Entidades;
using SingleSignOn.Configurations;

namespace ServicoCredenciais.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AutenticacoesController : Controller
    {
        private readonly SignInManager<Usuario> signInManager;
        private readonly UserManager<Usuario> userManager;
        private readonly TokenConfiguration tokenConfiguration;
        private readonly SigningConfiguration signingConfiguration;

        public AutenticacoesController(SignInManager<Usuario> signInManager,
            UserManager<Usuario> userManager,
            TokenConfiguration tokenConfiguration,
            SigningConfiguration signingConfiguration)
        {
            this.tokenConfiguration = tokenConfiguration;
            this.signingConfiguration = signingConfiguration;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }


        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AutenticarResponse))]
        public async Task<ObjectResult> AutenticarAsync([FromBody] AutenticarRequest request)
        {
            //todo adicionar validação nas requests
            var usuario = await userManager.FindByNameAsync(request.NomeDeUsuario);
            if (usuario == null) return NotFound("Usuário não encontrado");

            var signinResult = await signInManager
                .CheckPasswordSignInAsync(usuario, request.Senha, false);

            if (!signinResult.Succeeded) return BadRequest("Usuario ou senha inválidos");

            ClaimsIdentity identity = usuario.Identidade();

            String token = CreateSecurityToken(identity);

            return Ok(new AutenticarResponse
            {
                Token = token,
                Mensagem = "OK"
            });
        }

        private string CreateSecurityToken(ClaimsIdentity identity)
        {
            DateTime dataCriacao = DateTime.Now;
            DateTime dataExpiracao = dataCriacao + TimeSpan.FromSeconds(this.tokenConfiguration.Seconds);

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = this.tokenConfiguration.Issuer,
                Audience = this.tokenConfiguration.Audience,
                SigningCredentials = this.signingConfiguration.Credentials(),
                Subject = identity,
                NotBefore = dataCriacao,
                Expires = dataExpiracao
            });
            return handler.WriteToken(token);
        }
    }
}
