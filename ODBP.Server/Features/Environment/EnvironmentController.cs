using Microsoft.AspNetCore.Mvc;

namespace ODBP.Features.Environment
{
    [Route("api/environment")]
    [ApiController]
    public class EnvironmentController(IConfiguration configuration) : ControllerBase
    {
        private readonly IConfiguration _configuration = configuration;

        [HttpGet("resources")]
        public IActionResult GetExternalResources()
        {
            var resources = _configuration.GetSection("EXTERNAL_RESOURCES");

            var response = new Resources
            {
                Website = Uri.TryCreate(resources["GEMEENTE_WEBSITE_LINK"], UriKind.Absolute, out var website) ? website.ToString() : null,
                Logo = Uri.TryCreate(resources["GEMEENTE_LOGO_LINK"], UriKind.Absolute, out var logo) ? logo.ToString() : null,
                Favicon = Uri.TryCreate(resources["GEMEENTE_FAVICON_LINK"], UriKind.Absolute, out var favicon) ? favicon.ToString() : null,
                Image = Uri.TryCreate(resources["GEMEENTE_MAIN_IMAGE_LINK"], UriKind.Absolute, out var image) ? image.ToString() : null,
                Tokens = Uri.TryCreate(resources["GEMEENTE_DESIGN_TOKENS_LINK"], UriKind.Absolute, out var tokens) ? tokens.ToString() : null,
                Theme = resources["GEMEENTE_THEME"],
            };

            return Ok(response);
        }

        public class Resources
        {
            public string? Website { get; set; }
            public string? Logo { get; set; }
            public string? Favicon { get; set; }
            public string? Image { get; set; }
            public string? Tokens { get; set; }
            public string? Theme { get; set; }
        }
    }
}
