using System;
using System.Linq;
using Blog.API.Domain.Mappers;
using Blog.API.Domain.Models;
using Blog.API.Domain.Repositories;
using Blog.API.Domain.Services.ElasticSearch;
using Blog.API.Domain.Services.Post;
using Blog.API.Domain.Services.Providers;
using Blog.API.Domain.Validators;
using Blog.API.Extensions;
using Couchbase.Configuration.Client;
using Couchbase.Extensions.DependencyInjection;
using Elasticsearch.Net;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Nest;

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
            services.AddControllers(options => { options.AllowEmptyInputInBodyModelBinding = true; })
                .AddFluentValidation(o => { o.RegisterValidatorsFromAssemblyContaining<PostValidator>(); }
                )
                .AddJsonOptions(opt => { opt.JsonSerializerOptions.IgnoreNullValues = true; }
                );

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
                .AddCouchbaseBucket<IPostCouchbaseProvider>("posts");

            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IPostService, PostService>();

            services.AddValidatorsFromAssemblyContaining<PostValidator>();
            services.AddAutoMapper(typeof(PostMapper));

            AddElastic(services);
        }

        private void AddElastic(IServiceCollection services)
        {
            services.AddScoped<IElasticSearchService, ElasticSearchService>();
            services.AddScoped<ElasticClient>(provider =>
            {
                var elasticOptions = new ElasticSearchOptions();
                Configuration.Bind("ElasticSearchOptions", elasticOptions);
                var uris = elasticOptions.HostUrls.Split(",").Select(u => new Uri(u)).ToArray();
                var connectionPool = new SniffingConnectionPool(uris);
                var settings = new ConnectionSettings(connectionPool);
                settings.DefaultFieldNameInferrer(p => p);
                var client = new ElasticClient(settings);
                return client;
            });
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