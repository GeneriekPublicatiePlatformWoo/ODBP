using System.Text.Json;
using System.Web;
using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Mvc;
using ODBP.Apis.Search;

namespace ODBP.Features.Zoeken
{
    [ApiController]
    [Route("api/zoeken")]
    public class ZoekenController(ISearchClient client) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] Apis.Search.SearchRequest request, CancellationToken token)
        {
            var result = await client.Search(request, token);
            return Ok(result);
        }
    }
}
