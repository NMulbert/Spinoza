using AutoMapper;
using CatalogManager;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.AllowAnyOrigin().WithMethods("PUT","POST", "DELETE", "GET");
                      });
});
// Add services to the container.

builder.Services.AddControllers().AddDapr();
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
//app.UseHttpsRedirection();
//app.UseRouting();
app.UseAuthorization();
app.UseCloudEvents();
app.Urls.Add("http://*:80");
app.MapControllers();
app.MapSubscribeHandler();
//app.UseEndpoints(endpoints =>
//{

//    endpoints.MapSubscribeHandler();
//    endpoints.MapControllers();
//});
app.Run();
