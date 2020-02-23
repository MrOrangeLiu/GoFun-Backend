using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DivingApplication.DbContexts;
using DivingApplication.Repositories;
using DivingApplication.Services;
using DivingApplication.Services.PropertyServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;

namespace DivingApplication
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

            var appSettingsSection = Configuration.GetSection("AppSettings");
            var appSettings = appSettingsSection.Get<AppSettingsService>();

            services.Configure<AppSettingsService>(appSettingsSection);

            services.AddControllers(setupAction =>
            {
                // If the return type is not acceptable (application/json || application/xml)
                setupAction.ReturnHttpNotAcceptable = true;
            })
             .AddNewtonsoftJson(setupAction => // The first one will be default (XML)
            {
                setupAction.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); // For JsonPatchDocument
                setupAction.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize;
                setupAction.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
            })
              .AddXmlDataContractSerializerFormatters(); // adding the XML support


            var key = Encoding.ASCII.GetBytes(appSettings.SecretForJwt);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
          .AddJwtBearer(x =>
          {
              x.Events = new JwtBearerEvents
              {
                  OnTokenValidated = (context) =>
                 {
                     var userService = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
                     var userId = Guid.Parse(context.Principal.FindFirstValue(ClaimTypes.NameIdentifier));
                     var user = userService.GetUserForJwt(userId);
                     if (user == null)
                     {
                         // return unauthorized if user no longer exists
                         context.Fail("Unauthorized");
                     }
                     return Task.CompletedTask;
                 }
              };
              x.RequireHttpsMetadata = false;
              x.SaveToken = true;
              x.TokenValidationParameters = new TokenValidationParameters
              {
                  ValidateIssuerSigningKey = true,
                  IssuerSigningKey = new SymmetricSecurityKey(key),
                  ValidateIssuer = false,
                  ValidateAudience = false
              };
          });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("NormalAndAdmin", policy => policy.RequireRole("Admin", "Normal"));
            });


            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); // Adding autoMapper service

            services.AddDbContext<DivingAPIContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPostRepository, PostRepository>();

            services.AddScoped<IPropertyMappingService, PropertyMappingService>();
            services.AddScoped<IPropertyValidationService, PropertyValidationService>();

            services.AddCors(setupActions => setupActions.AddPolicy("CorsPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
            }));

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseCors("CorsPolicy");

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
