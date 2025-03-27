namespace BlinkayOccupation.API.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IConfiguration AddAppSettings(this IConfiguration configuration, WebApplicationBuilder builder)
        {
            string environmentVariable = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            builder.Configuration.AddJsonFile("appsettings.json");

            if (!string.IsNullOrEmpty(environmentVariable))
            {
                builder.Configuration.AddJsonFile($"appsettings.{environmentVariable}.json");
            }

            builder.Configuration.AddEnvironmentVariables();
            return builder.Configuration;
        }
    }
}
