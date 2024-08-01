using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace cepedi_mvc_keycloak.Configurations
{
    public static class KeycloakConfig
    {
        public static IServiceCollection ConfigurarKeycloak(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(opt =>
            {
                opt.ExpireTimeSpan = TimeSpan.FromMinutes(1);
            })
            .AddOpenIdConnect(options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Authority = $"{configuration["Keycloak:auth-server-url"]}realms/{configuration["Keycloak:realm"]}";
                options.ClientId = configuration["Keycloak:resource"];
                options.ClientSecret = configuration["Keycloak:credentials:secret"];
                options.ResponseType = OpenIdConnectResponseType.CodeIdTokenToken;
                options.UsePkce = true;
                options.Scope.Add("profile");
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters { RoleClaimType = "roles" };
            });



            return services;
        }
    }
}
