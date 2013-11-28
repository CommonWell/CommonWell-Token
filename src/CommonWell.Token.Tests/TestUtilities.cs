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
using System.IdentityModel.Configuration;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;

namespace CommonWell.Token.Tests
{
    internal static class TestClaims
    {
        internal static string OrganizationClaim = "CommonWell Token Organization";
        internal static Uri OrganizationIdClaim = new Uri("urn:oid:1.2.3.4.5.6.7.8");
        internal static Uri HomeCommunityIdClaim = new Uri("urn:oid:9.8.7.6.5.4.3");
        internal static string NPIClaim = "12345667";
        internal static string SubjectClaim = "Dr. Vladmir Zhivago";
        internal static RoleClaim SubjectRoleClaim = new RoleClaim("PROVIDER", "234");
        internal static PurposeOfUseClaim PurposeOfUseClaim = new PurposeOfUseClaim("TREATMENT", "5678");
        internal static DateTime TokenExpiration = DateTime.Now.AddDays(3);
        internal static string Audience = "urn:commonwellalliance.org";
    }

    internal static class Utilities
    {
        internal static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        internal static SamlTokenContract SetBasicSAMLContract<T>(X509Certificate2 certificate)
            where T : SamlTokenContract, new()
        {
            return new T
            {
                Issuer = (certificate == null) ? "null" : certificate.SubjectName.Name,
                Organization = TestClaims.OrganizationClaim,
                OrganizationId = TestClaims.OrganizationIdClaim,
                Npi = TestClaims.NPIClaim,
                Subject = TestClaims.SubjectClaim,
                SubjectId = TestClaims.SubjectClaim,
                SubjectRole = TestClaims.SubjectRoleClaim,
                PurposeOfUse = TestClaims.PurposeOfUseClaim,
                Expiration = TestClaims.TokenExpiration,
                SigningCertificate = certificate,
                UseRsa = false,
                AlgorithmSuite = new Basic128SecurityAlgorithmSuite()
            };
        }

        internal static JWTTokenContract SetBasicJWTContract(X509Certificate2 certificate)
        {
            return new JWTTokenContract
            {
                Issuer = (certificate == null) ? "null" : certificate.SubjectName.Name,
                NameOption = NamingProtocol.Xspa,
                Organization = TestClaims.OrganizationClaim,
                OrganizationId = TestClaims.OrganizationIdClaim,
                Npi = TestClaims.NPIClaim,
                Subject = TestClaims.SubjectClaim,
                SubjectId = TestClaims.SubjectClaim,
                SubjectRole = TestClaims.SubjectRoleClaim,
                PurposeOfUse = TestClaims.PurposeOfUseClaim,
                Expiration = TestClaims.TokenExpiration,
                SigningCertificate = certificate
            };
        }
    }

    internal sealed class InitializedFixture
    {
        private static volatile InitializedFixture _instance;
        private static readonly object SyncRoot = new object();
        internal IdentityConfiguration IdentityConfigurationForTest;
        internal DateTime InvalidExpiration = DateTime.Now.AddDays(-3);
        internal DateTime ValidExpiration = DateTime.Now.AddDays(3);
        internal X509Certificate2 X509Certificate;

        private InitializedFixture()
        {
            var issuerRegistry = new TrustedIssuerNameRegistry();
            if (X509Certificate == null)
            {
                X509Certificate = new X509Certificate2(@"CommonWellTokenTest.pfx", "commonwell");
            }
            if (IdentityConfigurationForTest == null)
            {
                IdentityConfigurationForTest = new IdentityConfiguration(false)
                {
                    AudienceRestriction = {AudienceMode = AudienceUriMode.Never},
                    CertificateValidationMode = X509CertificateValidationMode.None,
                    RevocationMode = X509RevocationMode.NoCheck,
                    MaxClockSkew = new TimeSpan(50000000),
                    IssuerNameRegistry = issuerRegistry,
                    ServiceCertificate = X509Certificate
                };

                IdentityConfigurationForTest.SecurityTokenHandlers.Clear();
                IdentityConfigurationForTest.SecurityTokenHandlers.Add(new CustomSaml2SecurityTokenHandler());
                IdentityConfigurationForTest.SecurityTokenHandlers.Add(new JwtSecurityTokenHandler());
                IdentityConfigurationForTest.SecurityTokenHandlers.Add(new X509SecurityTokenHandler());
            }
        }

        internal static InitializedFixture Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new InitializedFixture();
                        }
                    }
                }
                return _instance;
            }
        }
    }
}