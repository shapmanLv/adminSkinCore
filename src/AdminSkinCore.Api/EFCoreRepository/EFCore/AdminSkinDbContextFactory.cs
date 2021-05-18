using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.EFCoreRepository.EFCore
{
    public class AdminSkinDbContextFactory : IDesignTimeDbContextFactory<AdminSkinDbContext>
    {
        public AdminSkinDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
               .AddJsonFile("appsettings.json")
               .AddEnvironmentVariables()
               .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AdminSkinDbContext>();

            optionsBuilder.UseMySql(config.GetConnectionString("AdminSkinMysqlDb"),
                     ServerVersion.AutoDetect(config.GetConnectionString("AdminSkinMysqlDb")),
                     mySqlOptionsAction: options =>
                     {
                         options.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                         options.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                     });

            return new AdminSkinDbContext(optionsBuilder.Options);
        }
    }
}
