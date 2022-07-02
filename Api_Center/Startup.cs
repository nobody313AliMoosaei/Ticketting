using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLayer.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authentication.Google;
using Api_Center.Models;

namespace Api_Center
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

            services.AddControllers();


            services.AddCors(o =>
            {
                o.AddPolicy("AllowAll", t => 
                {
                    t.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
                });
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Api_Center", Version = "v1" });
            });

            #region Bearer
            /*
            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(ConfigOption =>
            {
                ConfigOption.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidIssuer = Configuration["JWTConfiguration:issuer"],
                    ValidAudience = Configuration["JWTConfiguration:audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWTConfiguration:key"])),
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true
                };
                ConfigOption.SaveToken = true;
                ConfigOption.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = Context =>
                      {
                          return Task.CompletedTask;
                      },
                    OnChallenge = Context =>
                      {
                          return Task.CompletedTask;
                      },
                    OnTokenValidated = Context =>
                    {
                        return Task.CompletedTask;
                    },
                    OnForbidden = Context =>
                      {
                          return Task.CompletedTask;
                      },
                    OnMessageReceived = Context =>
                      {
                          return Task.CompletedTask;
                      }
                };
            });
            */
            #endregion
            #region Database
            try
            {
                services.AddDbContext<DataLayer.DataBase.Context_DB>(option =>
                {
                    option.UseSqlServer(Configuration.GetConnectionString("SqlConnection"));
                    // Data Source=SQL8002.site4now.net;Initial Catalog=db_a8907f_alimoosaei;User Id=db_a8907f_alimoosaei_admin;Password=nobody3981231102
                });                
            }
            catch
            {
                throw new Exception("There is no connection to the database");
            }
            #endregion
            #region Dependency Injection
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ITicketService, TicketService>();
            services.AddScoped<IMethodHelper, MethodHelper>();

            #endregion
            #region Google Authentication
            /*
             Client Id = 541760749361-t4cbnqgpd16hdnlpcq28l0ga9b3cs8a7.apps.googleusercontent.com
            Client Secret = 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
            
            services.AddAuthentication(t =>
            {
                t.DefaultAuthenticateScheme = GoogleDefaults.AuthenticationScheme;
                t.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            }).AddCookie().AddGoogle(option =>
            {
                option.ClientId = "541760749361-t4cbnqgpd16hdnlpcq28l0ga9b3cs8a7.apps.googleusercontent.com";
                option.ClientSecret = "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";
            });*/
            #endregion
            #region Authentication
            
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(option =>
                {
                    option.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["JWTConfiguration:issuer"],
                        ValidAudience = Configuration["JWTConfiguration:audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(Configuration["JWTConfiguration:key"]))
                    };
                });
            #endregion

            services.AddMvc();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseExceptionHandler("/error");
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api_Center v1"));
            }else
            {
                app.UseExceptionHandler("/error");
            }
            app.UseHttpsRedirection();
            app.UseCors("AllowAll");

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
