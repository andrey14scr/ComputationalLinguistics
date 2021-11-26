using System;
using ComputationalLinguistics.Core.Services.Implementation;
using ComputationalLinguistics.Core.Services.Interfaces;
using ComputationalLinguistics.DAL;
using ComputationalLinguistics.DAL.Core.Entities;
using ComputationalLinguistics.DAL.Repositories.Implementation;
using ComputationalLinguistics.DAL.Repositories.Interfaces;
using ComputationalLinguistics.Tools;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ComputationalLinguistics
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
            services.AddControllersWithViews();
            services.AddTransient<IRepository<Word>, WordRepository>();
            services.AddTransient<IRepository<WordInText>, WordInTextRepository>();
            services.AddTransient<IRepository<TextFile>, TextFileRepository>();
            services.AddTransient<IRepository<TagInfo>, TagInfoRepository>();
            services.AddTransient<IRepository<TagPair>, TagPairsRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IWordService, WordService>();
            services.AddScoped<ITextService, TextService>();
            services.AddScoped<ITagsInfoService, TagsInfoService>();

            services.AddAutoMapper(typeof(AutoMap).Assembly);
            
            services.AddDbContext<ComputationalLinguisticsContext>(opt => 
                opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddMemoryCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            Variables.WordsBlockSize = Convert.ToInt32(Configuration["Variables:WordsBlockSize"]);

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Words}/{action=Index}/{id?}");
            });
        }
    }
}