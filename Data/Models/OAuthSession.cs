using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace BTCPayServer.Plugins.OAuth.Data.Models
{
    public class OAuthSession
    {
        [Key]
        [JsonIgnore]
        public string Token { get; set; }

        [JsonPropertyName("active")]
        [Column(TypeName = "boolean")]
        public bool Active { get; set; }

        [JsonPropertyName("scope")]
        public string Scope { get; set; }

        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }

        [JsonPropertyName("iss")]
        public string Issuer { get; set; }

        [JsonPropertyName("sub")]
        public string Subject { get; set; }

        [JsonPropertyName("exp")]
        public Int64 ExpiresAt { get; set; }

        [JsonPropertyName("iat")]
        public Int64 IssuedAt { get; set; }

        [JsonPropertyName("nbf")]
        public Int64 NotBefore { get; set; }

        [JsonIgnore]
        [JsonPropertyName("aud")]
        public string Audience { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("token_use")]
        public string TokenUse { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("identifier")]
        public string Identifier { get; set; }

    }

}

