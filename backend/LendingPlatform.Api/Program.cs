using LendingPlatform.Api.Data;
using LendingPlatform.Api.Services;
using LendingPlatform.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────────────────────────────────

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title       = "Lending Platform API",
        Version     = "v1",
        Description = "REST API for submitting and evaluating loan applications against the lending policy."
    });

    // Include XML documentation comments in Swagger UI.
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// SQLite database via EF Core
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=lending-platform.db";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Application services
builder.Services.AddScoped<ILoanDecisionService, LoanDecisionService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();

// CORS — restricted to the React development server origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendDev", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ── Build ─────────────────────────────────────────────────────────────────────

var app = builder.Build();

// Auto-apply pending EF Core migrations in the Development environment only.
// The "Testing" environment (used by integration tests with an in-memory DB)
// is deliberately excluded because Migrate() is a relational-only operation.
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Lending Platform API v1");
    options.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseCors("FrontendDev");
app.MapControllers();

app.Run();

// Make the implicit Program class accessible from the test project.
public partial class Program { }
