using System.ComponentModel.DataAnnotations;
// using BTCPayServer.Plugins.Kratos.Validation;

namespace BTCPayServer.Plugins.OAuth.Data
{
    public class OAuthConf
    {
        [Display(Name = "Endpoint for Token Introspection")]
        // [KratosPublicAttribute]
        public string IntroEndpoint { get; set; }

        [Display(Name = "List of allowed scopes, scope names separated by ;")]
        // [KratosPublicAttribute]
        public string AllowedScopes { get; set; }
    }
}
