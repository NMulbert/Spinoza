using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(SignalRConnectionSupport.Startup))]


namespace SignalRConnectionSupport
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var defaultCorsPolicyName = "myAllowSpecificOrigins";

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(defaultCorsPolicyName, b =>
                {
                    //App:CorsOrigins in appsettings.json can contain more than one address with split by comma.
                    b.SetIsOriginAllowed((_) => true)
                     .SetIsOriginAllowedToAllowWildcardSubdomains()
                     .AllowAnyHeader()
                     .AllowAnyMethod()
                     .AllowCredentials()
                     .WithHeaders("Access-Control-Allow-Origin", "*");
                });
            });
        }
    }
}
