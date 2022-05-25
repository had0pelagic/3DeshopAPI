using _3DeshopAPI.Models.Settings;
using Domain.Product;
using Infrastructure;
using System.Reflection;

namespace _3DeshopAPI.Extensions;

public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Prepares database with default values
    /// </summary>
    /// <param name="app"></param>
    /// <param name="defaultFileConfiguration"></param>
    /// <returns></returns>
    public static async Task<IApplicationBuilder> PrepareDatabase(this IApplicationBuilder app, DefaultFileSettings defaultFileConfiguration, List<DefaultCategorySettings> defaultCategorySettings, List<DefaultFormatSettings> defaultFormatSettings)
    {
        using var scopedServices = app.ApplicationServices.CreateScope();
        var serviceProvider = scopedServices.ServiceProvider;
        var context = serviceProvider.GetRequiredService<Context>();

        await SeedDefaultImage(context, defaultFileConfiguration.Image);
        await SeedCategories(context, defaultCategorySettings);
        await SeedFormats(context, defaultFormatSettings);

        return app;
    }

    /// <summary>
    /// Uploads default image to database
    /// </summary>
    /// <param name="context"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    private static async Task SeedDefaultImage(Context context, string id)
    {
        var defaultImageGuid = new Guid(id);
        var image = await context.Images.FindAsync(defaultImageGuid);

        if (image != null)
        {
            return;
        }

        var defaultImage = GetDefaultImage(defaultImageGuid);

        await context.Images.AddAsync(defaultImage);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Uploads default product categories to database
    /// </summary>
    /// <param name="context"></param>
    /// <param name="defaultCategorySettings"></param>
    /// <returns></returns>
    private static async Task SeedCategories(Context context, List<DefaultCategorySettings> defaultCategorySettings)
    {
        foreach (var categoryValues in defaultCategorySettings)
        {
            var categoryId = new Guid(categoryValues.Id);
            var dbCategory = await context.Categories.FindAsync(categoryId);

            if (dbCategory != null)
            {
                continue;
            }

            var category = new Category { Id = categoryId, Name = categoryValues.Name };
            await context.Categories.AddAsync(category);
        }

        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Uploads default formats to database
    /// </summary>
    /// <param name="context"></param>
    /// <param name="defaultFormatSettings"></param>
    /// <returns></returns>
    private static async Task SeedFormats(Context context, List<DefaultFormatSettings> defaultFormatSettings)
    {
        foreach (var formatValues in defaultFormatSettings)
        {
            var formatId = new Guid(formatValues.Id);
            var dbFormat = await context.Formats.FindAsync(formatId);

            if (dbFormat != null)
            {
                continue;
            }

            var format = new Format { Id = formatId, Name = formatValues.Name };
            await context.Formats.AddAsync(format);
        }

        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Reads default server image file, converts to base64 and returns as Image
    /// </summary>
    /// <returns></returns>
    private static Image GetDefaultImage(Guid id)
    {
        const string fileName = "DefaultImage.png";
        var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $@"Files\Images\{fileName}");
        var imageBytes = System.IO.File.ReadAllBytes(path);
        var imageToBase64 = Convert.ToBase64String(imageBytes);
        var format = GetFileExtension(imageToBase64);

        var image = new Image
        {
            Id = id,
            Data = imageBytes,
            Format = format,
            Name = fileName
        };

        return image;
    }

    /// <summary>
    /// Returns file type based of given base64
    /// </summary>
    /// <param name="base64"></param>
    /// <returns></returns>
    private static string GetFileExtension(string base64)
    {
        var data = base64.Substring(0, 5);

        return data.ToUpper() switch
        {
            "IVBOR" => "data:image/png;base64",
            "/9J/4" => "data:image/jpeg;base64",
            _ => string.Empty,
        };
    }
}