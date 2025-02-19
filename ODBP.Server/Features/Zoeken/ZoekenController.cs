using Microsoft.AspNetCore.Mvc;
using ODBP.Apis.Search;

namespace ODBP.Features.Zoeken
{
    [ApiController]
    [Route("api/zoeken")]
    public class ZoekenController(ISearchClient client) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Get([FromBody] Apis.Search.SearchRequest request, CancellationToken token)
        {
            var result = await client.Search(request, token);
            return Ok(result);
        }
    }
}
