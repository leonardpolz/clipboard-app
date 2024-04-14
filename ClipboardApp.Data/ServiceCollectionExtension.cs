using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ClipboardApp.Data;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddServices(this IServiceCollection collection, string databaseName)
    {
        collection.AddDbContext<ClipboardAppDbContext>(options =>
            options.UseInMemoryDatabase(databaseName));

        return collection;
    } 
}