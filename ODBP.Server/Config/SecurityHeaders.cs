using System;

namespace Microsoft.AspNetCore.Builder
{
    public static class SecurityHeaders
    {
        public static IApplicationBuilder UseOdbpSecurityHeaders(this WebApplication app)
        {
            var configuration = app.Services.GetRequiredService<IConfiguration>();
            var resources = configuration.GetSection("EXTERNAL_RESOURCES");
  
            var styleSources = new List<string?> {
                "'self'",
                Uri.TryCreate(resources["GEMEENTE_DESIGN_TOKENS_LINK"], UriKind.Absolute, out var tokens) ? tokens.ToString() : null
            };

            var imgSources = new List<string?> {
                "'self'",
                Uri.TryCreate(resources["GEMEENTE_LOGO_LINK"], UriKind.Absolute, out var logo) ? logo.ToString() : null,
                Uri.TryCreate(resources["GEMEENTE_FAVICON_LINK"], UriKind.Absolute, out var favicon) ? favicon.ToString() : null,
                Uri.TryCreate(resources["GEMEENTE_MAIN_IMAGE_LINK"], UriKind.Absolute, out var image) ? image.ToString() : null
            };

            return app.UseSecurityHeaders(x => x
                .AddDefaultSecurityHeaders()
                .AddCrossOriginOpenerPolicy(x =>
                {
                    x.SameOrigin();
                })
                .AddCrossOriginEmbedderPolicy(x =>
                {
                    x.RequireCorp();
                })
                .AddCrossOriginResourcePolicy(x =>
                {
                    x.SameOrigin();
                })
                .AddContentSecurityPolicy(csp =>
                {
                    csp.AddUpgradeInsecureRequests();
                    csp.AddDefaultSrc().None();
                    csp.AddConnectSrc().Self();
                    csp.AddScriptSrc().Self();
                    csp.AddStyleSrc().From(string.Join(" ", styleSources.Where(src => !string.IsNullOrWhiteSpace(src))));
                    csp.AddImgSrc().From(string.Join(" ", imgSources.Where(src => !string.IsNullOrWhiteSpace(src))));
                    csp.AddFontSrc().Self();
                    csp.AddFrameAncestors().None();
                    csp.AddFormAction().Self();
                    csp.AddBaseUri().None();
                })
                .AddPermissionsPolicy(permissions =>
                {
                    permissions.AddAccelerometer().None();
                    permissions.AddAmbientLightSensor().None();
                    permissions.AddAutoplay().None();
                    permissions.AddCamera().None();
                    permissions.AddEncryptedMedia().None();
                    permissions.AddFullscreen().None();
                    permissions.AddGeolocation().None();
                    permissions.AddGyroscope().None();
                    permissions.AddMagnetometer().None();
                    permissions.AddMicrophone().None();
                    permissions.AddMidi().None();
                    permissions.AddPayment().None();
                    permissions.AddPictureInPicture().None();
                    permissions.AddSpeaker().None();
                    permissions.AddSyncXHR().None();
                    permissions.AddUsb().None();
                    permissions.AddVR().None();
                }));
        }
    }
}
