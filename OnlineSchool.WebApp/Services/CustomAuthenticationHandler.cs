using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace OnlineSchool.WebApp.Services
{
    public class CustomAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public CustomAuthenticationHandler(
            AuthenticationStateProvider authenticationStateProvider,
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder) : base(options, logger, encoder)
        {
            _authenticationStateProvider = authenticationStateProvider;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Получаем состояние из нашего кастомного провайдера
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            // Если пользователь аутентифицирован, создаем успешный тикет
            if (user.Identity?.IsAuthenticated == true)
            {
                var ticket = new AuthenticationTicket(user, Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }

            // Если нет, сообщаем об отсутствии результата
            return AuthenticateResult.NoResult();
        }

        // Этот метод вызывается, когда авторизация не пройдена
        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            // Перенаправляем на страницу входа
            Response.Redirect("/login");
            return Task.CompletedTask;
        }
    }
}