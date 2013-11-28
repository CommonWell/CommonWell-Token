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

using System.IdentityModel.Tokens;

namespace CommonWell.Token
{
    public class TrustedIssuerNameRegistry : IssuerNameRegistry
    {
        public override string GetIssuerName(SecurityToken securityToken)
        {
            var x509Token = securityToken as X509SecurityToken;
            if (x509Token != null)
            {
                // Here you would check thumbprint against data store of trusted issuers
                return x509Token.Certificate.Thumbprint;
            }
            var rsaToken = securityToken as RsaSecurityToken;
            if (rsaToken != null)
            {
                // Here you would check ID against data store of trusted issuers
                return rsaToken.Id;
            }
            throw new SecurityTokenException("Untrusted issuer.");
        }
    }
}