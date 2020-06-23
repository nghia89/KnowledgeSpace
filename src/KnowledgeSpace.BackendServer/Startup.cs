using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using KnowledgeSpace.BackendServer.Data;
using KnowledgeSpace.BackendServer.Data.Entities;
using KnowledgeSpace.BackendServer.Extensions;
using KnowledgeSpace.BackendServer.IdentityServer;
using KnowledgeSpace.BackendServer.Services;
using KnowledgeSpace.ViewModels;
using KnowledgeSpace.ViewModels.FluntVaition;
using KnowledgeSpace.ViewModels.Systems;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace KnowledgeSpace.BackendServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        private readonly string KspSpecificOrigins = "KspSpecificOrigins";
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //var secret = new IdentityServer4.Models.Secret("secret".Sha256());
            //1. Setup entity framework

            services.AddDbContextPool<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            //services.AddDbContextPool<ApplicationDbContext>((serviceProvider, optionsBuilder) =>
            //{
            //    optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            //    optionsBuilder.UseInternalServiceProvider(serviceProvider.GetService<IUserResolverService>());
            //});
            //2. Setup idetntity
            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
            .AddInMemoryApiResources(ConfigIdentity.Apis)
            .AddInMemoryClients(Configuration.GetSection("IdentityServer:Clients"))
            .AddInMemoryIdentityResources(ConfigIdentity.Ids)
            .AddAspNetIdentity<User>()
            .AddProfileService<IdentityProfileService>()
            .AddDeveloperSigningCredential();
            services.AddCors(options =>
            {
                options.AddPolicy(KspSpecificOrigins,
                builder =>
                {
                    builder.WithOrigins(Configuration["AllowOrigins"])
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
            services.Configure<IdentityOptions>(options =>
            {
                // Default Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.User.RequireUniqueEmail = true;
            });
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            services.AddControllersWithViews()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<RoleRequestValidator>());
            services.AddAuthentication()
               .AddLocalApi("Bearer", option =>
               {
                   option.ExpectedScope = "api.knowledgespace";
               });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Bearer", policy =>
                {
                    policy.AddAuthenticationSchemes("Bearer");
                    policy.RequireAuthenticatedUser();
                });
            });

            #region  config remove  Identity
            services.AddRazorPages(options =>
            {
                options.Conventions.AddAreaFolderRouteModelConvention("Identity", "/Account/", model =>
                {
                    foreach (var selector in model.Selectors)
                    {
                        var attributeRouteModel = selector.AttributeRouteModel;
                        attributeRouteModel.Order = -1;
                        attributeRouteModel.Template = attributeRouteModel.Template.Remove(0, "Identity".Length);
                    }
                });
            });
            #endregion

            services.AddTransient<DbInitializer>();


            #region login and sendMail
            services.AddTransient<IUserResolverService, UserResolverService>();
            services.AddTransient<IEmailSender, EmailSenderService>();
            services.AddTransient<ISequenceService, SequenceService>();
            services.AddTransient<IStorageService, FileStorageService>();
            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            services.AddTransient<IViewRenderService, ViewRenderService>();
            services.AddTransient<ICacheService, DistributedCacheService>();
            services.AddTransient<IOneSignalService, OneSignalService>();
            #endregion
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Knowledge Space API", Version = "v1" });
                #region cấu hình authorize

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(Configuration["AuthorityUrl"] + "/connect/authorize"),
                            Scopes = new Dictionary<string, string> { { "api.knowledgespace", "KnowledgeSpace API" } }
                        },
                    },
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new List<string>{ "api.knowledgespace" }
                    }
                });

                #endregion
            });

            services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = Configuration.GetConnectionString("DefaultConnection");
                options.SchemaName = "dbo";
                options.TableName = "CacheTable";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseErrorWrapping();
            app.UseStaticFiles();

            app.UseIdentityServer();

            app.UseAuthentication();
            app.UseCors(KspSpecificOrigins);
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            #region  config run razorPages
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });
            #endregion

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.OAuthClientId("swagger");// src ConfigIdentity.cs
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Knowledge Space API V1");
            });
        }
    }
}