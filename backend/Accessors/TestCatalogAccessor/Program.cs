using AutoMapper;
using Spinoza.Backend.Crosscutting.CosmosDBWrapper;
using System.Text.Json;
using Spinoza.Backend.Accessor.TestCatalog;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.AllowAnyOrigin().WithMethods("PUT", "POST", "DELETE", "GET");                          
                      });
});
builder.Services.AddControllers().AddDapr()
    .AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});
builder.Services.AddSingleton<ICosmosDBWrapper, CosmosDBWrapper>();
builder.Services.AddSingleton<ICosmosDbInformationProvider>(new CosmosDbInformationProvider("Catalog", "Tests", "testVersion", "title"));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

try
{
    var mapperConfig = new MapperConfiguration(mc => { mc.AddProfile(new AutoMapperProfile()); });
    IMapper mapper = mapperConfig.CreateMapper();
    mapperConfig.AssertConfigurationIsValid();
    builder.Services.AddSingleton(mapper);
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
    throw;
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(MyAllowSpecificOrigins);

app.UseHttpsRedirection();

app.Urls.Add("http://*:80");

app.UseAuthorization();

app.MapControllers();

app.Run();
