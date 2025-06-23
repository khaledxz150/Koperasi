

using Infrastructure.Koperasi;
using Infrastructure.Koperasi.Managers;

var builder = WebApplication.CreateBuilder(args);

// Add services

builder.Services.InjectDB<ApplicationDbContext>(builder.Configuration.GetConnectionString("DefaultConnection"));


builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ILocalizationService, LocalizationService>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS for mobile apps
builder.Services.AddCors(options =>
{
    options.AddPolicy("MobileAppPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("MobileAppPolicy");
app.UseAuthorization();
app.MapControllers();

// Initialize database and cache
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();

    var localizationService = scope.ServiceProvider.GetRequiredService<ILocalizationService>();
    await localizationService.RefreshCacheAsync();
}
