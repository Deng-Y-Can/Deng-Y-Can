using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using WebApi.Middleware;
using WebApi.Services;

namespace WebApi
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
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                });

            services.AddMemoryCache();

            services.AddHttpClient();

            services.AddSingleton<GenericCrudService>();
            services.AddSingleton<FileService>();
            services.AddSingleton<ExportService>();
            services.AddSingleton<EmailService>();
            services.AddSingleton<SmsService>();
            services.AddSingleton<AuditService>();
            services.AddSingleton<DictService>();
            services.AddSingleton<CacheService>();
            services.AddSingleton<JobService>();
            services.AddSingleton<CryptoService>();
            services.AddSingleton<EncodeService>();
            services.AddSingleton<UtilityService>();
            services.AddSingleton<ProxyService>();
            services.AddSingleton<HealthService>();
            services.AddSingleton<IpService>();
            services.AddSingleton<QrCodeService>();
            services.AddSingleton<RateLimitService>();
            services.AddSingleton<JwtService>();
            services.AddSingleton<TextService>();
            services.AddSingleton<ColorService>();
            services.AddSingleton<NumberService>();
            services.AddSingleton<GeoService>();
            services.AddSingleton<MockService>();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "WebApi",
                    Version = "v1",
                    Description = "Universal API - CRUD, Files, Export, Notification, Cache, Jobs, Audit, Dict, Crypto, Encode, Utility, Proxy, Health, IP, QRCode, RateLimit, JWT, Text, Color, Number, Geo, Mock"
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi v1");
                    c.RoutePrefix = string.Empty;
                });
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseCors();

            app.UseMiddleware<AuditMiddleware>();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
