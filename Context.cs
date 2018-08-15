using Chama.WebApi.ContextDatabase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

using System.IO;

namespace Chama.WebApi
{
    public class Context : IDesignTimeDbContextFactory<ChamaContext>
    {
        public ChamaContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<ChamaContext>();
            builder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            return new ChamaContext(builder.Options);
        }
    }
}
