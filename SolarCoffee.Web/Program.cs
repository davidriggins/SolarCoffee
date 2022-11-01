using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using SolarCoffee.Data;
using SolarCoffee.Services.Customer;
using SolarCoffee.Services.Inventory;
using SolarCoffee.Services.Order;
using SolarCoffee.Services.Product;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddCors(options =>
//  options.AddPolicy(MyAllowSpecificOrigins,
//    policy =>
//    {
//      policy.WithOrigins("http://localhost:8080")
//      //.AllowAnyOrigin()
//      .AllowAnyHeader()
//      .AllowAnyMethod();
//    }));


builder.Services.AddControllers().AddNewtonsoftJson(opts =>
{
  opts.SerializerSettings.ContractResolver = new DefaultContractResolver
  {
    NamingStrategy = new CamelCaseNamingStrategy()
  };
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Access to DB Connection String
builder.Services.AddDbContext<SolarDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("solar.dev")));

builder.Services.AddTransient<IProductService, ProductService>();
builder.Services.AddTransient<ICustomerService, CustomerService>();
builder.Services.AddTransient<IInventoryService, InventoryService>();
builder.Services.AddTransient<IOrderService, OrderService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseRouting();

//app.UseCors(MyAllowSpecificOrigins);

app.UseCors(builder =>
  builder
    .WithOrigins(
      "http://localhost:8080",
      "http://localhost:8081",
      "http://localhost:8082")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()
    );

app.UseAuthorization();

app.MapControllers();

app.UseEndpoints(endpoints =>
  endpoints.MapControllers());


app.Run();
