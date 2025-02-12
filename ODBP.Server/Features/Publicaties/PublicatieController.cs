using System.Net;
using Microsoft.AspNetCore.Mvc;
using ODBP.Apis.Odrc;

namespace ODBP.Features.Publicaties
{
    [ApiController]
    public class PublicatieController(IOdrcClientFactory clientFactory) : ControllerBase
    {
        const string Gepubliceerd = "gepubliceerd";

        [HttpGet("api/{version}/publicaties/{uuid:guid}")]
        public async Task<IActionResult> Get(string version, Guid uuid, CancellationToken token)
        {
            using var client = clientFactory.Create("Publicatie ophalen");

            var url = $"/api/{version}/publicaties/{uuid}";

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

            var publicatie = await response.Content.ReadFromJsonAsync<Publicatie>(token);

            if (publicatie?.Publicatiestatus != Gepubliceerd)
            {
                // Log publicatie nog niet / niet meer gepubliceerd...
                return NotFound();
            }

            return Ok(publicatie);
        }
    }
}
