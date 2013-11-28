// ============================================================================
//  Copyright 2013 CommonWell Health Alliance
//   
//  Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
//  this file except in compliance with the License. You may obtain a copy of the 
//  License at 
//  
//      http://www.apache.org/licenses/LICENSE-2.0 
//  
//  Unless required by applicable law or agreed to in writing, software distributed 
//  under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
//  CONDITIONS OF ANY KIND, either express or implied. See the License for the 
//  specific language governing permissions and limitations under the License.
// ============================================================================

using System;
using System.Collections.Generic;
using System.IdentityModel.Configuration;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;

namespace CommonWell.Token
{
    public static class ExtensionMethods
    {
        public static string Write(this Saml2SecurityToken token, IdentityConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentException("Identity configuration cannot be null", "configuration");

            var settings = new XmlWriterSettings {Indent = false, Encoding = Encoding.Default};
            var sbuilder = new StringBuilder();
            using (var writer = XmlWriter.Create(sbuilder, settings))
            {
                if (token != null) configuration.SecurityTokenHandlers.WriteToken(writer, token);
            }
            var tokenString = sbuilder.ToString();
            return tokenString;
        }

        public static string Write(this JwtSecurityToken token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        /// <summary>Turns a supported generic XML security token into a security token.</summary>
        /// <param name="token">The generic XML security token.</param>
        /// <returns>A SecurityToken</returns>
        public static SecurityToken ToSecurityToken(this GenericXmlSecurityToken token)
        {
            SecurityTokenHandlerCollection handlerCollection =
                SecurityTokenHandlerCollection.CreateDefaultSecurityTokenHandlerCollection();
            return token.ToSecurityToken(handlerCollection);
        }

        /// <summary>Turns a supported generic XML security token into a security token.</summary>
        /// <param name="token">The token.</param>
        /// <param name="decryptionCertificate">The decryption certificate.</param>
        /// <returns>A SecurityToken</returns>
        public static SecurityToken ToSecurityToken(this GenericXmlSecurityToken token,
            X509Certificate2 decryptionCertificate)
        {
            SecurityTokenHandlerCollection handlerCollection =
                SecurityTokenHandlerCollection.CreateDefaultSecurityTokenHandlerCollection(new SecurityTokenHandlerConfiguration
                {
                    ServiceTokenResolver = decryptionCertificate.CreateSecurityTokenResolver()
                });
            return token.ToSecurityToken(handlerCollection);
        }

        /// <summary>Turns a supported generic XML security token into a security token.</summary>
        /// <param name="token">The generic XML security token.</param>
        /// <param name="handler">The security token handler.</param>
        /// <returns>A SecurityToken</returns>
        public static SecurityToken ToSecurityToken(this GenericXmlSecurityToken token,
            SecurityTokenHandlerCollection handler)
        {
            var xmlTextReader = new XmlTextReader(new StringReader(token.TokenXml.OuterXml));
            if (handler.CanReadToken(xmlTextReader))
                return handler.ReadToken(xmlTextReader);
            throw new InvalidOperationException("Unsupported token type");
        }

        /// <summary>Retrieves the XML from a <c>GenericXmlSecurityToken</c>.</summary>
        /// <param name="token">The token.</param>
        /// <returns>The token XML string.</returns>
        public static string ToTokenXmlString(this GenericXmlSecurityToken token)
        {
            return token.TokenXml.OuterXml;
        }

        /// <summary>Converts a supported token to an XML string.</summary>
        /// <param name="token">The token.</param>
        /// <returns>The token XML string.</returns>
        public static string ToTokenXmlString(this SecurityToken token)
        {
            var token1 = token as GenericXmlSecurityToken;
            if (token1 != null)
                return ToTokenXmlString(token1);
            var handlerCollection =
                SecurityTokenHandlerCollection.CreateDefaultSecurityTokenHandlerCollection();
            handlerCollection.AddOrReplace(new CustomSaml2SecurityTokenHandler());
            return ToTokenXmlString(token, handlerCollection);
        }

        /// <summary>Converts a supported token to an XML string.</summary>
        /// <param name="token">The token.</param>
        /// <param name="handler">The token handler.</param>
        /// <returns>The token XML string.</returns>
        public static string ToTokenXmlString(this SecurityToken token, SecurityTokenHandlerCollection handler)
        {
            if (!handler.CanWriteToken(token))
                throw new InvalidOperationException("Token type not supported");
            var sb = new StringBuilder(128);
            handler.WriteToken(new XmlTextWriter(new StringWriter(sb)), token);
            return sb.ToString();
        }

        /// <summary>Converts a SecurityToken to an IClaimsPrincipal.</summary>
        /// <param name="token">The token.</param>
        /// <param name="signingCertificate">The signing certificate.</param>
        /// <returns>An IClaimsPrincipal</returns>
        public static ClaimsPrincipal ToClaimsPrincipal(this SecurityToken token, X509Certificate2 signingCertificate)
        {
            SecurityTokenHandlerConfiguration standardConfiguration = CreateStandardConfiguration(signingCertificate);
            return ToClaimsPrincipal(token,
                CreateDefaultHandlerCollection(standardConfiguration));
        }

        /// <summary>Converts a SecurityToken to an IClaimsPrincipal.</summary>
        /// <param name="token">The token.</param>
        /// <param name="signingCertificate">The signing certificate.</param>
        /// <param name="audienceUri">The audience URI.</param>
        /// <returns>An IClaimsPrincipal</returns>
        public static ClaimsPrincipal ToClaimsPrincipal(this SecurityToken token, X509Certificate2 signingCertificate,
            string audienceUri)
        {
            SecurityTokenHandlerConfiguration standardConfiguration = CreateStandardConfiguration(signingCertificate);
            standardConfiguration.AudienceRestriction.AudienceMode = AudienceUriMode.Always;
            standardConfiguration.AudienceRestriction.AllowedAudienceUris.Add(new Uri(audienceUri));
            return ToClaimsPrincipal(token,
                CreateDefaultHandlerCollection(standardConfiguration));
        }

        /// <summary>Converts a SecurityToken to an IClaimsPrincipal.</summary>
        /// <param name="token">The token.</param>
        /// <param name="handler">The handler.</param>
        /// <returns>An IClaimsPrincipal</returns>
        public static ClaimsPrincipal ToClaimsPrincipal(this SecurityToken token, SecurityTokenHandlerCollection handler)
        {
            return
                new ClaimsPrincipal(handler.ValidateToken(token).Select(identity => identity));
        }

        private static SecurityTokenHandlerConfiguration CreateStandardConfiguration(X509Certificate2 signingCertificate)
        {
            return new SecurityTokenHandlerConfiguration
            {
                AudienceRestriction =
                {
                    AudienceMode = AudienceUriMode.Never
                },
                IssuerNameRegistry = CreateIssuerNameRegistry(signingCertificate),
                IssuerTokenResolver = CreateSecurityTokenResolver(signingCertificate),
                SaveBootstrapContext = true
            };
        }

        private static IssuerNameRegistry CreateIssuerNameRegistry(this X509Certificate2 certificate)
        {
            var issuerNameRegistry = new ConfigurationBasedIssuerNameRegistry();
            if (certificate.Thumbprint != null)
                issuerNameRegistry.AddTrustedIssuer(certificate.Thumbprint, certificate.Subject);
            return issuerNameRegistry;
        }

        private static SecurityTokenResolver CreateSecurityTokenResolver(this X509Certificate2 certificate)
        {
            return SecurityTokenResolver.CreateDefaultSecurityTokenResolver(new List<SecurityToken>
            {
                new X509SecurityToken(certificate)
            }.AsReadOnly(), true);
        }

        private static SecurityTokenHandlerCollection CreateDefaultHandlerCollection(
            this SecurityTokenHandlerConfiguration configuration)
        {
            var handlerCollection =
                SecurityTokenHandlerCollection.CreateDefaultSecurityTokenHandlerCollection(configuration);
            handlerCollection.AddOrReplace(new CustomSaml2SecurityTokenHandler());
            return handlerCollection;
        }
    }
}