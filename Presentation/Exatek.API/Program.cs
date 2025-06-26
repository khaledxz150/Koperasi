using Application.Services.Localization;
using Application.Services.System;
using Application.Services.User;
using Application.UnitOfWork;
using Application.UnitOfWork.Repos;

using Core.Services.Localization;
using Core.Services.System;
using Core.Services.User;
using Core.UnitOfWork;
using Core.UnitOfWork.Repos;

using Infrastructure.Data;
using Infrastructure.Koperasi.Managers;

using Koperasi.API.Filters.Operation;

using Microsoft.OpenApi.Models;

using Models.Entities.User;
using Models.ViewModels.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Exatek API", Version = "v1" });

    c.AddSecurityDefinition("LanguageID", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "LanguageID",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Description = "Required LanguageID header. Allowed values: 1 or 2. 1-English 2-Arabic",
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "LanguageID"
                }
            },
            new string[] { }
        }
    });

    // Add LanguageID header to all operations
    c.OperationFilter<LanguageIdHeaderOperationFilter>();
});

// Register the operation filter
builder.Services.AddSwaggerGenNewtonsoftSupport();
builder.Services.AddTransient<LanguageIdHeaderOperationFilter>();
builder.Services.InjectDB<ApplicationDbContext, Users>(builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.InjectIdentity<ApplicationDbContext,Users>(builder.Configuration);

builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));


builder.Services.AddScoped<ILocalizationService, LocalizationService>();
builder.Services.AddScoped<IDictionaryLocalizationRepository, DictionaryLocalizationRepository>();
builder.Services.AddScoped<ILanguagesRepository, LanguagesRepository>();


builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IPolicyService, PolicyService>();
builder.Services.AddScoped<IPolicyLocalizationRepository, PolicyLocalizationRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


//IOptions
builder.Services.Configure<TwilioOptions>(builder.Configuration.GetSection("Twilio"));
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));

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

app.UseDefaultFiles();
app.UseStaticFiles();

// Initialize database and cache
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();

    var localizationService = scope.ServiceProvider.GetRequiredService<ILocalizationService>();
    await localizationService.RefreshCacheAsync();
}

app.Run();