using BTCPayServer.Abstractions.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace BTCPayServer.Plugins.OAuth.Auth;

public class OAuthActionConvention : IActionModelConvention
{
    public void Apply(ActionModel action)
    {
        foreach (object attribute in action.Attributes)
        {
            if (attribute is not null)
            {
                if (attribute is AuthorizeAttribute authattribute)
                {
                    if (authattribute.AuthenticationSchemes is not null)
                    {
                        if (authattribute.AuthenticationSchemes.Contains(AuthenticationSchemes.Greenfield) ||
                        authattribute.AuthenticationSchemes.Contains(AuthenticationSchemes.GreenfieldAPIKeys) ||
                        authattribute.AuthenticationSchemes.Contains(AuthenticationSchemes.GreenfieldBasic))
                        {
                            authattribute.AuthenticationSchemes = authattribute.AuthenticationSchemes + ",OAuth.API";
                        }
                    }
                }
            }
        }
    }
}