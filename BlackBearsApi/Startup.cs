﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlackBearApi.Model;
using BlackBearsApi.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace BlackBearApi
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
            services.AddSingleton(typeof(IDbCollectionRepository<Bear>), typeof(DbCollectionRepository<Bear>));
            services.AddSingleton(typeof(IDbCollectionRepository<Food>), typeof(DbCollectionRepository<Food>));
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder => builder.WithOrigins("*").AllowAnyHeader().AllowAnyMethod());
            });

            services.AddMvc();

            services.AddSwaggerGen(c => 
            {
                c.SwaggerDoc("v1", new Info { Title = "Black Bears API", Version = "v1"});
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors("AllowAll");

            app.UseSwagger();

            app.UseSwaggerUI(c => 
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Black Bears API");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}