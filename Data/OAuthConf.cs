using System.ComponentModel.DataAnnotations;
// using BTCPayServer.Plugins.Kratos.Validation;

namespace BTCPayServer.Plugins.OAuth.Data
{
    public class OAuthConf
    {
        [Display(Name = "Endpoint for Token Introspection")]
        // [KratosPublicAttribute]
        public string Intro_Endpoint { get; set; }
    }
}
