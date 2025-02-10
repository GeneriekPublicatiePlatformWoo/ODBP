using Microsoft.AspNetCore.Mvc;
using ODBP.Apis.Odrc;
using System.Text.Json.Nodes;

namespace ODBP.Features.Documenten
{
    [ApiController]
    public class DocumentenOverzichtController(IOdrcClientFactory clientFactory) : ControllerBase
    {
        [HttpGet("api/{version}/documenten")]
        public async Task<IActionResult> Get(
            string version,
            [FromQuery] string publicatie,
            CancellationToken token,
            [FromQuery] string? page = "1")
        {
            using var client = clientFactory.Create("Documenten ophalen");

            var url = $"/api/{version}/documenten?publicatie={publicatie}&publicatiestatus=gepubliceerd&page={page}";

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
