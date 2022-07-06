using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Bank.Models;
using Bank.Blockchain;

namespace Bank
{
    public class Startup
    {
        public string APItitle { get; set; }
        public string APIversion { get; set; }

        public string SwaggerXMLFile { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            APItitle = Configuration.GetSection("APITitle").Value;
            APIversion = Configuration.GetSection("APIVersion").Value;
            SwaggerXMLFile = Configuration.GetSection("SwaggerXMLFile").Value;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(APIversion,
                    new OpenApiInfo
                    {
                        Title = APItitle,
                        Version = APIversion,
                     });
            });

            services.AddControllers();

            services.AddDbContext<BankContext>(opt =>
                opt.UseInMemoryDatabase("IOBuilderTest")
                );

            services.AddSingleton<IBankContract, BankContract>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/" + APIversion + "/swagger.json", APItitle);
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
