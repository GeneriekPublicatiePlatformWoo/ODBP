using Microsoft.AspNetCore.Mvc;
using ODBP.Apis.Odrc;
using System.Text.Json.Nodes;

namespace ODBP.Features.Informatiecategorieen
{
    [ApiController]
    public class InformatiecategorieenController(IOdrcClientFactory clientFactory) : ControllerBase
    {
        [HttpGet("api/{version}/informatiecategorieen")]
        public async Task<IActionResult> Get(string version, CancellationToken token, [FromQuery] string? page = "1")
        {
            using var client = clientFactory.Create("Informatiecategorieen ophalen");
            var url = $"/api/{version}/informatiecategorieen?page={page}";

            using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(502);
            }

            var json = await response.Content.ReadFromJsonAsync<PagedResponseModel<JsonNode>>(token);

            return Ok(json);
        }
    }
}
