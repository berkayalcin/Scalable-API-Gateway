using System;
using System.Linq;
using Blog.API.Domain.Repositories;
using Blog.API.Domain.Services.Post;
using Blog.API.Domain.Services.Providers;
using Blog.API.Extensions;
using Couchbase.Configuration.Client;
using Couchbase.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Blog.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "Blog_API", Version = "v1"}); });
            services.AddConsulConfig(Configuration);
            services.AddHealthChecks();
            services.AddCouchbase(client =>
                {
                    var ipList = Configuration["CouchbaseServerUrl"].Split(',').Select(ip => new Uri(ip)).ToList();
                    client.Servers = ipList;
                    client.UseSsl = false;
                    client.Username = Configuration["CouchbaseUserName"];
                    client.Password = Configuration["CouchbasePassword"];
                    client.UseConnectionPooling = true;
                    client.ConnectionPool = new ConnectionPoolDefinition
                    {
                        SendTimeout = 120000,
                        MaxSize = 20,
                        MinSize = 20
                    };
                    client.OperationLifespan = 90000;
                })
                .AddCouchbaseBucket<IPostCouchbaseProvider>("Posts");

            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IPostService, PostService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Blog_API v1"));
            }

            app.UseHealthChecks("/healthcheck");

            app.UseConsul();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}