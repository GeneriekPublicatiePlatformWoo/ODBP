using Microsoft.AspNetCore.Mvc;
using ODBP.Apis.Odrc;
using System.Net;

namespace ODBP.Features.Documenten
{
    [ApiController]
    public class DocumentController(IOdrcClientFactory clientFactory) : ControllerBase
    {
        const string Gepubliceerd = "gepubliceerd";

        [HttpGet("api/{version}/documenten/{uuid:guid}")]
        public async Task<IActionResult> Get(string version, Guid uuid, CancellationToken token)
        {
            using var client = clientFactory.Create("Document ophalen");

            var url = $"/api/{version}/documenten/{uuid}";

            using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                // Log 404...
                return NotFound();
            }

            if (!response.IsSuccessStatusCode)
            {
                // Log error ODRC...
                return StatusCode(502);
            }

            var document = await response.Content.ReadFromJsonAsync<PublicatieDocument>(token);

            if (document?.Publicatiestatus != Gepubliceerd)
            {
                // Log document nog niet / niet meer gepubliceerd...
                return NotFound();
            }

            return Ok(document);
        }
    }
}
