using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using pwc.Application.Mapping;
using pwc.Extensions;
using pwc.Infrastructure;
using System.Reflection;
using pwc.Application.CQRS.Commands.Items;
using pwc.Domain.Interface.Repo;
using pwc.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateItemCommand).Assembly);
    // Or if your commands are in the same assembly as Program.cs:
    // cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});


// Transients 
builder.Services.AddTransient<IItemRepository, ItemRepository>();

builder.Services.AddAutoMapper(typeof(MappingProfile)); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContextPool<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        o => o.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorCodesToAdd: null)
    ));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PWC API", Version = "v1" });

    // Optional: Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigrations();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
