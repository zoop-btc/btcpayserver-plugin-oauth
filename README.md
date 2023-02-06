# BTCPay Server OAuth Plugin

A [BTCPay Server](https://github.com/btcpayserver) plugin.  
This makes it possible to verify OAuth2 tokens. Right now it is build to work with Hydras [OAuth Introspection](https://www.ory.sh/docs/hydra/reference/api#tag/oAuth2/operation/introspectOAuth2Token) endpoint.  

## In Scope

* Login users with OAuth 2 tokens
* Introspect via Hydra Endpoint secured by jwt or basic auth
* Authorize Greenfield requests
* Optional automatic user registration

## Out of Scope

* Be an Oauth 2 Client
* Be an Oauth 2 Login Provider
* OpenID Connect Authorization
* Authorize Website requests

