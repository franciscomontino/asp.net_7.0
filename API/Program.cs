using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Product_API;
using Product_API.Data;
using Product_API.Repository;
using Product_API.Repository.IRepository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers(option =>
{
  option.CacheProfiles.Add("Default30",
      new CacheProfile()
      {
        Duration = 30
      });
}).AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
  options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
  {
    Description = "Ingresar Bearer [space] tuToken \r\n\r\n " +
                    "Ejemplo: Bearer 123456abcder",
    Name = "Authorization",
    In = ParameterLocation.Header,
    Scheme = "Bearer"
  });
  options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id= "Bearer"
                },
                Scheme = "oauth2",
                Name="Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

var key = builder.Configuration.GetValue<string>("ApiSettings:Secret");

builder.Services.AddAuthentication(x =>
{
  x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(x =>
    {
      x.RequireHttpsMetadata = false;
      x.SaveToken = true;
      x.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
      };
    });
builder.Services.AddResponseCaching();

builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddAutoMapper(typeof(MappingConfig));

// Add classes implementing the repository pattern
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IKindOfProductRepository, KindOfProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
// Swagger config
app.UseSwagger();
app.UseSwaggerUI(options =>
{
  options.SwaggerEndpoint("swagger/v1/swagger.json", "v1");
  options.RoutePrefix = string.Empty;
});
app.Use(async (context, next) =>
{
  if (context.Request.Path == "/")
  {
    context.Response.Redirect("/swagger/index.html");
  }
  await next();
});
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.Run();
