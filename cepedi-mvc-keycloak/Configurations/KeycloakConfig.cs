using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Security.Claims;

namespace cepedi_mvc_keycloak.Configurations
{
    public static class KeycloakConfig
    {
        public static IServiceCollection ConfigurarKeycloak(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
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
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    NameClaimType = "preferred_username",
                    RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
                };
                options.Events = new OpenIdConnectEvents
                {
                    OnTokenValidated = context =>
                    {
                        var claimsIdentity = (ClaimsIdentity)context.Principal.Identity;

                        var roleClaims = context.Principal.FindAll("http://schemas.microsoft.com/ws/2008/06/identity/claims/role");

                        var newRoleClaims = roleClaims.Select(roleClaim =>
                                new Claim(ClaimsIdentity.DefaultRoleClaimType, roleClaim.Value)).ToList();

                        claimsIdentity.AddClaims(newRoleClaims);

                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }
    }
}
