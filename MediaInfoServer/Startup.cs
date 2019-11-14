using System.Net.Http;
using Amazon;
using Amazon.S3;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Minio;
using PodcastCore.MediaInfoServer.Common;
using PodcastCore.MediaInfoServer.Services;

namespace PodcastCore.MediaInfoServer
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddLogging(builder => builder.AddConsole());
            services.AddMvc();

            // bind configuration
            var config = new Configuration();
            _configuration.Bind(config);
            services.AddSingleton(config);

            // configure storage client
            services.AddSingleton(new MinioClient(config.ServiceUrl, config.AccessKey, config.SecretKey));
            services.AddSingleton(new AmazonS3Client(config.AccessKey, config.SecretKey, new AmazonS3Config
            {
                ForcePathStyle = true,
                RegionEndpoint = RegionEndpoint.APEast1,
                ServiceURL = config.ServiceUrl
            }));

            services.AddSingleton(new HttpClient());
            services.AddSingleton(new MemoryStreamPool());
            services.AddSingleton<MediaInfoService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}