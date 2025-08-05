using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using VNet.EntityFramework.AutoMigration;
using VNet.Utilites;
using VNet.Web.BaseCore;
using VNet.Web.BaseCore.Config;
using VNet.Web.BaseCore.Logs;

namespace VNet.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            try
            {
                // ����XML����
                builder.Services.AddXmlConfiguration(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "Data", "AppConfig.xml"));
                // ע���ܼ��ܷ���
                builder.Services.AddSingleton<IVNetCryptoProvider, VNetCryptoProvider>();
                // ��ȡ����ʵ��
                var appConfig = ConfigManager.Instance.GetConfig();
                var swaggerConfig = appConfig.Swagger;
                var globalExceptionConfig = appConfig.GlobalException;

                // ����CORS
                var allowOrigins = ConfigManager.Instance.GetAllowOrigins();

                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("DefaultPolicy", policy =>
                    {
                        if (allowOrigins.Length > 0)
                        {
                            policy.WithOrigins(allowOrigins);
                        }
                        else
                        {
                            policy.AllowAnyOrigin();
                        }
                        policy.AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                    });
                });

                builder.Services.Configure<IISServerOptions>(options =>
                {
                    options.MaxRequestBodySize = appConfig.UploadSetting.MaxFileSize;
                });

                builder.Services.Configure<FormOptions>(options =>
                {
                    options.ValueLengthLimit = (int)appConfig.UploadSetting.MaxFileSize;
                    options.MultipartBodyLengthLimit = appConfig.UploadSetting.MaxFileSize;
                    options.MultipartHeadersLengthLimit = (int)appConfig.UploadSetting.MaxFileSize;
                });

                if (swaggerConfig.IsEnable)
                {
                    builder.Services.AddEndpointsApiExplorer();
                    builder.Services.AddSwaggerGen(c =>
                    {
                        c.SwaggerDoc(swaggerConfig.Version, new Microsoft.OpenApi.Models.OpenApiInfo
                        {
                            Title = swaggerConfig.Title,
                            Version = swaggerConfig.Version,
                            Description = swaggerConfig.Description
                        });

                        // ���JWT��֤��Swagger
                        c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                            Name = "Authorization",
                            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                            Scheme = "Bearer"
                        });

                        c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                        {
                            {
                                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                                {
                                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                                    {
                                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                        Id = "Bearer"
                                    }
                                },
                                Array.Empty<string>()
                            }
                        });
                    });
                }

                builder.AddLogging();
                builder.Services.AddConfiguredDatabase<VNetDbContext>();
                builder.Services.AddConfiguredRedis();
                builder.Services.AddControllers();
                builder.Services.ConfigureHttpJsonOptions(options =>
                {
                    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.SerializerOptions.WriteIndented = true;
                });
                builder.Services.AddScoped<DbContext, VNetDbContext>();
                builder.Services.AddAutoMigrationProvider();

                var app = builder.Build();
                ShellScope.RegisterShellScope(app.Services);
                if (globalExceptionConfig.IsEnable)
                {
                    app.UseExceptionHandler(errorApp =>
                    {
                        errorApp.Run(async context =>
                        {
                            context.Response.StatusCode = 500;
                            context.Response.ContentType = "application/json";

                            var error = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
                            if (error != null)
                            {
                                var response = new
                                {
                                    success = false,
                                    message = "�������ڲ�����",
                                    detail = globalExceptionConfig.ShowDetail ? error.Error.Message : null,
                                    timestamp = DateTime.UtcNow,
                                    path = context.Request.Path.Value
                                };

                                var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                                logger.LogError(error.Error, "ȫ���쳣�������쳣: {Message}", error.Error.Message);

                                var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
                                {
                                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                                });

                                await context.Response.WriteAsync(jsonResponse);
                            }
                        });
                    });
                }

                if (app.Environment.IsDevelopment())
                {
                    if (swaggerConfig.IsEnable)
                    {
                        app.UseSwagger();
                        app.UseSwaggerUI(c =>
                        {
                            c.SwaggerEndpoint($"/swagger/{swaggerConfig.Version}/swagger.json", swaggerConfig.Title);
                            c.RoutePrefix = "/swagger";
                        });
                    }
                }
                else
                {
                    app.UseHsts();
                }
                app.UseHttpsRedirection();
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), appConfig.UploadSetting.SavePath);
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                app.AutoMigrationAsync<VNetDbContext>().Wait();
                app.UseStaticFiles();
                app.UseCors("DefaultPolicy");
                app.UseAuthentication();
                app.UseAuthorization();
                app.MapControllers();
                app.MapGet("/health", () => new
                {
                    status = "healthy",
                    timestamp = DateTime.UtcNow,
                    version = swaggerConfig.Version
                }).AllowAnonymous();
                if (app.Environment.IsDevelopment())
                {
                    app.MapGet("/config", () => new
                    {
                        database = appConfig.DbSetting.GetActiveDbConfig().GetType().Name,
                        uploadMaxSize = appConfig.UploadSetting.MaxFileSize / 1024 / 1024 + "MB",
                        corsOrigins = allowOrigins,
                        swaggerEnabled = swaggerConfig.IsEnable
                    }).AllowAnonymous();
                }
                Console.WriteLine($"Ӧ�������ɹ�!");
                Console.WriteLine($"Swagger: {(swaggerConfig.IsEnable ? "����" : "����")}");
                Console.WriteLine($"ȫ���쳣����: {(globalExceptionConfig.IsEnable ? "����" : "����")}");
                Console.WriteLine($"CORS������Դ: {(allowOrigins.Length > 0 ? string.Join(", ", allowOrigins) : "������Դ")}");
                Console.WriteLine($"�ļ��ϴ�����С: {appConfig.UploadSetting.MaxFileSize / 1024 / 1024}MB");
                app.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ӧ������ʧ��: {ex.Message}");
                Console.WriteLine($"��ջ����: {ex.StackTrace}");
                throw;
            }
        }
    }
}