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

namespace CommonWell.Token
{
    internal static class CustomSaml2TokenConstants
    {
        public const string Audience = "Audience";
        public const string ExpiresOn = "ExpiresOn";
        public const string Id = "Id";
        public const string Issuer = "Issuer";
        public const string Signature = "HMACSHA256";
        public const string ValidFrom = "ValidFrom";
        public const string ValueTypeUri = "urn:oasis:names:tc:SAML:2.0:assertion";
        public const string SAML2TokenType = "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0";
        public const string SymmetricKey = "http://docs.oasis-open.org/ws-sx/ws-trust/200512/SymmetricKey";
        public const string AsymmetricKey = "http://docs.oasis-open.org/ws-sx/ws-trust/200512/PublicKey";
        public const string BearerKey = "http://docs.oasis-open.org/ws-sx/ws-trust/200512/Bearer";
    }

    /// <summary>Supported SAML v2.0 Authentication Context Classes</summary>
    public static class AuthenticationContextClasses
    {
        /// <summary>urn:oasis:names:tc:SAML:2.0:ac:classes:Password</summary>
        public const string Password = "urn:oasis:names:tc:SAML:2.0:ac:classes:Password";

        /// <summary>urn:oasis:names:tc:SAML:2.0:ac:classes:PasswordProtectedTransport</summary>
        public const string PasswordProtectedTransport =
            "urn:oasis:names:tc:SAML:2.0:ac:classes:PasswordProtectedTransport";

        /// <summary>urn:oasis:names:tc:SAML:2.0:ac:classes:TLSClient</summary>
        public const string TLSClient = "urn:oasis:names:tc:SAML:2.0:ac:classes:TLSClient";

        /// <summary>urn:oasis:names:tc:SAML:2.0:ac:classes:X509</summary>
        public const string X509Certificate = "urn:oasis:names:tc:SAML:2.0:ac:classes:X509";

        /// <summary>urn:oasis:names:tc:SAML:2.0:ac:classes:Kerberos</summary>
        public const string Kerberos = "urn:oasis:names:tc:SAML:2.0:ac:classes:Kerberos";

        /// <summary>urn:oasis:names:tc:SAML:2.0:ac:classes:InternetProtocol</summary>
        public const string InternetProtocol = "urn:oasis:names:tc:SAML:2.0:ac:classes:InternetProtocol";

        /// <summary>urn:oasis:names:tc:SAML:2.0:ac:classes:PreviousSession</summary>
        public const string PreviousSession = "urn:oasis:names:tc:SAML:2.0:ac:classes:PreviousSession";

        /// <summary>urn:oasis:names:tc:SAML:2.0:ac:classes:unspecified</summary>
        public const string Unspecified =
            "urn:oasis:names:tc:SAML:2.0:ac:classes:unspecified";
    }
}