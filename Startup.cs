using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Net.Http;
using System.Security.Claims;
using WebApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace WebApi
{
    public class Startup
    {
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            var sessionCookieLifetime = Configuration.GetValue("SessionCookieLifetimeMinutes", 60);
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApi", Version = "v1" });
            });

            services.AddEntityFrameworkSqlServer();
            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(
                   Configuration.GetConnectionString("DefaultConnection")
                   )
            );

            services.AddAuthentication(options =>
            {

                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;


            })
           .AddCookie(setup => setup.ExpireTimeSpan = TimeSpan.FromMinutes(sessionCookieLifetime))
           .AddOpenIdConnect(options =>
           {
               //HttpClientHandler handler = new HttpClientHandler();
               //handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
               //options.BackchannelHttpHandler = handler;
               options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
               options.Authority = Configuration["Authentication:oidc:Authority"];
               options.ClientId = Configuration["Authentication:oidc:ClientId"];
               options.ClientSecret = Configuration["Authentication:oidc:ClientSecret"];
               //options.TokenValidationParameters.NameClaimType = "email";
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   NameClaimType = "preferred_username",
                   RoleClaimType = "roles",
                   ValidateIssuer = true
               };

               options.RequireHttpsMetadata = false;
               options.GetClaimsFromUserInfoEndpoint = true;
               options.SaveTokens = true;
               options.RemoteSignOutPath = "/SignOut";
               options.SignedOutRedirectUri = "Redirect-here";
               options.ResponseType = "code";
               options.RequireHttpsMetadata = false;
               options.Scope.Add("email");

               options.Events = new OpenIdConnectEvents()
               {
                   OnUserInformationReceived = context =>
                   {
                       string rawAccessToken = context.ProtocolMessage.AccessToken;
                       string rawIdToken = context.ProtocolMessage.IdToken;
                       var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                       var accessToken = handler.ReadJwtToken(rawAccessToken);
                       var idToken = handler.ReadJwtToken(rawIdToken);

                       Console.WriteLine("--- rawAccessToken -----");
                       
                       foreach (var claim in accessToken.Claims)
                       {
                           Console.WriteLine($"{claim.Type}\t{claim.Value}");
                       }

                       Console.WriteLine("--- rawIdToken -----");

                       foreach (var claim in idToken.Claims)
                       {
                           Console.WriteLine($"{claim.Type}\t{claim.Value}");
                       }


                       return Task.CompletedTask;
                   },
               };

           });          

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            /*app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });*/

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseSwagger();
                //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi v1"));
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
