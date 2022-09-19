using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder applicationBuilder, bool isProd)
        {
            using( var ServiceScopes = applicationBuilder.ApplicationServices.CreateScope())
            {
                SeedData(ServiceScopes.ServiceProvider.GetService<AppDbContext>(),isProd);
            }
        }

        public static void SeedData(AppDbContext context, bool isProd)
        {
            if(isProd)
            {
                try
                {
                    context.Database.Migrate();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString);
                }

            }
            if(!context.Platforms.Any())
            {
                Console.WriteLine("Seeding Data");
                context.Platforms.AddRange(
                    new Platform() {
                        Name="Dot Net",
                        Publisher="Microsoft",
                        Cost=800
                    },
                    new Platform() {
                        Name="Sql Server",
                        Publisher="Microsoft",
                        Cost=900
                    },
                    new Platform() {
                        Name="Kuberantes",
                        Publisher="Cloud Native Computing Foundation",
                        Cost=1000
                    }
                );
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("DB already has data");
            }
        }
    }
}
