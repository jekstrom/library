using DataAccess.Repositories;
using Domain.Models;
using Domain.Repositories;
using Domain.Services;
using library.Configuration;
using library.DatabaseInitialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace library
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            string connectionString = Configuration["SQLiteConnectionString"];
            Tables.CreateBooksTable(connectionString);
            Tables.CreateUsersTable(connectionString);

            List<UserRoleMap> userRoleMap = new List<UserRoleMap>();
            Configuration.GetSection("UserRoleMap").Bind(userRoleMap);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "GitHub";
            })
                .AddCookie()
                .AddOAuth("GitHub", options =>
                {
                    options.ClientId = Configuration["GitHub:ClientId"];
                    options.ClientSecret = Configuration["GitHub:ClientSecret"];
                    options.CallbackPath = new PathString("/signin-github");

                    options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
                    options.TokenEndpoint = "https://github.com/login/oauth/access_token";
                    options.UserInformationEndpoint = "https://api.github.com/user";

                    options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                    options.ClaimActions.MapJsonKey("urn:github:login", "login");
                    options.ClaimActions.MapJsonKey("urn:github:url", "html_url");
                    options.ClaimActions.MapJsonKey("urn:github:avatar", "avatar_url");

                    options.Events = new OAuthEvents
                    {
                        OnCreatingTicket = async context =>
                        {
                            var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                            var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                            response.EnsureSuccessStatusCode();

                            var user = JObject.Parse(await response.Content.ReadAsStringAsync());

                            context.RunClaimActions(user);

                            string username = context.Principal.Claims.FirstOrDefault(c => c.Type == "urn:github:login").Value;
                            UserRoleMap roles = userRoleMap.FirstOrDefault(u => u.Username.Equals(username));

                            if (roles != null)
                            {
                                // Update role claims on current user
                                context.Principal.AddIdentity(new ClaimsIdentity(roles.Roles.Select(r => new Claim(ClaimTypes.Role, r)).ToList()));
                            }
                        }
                    };
                });

            services.AddLogging(configure => configure.AddDebug());
            services.AddTransient<IDbConnection>(s => new SqliteConnection(connectionString));

            services.AddSingleton<IRepository<LibraryUser>>(s => new UserRepository(s.GetRequiredService<IDbConnection>(), userRoleMap.ToDictionary(kp => kp.Username, kp => kp.Roles.ToList() as IList<string>), s.GetRequiredService<ILogger<UserRepository>>()));
            services.AddSingleton<IRepository<Book>>(s => new BookRepository(s.GetRequiredService<IDbConnection>(), s.GetRequiredService<ILogger<BookRepository>>()));
            services.AddSingleton<IUserBookRepository>(s => new UserBookRepository(s.GetRequiredService<IDbConnection>(), s.GetRequiredService<ILogger<UserBookRepository>>()));

            services.AddSingleton<IUserBookService>(s => new UserBookService(s.GetRequiredService<IUserBookRepository>(), s.GetRequiredService<IRepository<Book>>()));

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
