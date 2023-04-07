using DbExploration.Data;
using FlowerSpot.Middleware;
using FlowerSpot.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson(); ;
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
/*builder.Services.AddEntityFrameworkNpgsql().AddDbContext<FlowerDbContext>(opt =>
      opt.UseNpgsql(builder.Configuration.GetConnectionString("SampleDbConnection")));*/
if (builder.Environment.EnvironmentName.ToString() != "Testing")
{
    builder.Services.AddEntityFrameworkNpgsql().AddDbContext<FlowerDbContext>(opt =>
            opt.UseNpgsql(builder.Configuration.GetConnectionString("SampleDbConnection")));
}
else
{
    builder.Services.AddEntityFrameworkNpgsql().AddDbContext<FlowerDbContext>(options =>
                        options.UseInMemoryDatabase("TestDB"));
}
    

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFlowerService, FlowerService>();
builder.Services.AddScoped<ISightingService, SightingService>();
builder.Services.AddScoped<ILikeService, LikeService>();


builder.Services.AddAuthentication("BasicAuthentication")
        .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

var app = builder.Build();

//builder.Services.AddScoped<IUserRepository, UserRepository>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
public partial class Program { }