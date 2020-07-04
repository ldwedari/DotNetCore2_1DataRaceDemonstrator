using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetCore2_1HttpContextDataRace
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSingleton<Service>();
            services.AddHttpContextAccessor();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, Service service)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }

    public class Service
    {
        private readonly IHttpContextAccessor _accessor;

        public Service(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public async Task SomethingSlowAsync()
        {
            var y = _accessor.HttpContext.Items["x"];
            await Task.Delay(1500);

            // There's a data race here because this can be cleaned up at any time
            if (_accessor.HttpContext != null)
            {
                // Even though we did the null check above, it may have changed by the time we access the value we wanted.
                var x = _accessor.HttpContext.Items["x"];
                if (x != null && x != y)
                {
                    var now = $"{RuntimeHelpers.GetHashCode(_accessor.HttpContext):x8}";
                    x = y;
                }
            }
        }
    }
}
