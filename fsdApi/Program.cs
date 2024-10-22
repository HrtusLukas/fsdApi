using Microsoft.EntityFrameworkCore;
using fsdApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS policy for React app
builder.Services.AddCors(options =>
{
    options.AddPolicy("reactApp", policyBuilder =>
    {
        policyBuilder.WithOrigins("http://localhost:3000")
                     .AllowAnyHeader()
                     .AllowAnyMethod()
                     .AllowCredentials();
    });
});

// Configure PostgreSQL and register the DataContext
var Configuration = builder.Configuration;
builder.Services.AddDbContext<DataContext>(options =>
    options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Apply the CORS policy
app.UseCors("reactApp");

app.UseAuthorization();

app.MapControllers();

app.Run();
