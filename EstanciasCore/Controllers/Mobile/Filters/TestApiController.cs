using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DAL.Data;
using DAL.Models;
using EstanciasCore.API.Filters;

namespace EstanciasCore.API.Controllers
{
    [TypeFilter(typeof(ChequeaUatApiAttribute))]
    [ApiController]
    [Route("api/[controller]")]
    public class TestApiController : BaseApiController
    {

        public TestApiController(EstanciasContext context) : base(context)
        {
        }

        [HttpGet]
        [Route("Obtener")]
        public async Task<IActionResult> Get()
        {
            return Ok(new JsonResult(new { p1 = 2, p2 = "aaa" }));
        }

        [HttpPost]
        [Route("Grabar")]
        public async Task<IActionResult> Post([FromBody] RespuestaAPI objeto)
        {
            return Ok(new JsonResult(objeto));
        }

    }
}
