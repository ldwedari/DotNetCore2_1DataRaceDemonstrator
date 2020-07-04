using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DotNetCore2_1HttpContextDataRace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public async Task Get([FromServices]Service service)
        {
            HttpContext.Items["x"] = $"{RuntimeHelpers.GetHashCode(HttpContext):x8}";

            await Task.WhenAny(service.SomethingSlowAsync(), Task.Delay(1000));

            await HttpContext.Response.WriteAsync("Hello World!");
        }
    }
}
