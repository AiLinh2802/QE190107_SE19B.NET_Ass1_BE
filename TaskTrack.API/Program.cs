using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using TaskTrack.API.Infrastructure;
using TaskTrack.API.Middleware;
using TaskTrack.Repo.Data;
using TaskTrack.Repo.Repositories;
using TaskTrack.Service.Interfaces;
using TaskTrack.Service.Services;

var builder = WebApplication.CreateBuilder(args);

var rawConnection = Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Configure ConnectionStrings:DefaultConnection or DATABASE_URL.");
var connectionString = DatabaseUrlParser.Normalize(rawConnection);

builder.Services.AddDbContext<TaskTrackDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITagService, TagService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var allowedOrigins = (Environment.GetEnvironmentVariable("CORS_ORIGINS")
    ?? builder.Configuration["Cors:AllowedOrigins"]
    ?? "http://localhost:3000")
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
builder.Services.AddCors(options => options.AddPolicy("Frontend", policy =>
    policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod()));

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();
app.UseForwardedHeaders();
app.UseMiddleware<ApiExceptionMiddleware>();
app.UseCors("Frontend");

var swaggerEnabled = app.Environment.IsDevelopment()
    || builder.Configuration.GetValue<bool>("Swagger:Enabled")
    || bool.TryParse(Environment.GetEnvironmentVariable("ENABLE_SWAGGER"), out var enabled) && enabled;
if (swaggerEnabled)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/health");
app.MapControllers();
app.Run();

public partial class Program;
