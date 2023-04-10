using System.Runtime.CompilerServices;
using dionizos_backend_app.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using Microsoft.AspNetCore.HttpLogging;
using Quartz;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace dionizos_backend_app
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Prelaunch.GetSecrets();

            var configuration =  new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json");
            var config = configuration.Build();

            var builder = WebApplication.CreateBuilder(args);
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Services.AddHttpLogging(logging =>
            {
                // Customize HTTP logging here.
                logging.LoggingFields = HttpLoggingFields.RequestPath | HttpLoggingFields.RequestMethod | HttpLoggingFields.ResponseStatusCode;
                logging.MediaTypeOptions.AddText("application/javascript");
                logging.RequestBodyLogLimit = 4096;
                logging.ResponseBodyLogLimit = 4096;
            });

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<DionizosDataContext>(options => options.UseNpgsql());
            //Environment.GetEnvironmentVariable("POSTGRES_CONNSTRING"))

            builder.Services
                .AddMvc(options =>
                {
                    options.InputFormatters
                        .RemoveType<Microsoft.AspNetCore.Mvc.Formatters.SystemTextJsonInputFormatter>();
                    options.OutputFormatters
                        .RemoveType<Microsoft.AspNetCore.Mvc.Formatters.SystemTextJsonOutputFormatter>();
                })
                .AddNewtonsoftJson(opts =>
                {
                    opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    opts.SerializerSettings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
                })
                .AddXmlSerializerFormatters();

            builder.Services
                .AddSwaggerGen(builder =>
                {
                    builder.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "System rezerwacji miejsc na eventy",
                        Description = "System rezerwacji miejsc na eventy (ASP.NET Core 3.1)",
                        Contact = new OpenApiContact()
                        {
                            Name = "Swagger Codegen Contributors",
                            Url = new Uri("https://github.com/swagger-api/swagger-codegen"),
                            Email = "XXX@pw.edu.pl"
                        },
                        TermsOfService = new Uri("http://swagger.io/terms/")
                    });
                    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    builder.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

                    builder.AddSecurityDefinition("token", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.ApiKey,
                        Flows = new OpenApiOAuthFlows
                        {
                            ClientCredentials = new OpenApiOAuthFlow
                            {
                                TokenUrl = new Uri($"http://localhost:7046/connect/token"),
                                Scopes = { { "http://localhost:7046/", "System rezerwacji miejsc na eventy" } }
                            }
                        },
                        Name = "sessionToken",
                        In = ParameterLocation.Header
                    });

                    builder.AddSecurityRequirement(new OpenApiSecurityRequirement {
                        {
                            new OpenApiSecurityScheme {
                                Reference = new OpenApiReference {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "token"
                                },
                                Name = "sessionToken",
                                In = ParameterLocation.Header
                            },
                            new List<string>() {
                                "http://localhost:7046/"
                            }
                        }
                    });
                });

            builder.Services.AddCors();
            builder.Services.AddSingleton<IConfigurationRoot>(config);
            builder.Services.AddTransient<IHelper, Helpers>();
            builder.Services.AddTransient<IMailing, Mailing>();

            builder.Services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionScopedJobFactory();
                // Just use the name of your job that you created in the Jobs folder.
                var jobKey = new JobKey("RefreshEventStatus");
                q.AddJob<RefreshEventStatus>(opts => opts.WithIdentity(jobKey));

                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity("RefreshEventStatus-trigger")
                    //This Cron interval can be described as "run every minute" (when second is zero)
                    .WithCronSchedule("0 * * ? * *")
                );
            });
            builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

            var app = builder.Build();
            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(option =>
                {
                    option.SwaggerEndpoint("v1/swagger.json", "System rezerwacji miejsc na eventy Original");
                });
            }
            app.UseHttpLogging();
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.MapControllers();

            app.Run();
        }
    }
}