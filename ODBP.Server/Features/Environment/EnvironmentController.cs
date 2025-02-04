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
                _resourcesConfig.Website,
                _resourcesConfig.Logo,
                _resourcesConfig.Favicon,
                _resourcesConfig.Image,
                _resourcesConfig.Tokens,
                _resourcesConfig.Theme
            };

            return Ok(response);
        }
    }
}
