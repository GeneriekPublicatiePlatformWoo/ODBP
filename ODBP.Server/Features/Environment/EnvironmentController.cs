using Microsoft.AspNetCore.Mvc;
using static ODBP.Features.Environment.EnvironmentController;

namespace ODBP.Features.Environment
{
    [Route("api/environment")]
    [ApiController]
    public class EnvironmentController : ControllerBase
    {
        private readonly ResourcesConfig _resourcesConfig;

        public EnvironmentController(ResourcesConfig resourcesConfig)
        {
            _resourcesConfig = resourcesConfig;
        }

        [HttpGet("resources")]
        public IActionResult GetResources()
        {
            var response = new
            {
                _resourcesConfig.Name,
                _resourcesConfig.LogoUrl,
                _resourcesConfig.FaviconUrl,
                _resourcesConfig.ImageUrl,
                _resourcesConfig.TokensUrl,
                _resourcesConfig.Theme,
                _resourcesConfig.WebsiteUrl,
                _resourcesConfig.PrivacyUrl,
                _resourcesConfig.ContactUrl,
                _resourcesConfig.A11yUrl
            };

            return Ok(response);
        }
    }
}
