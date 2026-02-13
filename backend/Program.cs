var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddControllers();

// Add CORS policy configured for development and production
builder.Services.AddCors(options =>
{
    var corsOrigins = new List<string>
    {
        "http://localhost:4200",
        "https://localhost:4200"
    };

    // Add Azure App Service URLs if configured
    var azureFrontendUrl = builder.Configuration["AzureFrontendUrl"];
    if (!string.IsNullOrEmpty(azureFrontendUrl))
    {
        corsOrigins.Add(azureFrontendUrl);
    }

    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(corsOrigins.ToArray())
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngular");

// Serve static files (frontend)
app.UseStaticFiles();

// Map API controllers
app.MapControllers();

// SPA fallback - serve index.html for all unmatched routes
// This enables client-side routing for Angular
app.MapFallbackToFile("index.html");

app.Run();
