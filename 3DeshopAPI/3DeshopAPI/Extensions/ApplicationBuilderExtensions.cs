using _3DeshopAPI.Models.Settings;
using Domain.Product;
using Infrastructure;
using System.Reflection;

namespace _3DeshopAPI.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Prepares database with default values
        /// </summary>
        /// <param name="app"></param>
        /// <param name="defaultFileConfiguration"></param>
        /// <returns></returns>
        public static async Task<IApplicationBuilder> PrepareDatabase(this IApplicationBuilder app, DefaultFileSettings defaultFileConfiguration)
        {
            using var scopedServices = app.ApplicationServices.CreateScope();
            var serviceProvider = scopedServices.ServiceProvider;
            var context = serviceProvider.GetRequiredService<Context>();

            await SeedDefaultImage(context, defaultFileConfiguration.Image);

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
        /// Reads default server image file, converts to base64 and returns as Image
        /// </summary>
        /// <returns></returns>
        private static Image GetDefaultImage(Guid id)
        {
            string fileName = "DefaultImage.png";
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $@"Files\Images\{fileName}");
            byte[] imageBytes = System.IO.File.ReadAllBytes(path);
            string imageToBase64 = Convert.ToBase64String(imageBytes);
            string format = GetFileExtension(imageToBase64);

            var image = new Image()
            {
                Id = id,
                Data = imageBytes,
                Format = format,
                Name = fileName,
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
}
