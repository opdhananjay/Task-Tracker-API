using devops.Helpers;
using devops.Middlewares;
using devops.Repository;
using Serilog;
using Serilog.Exceptions;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithExceptionDetails()
    .WriteTo.Console()
    .WriteTo.File("Logs/app-log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Application Started");

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Start

    var allowedOrigins = builder.Configuration
        .GetSection("CorsSettings:AllowedOrigins")
        .Get<string[]>();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowReactApp", policy =>
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    builder.Services.AddSingleton<PasswordHelper>();

    builder.Services.AddScoped<SqlHelper>();

    builder.Services.AddScoped<IAuthRepository, AuthRepository>();

    builder.Services.AddScoped<ITrackerRepository, TrackerRepository>();



    // End

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }


    app.UseMiddleware<GlobalExceptionMiddleware>();  // Add Middlewares

    app.UseHttpsRedirection();

    app.UseCors("AllowReactApp");

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
   await Log.CloseAndFlushAsync();
}

