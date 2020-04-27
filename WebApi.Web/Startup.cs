using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Threading.Tasks;
using WebApi.Core.Entities;
using WebApi.Core.Interfaces;
using WebApi.Core.Services;
using WebApi.Infrastructure.Data;
using WebApi.Infrastructure.Identity;
using WebApi.Web.Helpers.Exceptions;

namespace WebApi.Web
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllers();

      var sqlConnectionString = Configuration.GetConnectionString("SqlConnection");
      services.AddDbContext<AppDbContext>(options => options.UseSqlServer(sqlConnectionString));

      var identityConnectionString = Configuration.GetConnectionString("IdentityConnection");
      services.AddDbContext<IdentityDbContext>(options => options.UseSqlServer(identityConnectionString));

      var redisConnectionString = Configuration.GetConnectionString("RedisConnection");
      services.AddDistributedRedisCache(options =>
      {
        options.Configuration = redisConnectionString;
        options.InstanceName = "master";
      });

      var appSettingsSection = Configuration.GetSection("Jwt");
      services.Configure<TokenSettings>(appSettingsSection);

      var appSettings = appSettingsSection.Get<TokenSettings>();
      services.AddSingleton(appSettings);

      services.AddIdentity<ApplicationUser, IdentityRole>(options =>
      {
        options.User.RequireUniqueEmail = true;
      })
        .AddEntityFrameworkStores<IdentityDbContext>();

      services.AddAuthentication(auth =>
      {
        auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      })
        .AddJwtBearer(options =>
        {
          options.ClaimsIssuer = appSettings.Issuer;
          options.IncludeErrorDetails = true;
          options.RequireHttpsMetadata = true;
          options.SaveToken = true;
          options.Validate(JwtBearerDefaults.AuthenticationScheme);
          options.TokenValidationParameters = new TokenValidationParameters
          {
            ClockSkew = TimeSpan.Zero,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = appSettings.Issuer,
            ValidAudience = appSettings.Audience,
            IssuerSigningKey = appSettings.SecurityKey,
            RequireSignedTokens = true,
            RequireExpirationTime = true

          };
          options.Events = new JwtBearerEvents
          {
            OnAuthenticationFailed = context =>
            {
              if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
              {
                context.Response.Headers.Add("Token-Expired", "true");
              }

              return Task.CompletedTask;
            }
          };
        });
      services.AddScoped<IUnitOfWork, UnitOfWork>();
      services.AddScoped<IAsyncCustomerService, CustomersService>();
      services.AddScoped<IJwtTokenService, JwtTokenService>();

      services.AddAutoMapper(typeof(Startup));

      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Store Api", Version = "v1" });
      });

      services.AddSpaStaticFiles(configuration =>
      {
        configuration.RootPath = "ClientApp/dist";
      });

      services.AddResponseCompression();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment() == false)
      {
        app.UseHsts();
      }

      app.UseExceptionHandlingMiddleware();

      app.UseSwagger()
         .UseSwaggerUI(c =>
         {
           c.SwaggerEndpoint("/swagger/v1/swagger.json", "Store Api");
         });

      app.UseHttpsRedirection();
      app.UseStaticFiles();
      if (!env.IsDevelopment())
      {
        app.UseSpaStaticFiles();
      }

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseResponseCompression();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });

      app.UseSpa(spa =>
      {
        spa.Options.SourcePath = "ClientApp";

        if (env.IsDevelopment())
        {
          spa.UseAngularCliServer(npmScript: "start");
          spa.Options.StartupTimeout = TimeSpan.FromSeconds(200);
        }
      });
    }
  }
}