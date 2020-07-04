using System.Runtime.CompilerServices;
using System.Security.Claims;
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
            var httpContextId = $"{RuntimeHelpers.GetHashCode(HttpContext):x8}";
            HttpContext.User.AddIdentities(new [] {new ClaimsIdentity(new [] {new Claim("dummyClaim", httpContextId) }) });
            HttpContext.Items["x"] = httpContextId;

            await Task.WhenAny(service.SomethingSlowAsync(), Task.Delay(1000));

            await HttpContext.Response.WriteAsync("Hello World!");
        }
    }
}
