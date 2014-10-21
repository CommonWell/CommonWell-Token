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
using System.Collections.ObjectModel;
using System.IdentityModel.Configuration;
using System.IdentityModel.Policy;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security.Tokens;
using System.Xml;

namespace CommonWell.Token
{
    public class TokenFactory
    {
        internal IdentityConfiguration IdentityConfiguration;
        internal CustomSaml2SecurityTokenHandler Saml2Handler;

        public TokenFactory(IdentityConfiguration configuration)
        {
            Saml2Handler = new CustomSaml2SecurityTokenHandler();
            if (configuration != null)
            {
                IdentityConfiguration = configuration;
                IdentityConfiguration.SecurityTokenHandlers.AddOrReplace(Saml2Handler);
            }
        }

        public TokenFactory() : this(null)
        {
        }

        private SecurityTokenDescriptor BuildSAMLDescriptorUsingXspaProfile(TokenContract claims)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(XspaClaimTypes.SubjectIdentifier, claims.SubjectId),
                    new Claim(XspaClaimTypes.SubjectRole, claims.SubjectRole.ToString()),
                    new Claim(XspaClaimTypes.SubjectOrganization, claims.Organization),
                    new Claim(XspaClaimTypes.OrganizationIdentifier, claims.OrganizationId.ToString()),
                    new Claim(XspaClaimTypes.PurposeOfUse, claims.PurposeOfUse.ToString())
                }),
                TokenIssuerName = claims.SigningCertificate.SubjectName.Name,
                AppliesToAddress = IUAClaimTypes.AppliesToAddress,
                Lifetime = new Lifetime(DateTime.Now.ToUniversalTime(), claims.Expiration),
            };
            if (!String.IsNullOrEmpty(claims.Npi))
            {
                tokenDescriptor.Subject.AddClaim(new Claim(XspaClaimTypes.NationalProviderIdentifier, claims.Npi));
            }
            if (claims.HomeCommunityId != null)
            {
                tokenDescriptor.Subject.AddClaim(new Claim(XspaClaimTypes.HomeCommunityId,
                    claims.HomeCommunityId.ToString()));
            }
            return tokenDescriptor;
        }

        private SecurityTokenDescriptor BuildDescriptorUsingIUAProfile(TokenContract claims)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(IUAClaimTypes.JWTId, Guid.NewGuid().ToString()),
                    new Claim(IUAClaimTypes.Subject, claims.Subject),
                    new Claim(IUAClaimTypes.SubjectIdentifier, claims.SubjectId),
                    new Claim(IUAClaimTypes.SubjectRole, claims.SubjectRole.Code),
                    new Claim(IUAClaimTypes.SubjectOrganization, claims.Organization),
                    new Claim(IUAClaimTypes.OrganizationIdentifier, claims.OrganizationId.ToString()),
                    new Claim(IUAClaimTypes.PurposeOfUse, claims.PurposeOfUse.Code)
                }),
                TokenIssuerName = claims.SigningCertificate.SubjectName.Name,
                AppliesToAddress = IUAClaimTypes.AppliesToAddress,
                Lifetime = new Lifetime(DateTime.Now.ToUniversalTime(), claims.Expiration),
            };
            if (!String.IsNullOrEmpty(claims.Npi))
            {
                tokenDescriptor.Subject.AddClaim(new Claim(IUAClaimTypes.NationalProviderIdentifier, claims.Npi));
            }
            if (claims.HomeCommunityId != null)
            {
                tokenDescriptor.Subject.AddClaim(new Claim(XspaClaimTypes.HomeCommunityId,
                    claims.HomeCommunityId.ToString()));
            }
            return tokenDescriptor;
        }

        private SecurityTokenDescriptor BuildJWTDescriptorUsingXspaProfile(TokenContract claims)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(XspaClaimTypes.SubjectIdentifier, claims.SubjectId),
                    new Claim(XspaClaimTypes.SubjectRole, claims.SubjectRole.Code),
                    new Claim(XspaClaimTypes.SubjectOrganization, claims.Organization),
                    new Claim(XspaClaimTypes.OrganizationIdentifier, claims.OrganizationId.ToString()),
                    new Claim(XspaClaimTypes.PurposeOfUse, claims.PurposeOfUse.Code)
                }),
                TokenIssuerName = claims.SigningCertificate.SubjectName.Name,
                AppliesToAddress = IUAClaimTypes.AppliesToAddress,
                Lifetime = new Lifetime(DateTime.Now.ToUniversalTime(), claims.Expiration),
            };
            if (!String.IsNullOrEmpty(claims.Npi))
            {
                tokenDescriptor.Subject.AddClaim(new Claim(XspaClaimTypes.NationalProviderIdentifier, claims.Npi));
            }
            if (claims.HomeCommunityId != null)
            {
                tokenDescriptor.Subject.AddClaim(new Claim(XspaClaimTypes.HomeCommunityId,
                    claims.HomeCommunityId.ToString()));
            }
            return tokenDescriptor;
        }

        private SecurityTokenDescriptor BuildJWTDescriptorUsingCustomProfile(TokenContract claims)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(CustomXspaClaimTypes.SubjectIdentifier, claims.SubjectId),
                    new Claim(CustomXspaClaimTypes.SubjectRole, claims.SubjectRole.Code),
                    new Claim(CustomXspaClaimTypes.SubjectOrganization, claims.Organization),
                    new Claim(CustomXspaClaimTypes.OrganizationIdentifier, claims.OrganizationId.ToString()),
                    new Claim(CustomXspaClaimTypes.PurposeOfUse, claims.PurposeOfUse.Code),
                    new Claim(CustomXspaClaimTypes.PayLoadHash, claims.PayLoadHash)
                }),
                TokenIssuerName = claims.SigningCertificate.SubjectName.Name,
                AppliesToAddress = CustomXspaClaimTypes.AppliesToAddress,
                Lifetime = new Lifetime(DateTime.Now.ToUniversalTime(), claims.Expiration),
            };
            if (!String.IsNullOrEmpty(claims.Npi))
            {
                tokenDescriptor.Subject.AddClaim(new Claim(CustomXspaClaimTypes.NationalProviderIdentifier, claims.Npi));
            }
            if (claims.HomeCommunityId != null)
            {
                tokenDescriptor.Subject.AddClaim(new Claim(CustomXspaClaimTypes.HomeCommunityId,
                    claims.HomeCommunityId.ToString()));
            }
            return tokenDescriptor;
        }

        private JwtSecurityToken BuildJwtToken(JWTTokenContract contract)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityTokenDescriptor tokenDescriptor;
            switch (contract.NameOption)
            {
                case NamingProtocol.IUA:
                    tokenDescriptor = BuildDescriptorUsingIUAProfile(contract);
                    break;
                case NamingProtocol.Xspa:
                    tokenDescriptor = BuildJWTDescriptorUsingXspaProfile(contract);
                    break;
                case NamingProtocol.Custom:
                    tokenDescriptor = BuildJWTDescriptorUsingCustomProfile(contract);
                    break;
                default:
                    throw new ArgumentException("Missing naming option", "contract");
            }
            tokenDescriptor.TokenType = "JWT";
            tokenDescriptor.SigningCredentials = new X509SigningCredentials(contract.SigningCertificate);
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return token as JwtSecurityToken;
        }

        public T GenerateToken<T>(TokenContract contract) where T : SecurityToken
        {
            if (contract is SamlTokenContract)
            {
                return BuildSamlToken(contract as SamlTokenContract) as T;
            }
            if (contract is JWTTokenContract)
            {
                return BuildJwtToken(contract as JWTTokenContract) as T;
            }
            throw new ArgumentException(String.Format("Unsupported token contract: {0}", contract.GetType().Name),
                "contract");
        }

        public bool ValidateToken(string token)
        {
            return false;
        }

        private GenericXmlSecurityToken BuildSamlToken<T>(T contract) where T : SamlTokenContract
        {
            var signingCredentials = SetSigningCredentials(contract);

            SecurityTokenDescriptor tokenDescriptor = BuildSAMLDescriptorUsingXspaProfile(contract);
            tokenDescriptor.TokenType = CustomSaml2TokenConstants.SAML2TokenType;
            tokenDescriptor.SigningCredentials = signingCredentials;

            if (contract.EncryptingCertificate != null)
            {
                var encryptingCredentials = new EncryptedKeyEncryptingCredentials(contract.EncryptingCertificate,
                    contract.AlgorithmSuite.DefaultAsymmetricKeyWrapAlgorithm,
                    contract.AlgorithmSuite.DefaultEncryptionKeyDerivationLength,
                    contract.AlgorithmSuite.DefaultEncryptionAlgorithm);
                tokenDescriptor.EncryptingCredentials = encryptingCredentials;
            }

            SetConfirmationMethod(contract, tokenDescriptor);

            tokenDescriptor.AddAuthenticationClaims(contract.AuthenticationContext);
            var samlToken = Saml2Handler.CreateToken(tokenDescriptor) as Saml2SecurityToken;

            if (samlToken == null)
            {
                throw new Exception("Failed to create Saml2 Security token");
            }

            return SetSecurityToken(contract, samlToken, Saml2Handler, tokenDescriptor);
        }

        private GenericXmlSecurityToken SetSecurityToken<T>(T contract, Saml2SecurityToken samlToken,
            CustomSaml2SecurityTokenHandler tokenHandler, SecurityTokenDescriptor tokenDescriptor)
            where T : SamlTokenContract
        {
            GenericXmlSecurityToken xmlToken;
            var outputTokenString = samlToken.ToTokenXmlString();
            var attachedReference = tokenHandler.CreateSecurityTokenReference(samlToken, true);
            var unattachedReference = tokenHandler.CreateSecurityTokenReference(samlToken, false);
            if (contract.Confirmation == SubjectConfirmationMethod.HolderOfKey)
            {
                if (contract is AsymmetricSamlTokenContract)
                {
                    xmlToken = new GenericXmlSecurityToken(
                        GetElement(outputTokenString),
                        new X509SecurityToken(contract.SigningCertificate),
                        DateTime.UtcNow,
                        DateTime.UtcNow.AddHours(8),
                        attachedReference,
                        unattachedReference,
                        new ReadOnlyCollection<IAuthorizationPolicy>(new List<IAuthorizationPolicy>()));
                }
                else if (contract is SymmetricSamlTokenContract)
                {
                    var proof = (SymmetricProofDescriptor) tokenDescriptor.Proof;
                    xmlToken = new GenericXmlSecurityToken(
                        GetElement(outputTokenString),
                        new BinarySecretSecurityToken(proof.GetKeyBytes()),
                        DateTime.UtcNow,
                        DateTime.UtcNow.AddHours(8),
                        attachedReference,
                        unattachedReference,
                        new ReadOnlyCollection<IAuthorizationPolicy>(new List<IAuthorizationPolicy>()));
                }
                else
                {
                    throw new InvalidOperationException("Unsupported Holder-of-Key contract: " + contract.GetType().Name);
                }
            }
            else
            {
                xmlToken = new GenericXmlSecurityToken(
                    GetElement(outputTokenString),
                    null,
                    DateTime.UtcNow,
                    DateTime.UtcNow.AddHours(8),
                    attachedReference,
                    unattachedReference,
                    new ReadOnlyCollection<IAuthorizationPolicy>(new List<IAuthorizationPolicy>()));
            }
            return xmlToken;
        }

        private static void SetConfirmationMethod<T>(T contract, SecurityTokenDescriptor tokenDescriptor)
            where T : SamlTokenContract
        {
            switch (contract.Confirmation)
            {
                case SubjectConfirmationMethod.Bearer:
                    break;
                case SubjectConfirmationMethod.HolderOfKey:
                    tokenDescriptor.Proof = (contract is AsymmetricSamlTokenContract)
                        ? (ProofDescriptor) CreateAsymmetricProofDescriptor(contract.SigningCertificate)
                        : CreateSymmetricProofDescriptor(contract.SigningCertificate,
                            contract.AlgorithmSuite.DefaultSymmetricKeyLength);
                    break;
                case SubjectConfirmationMethod.SenderVouches:
                    throw new NotSupportedException("SenderVouches is not supported.");
            }
        }

        private static SigningCredentials SetSigningCredentials<T>(T contract) where T : SamlTokenContract
        {
            SigningCredentials signingCredentials;
            if (contract.UseRsa)
            {
                var rsa = contract.SigningCertificate.PrivateKey as RSACryptoServiceProvider;
                if (rsa == null)
                {
                    throw new InvalidOperationException(
                        "Signing certificate must include private key for RSA signature.");
                }
                var rsaKey = new RsaSecurityKey(rsa);
                var rsaClause = new RsaKeyIdentifierClause(rsa);
                var ski = new SecurityKeyIdentifier(new SecurityKeyIdentifierClause[] {rsaClause});
                signingCredentials = new SigningCredentials(rsaKey,
                    contract.AlgorithmSuite.DefaultAsymmetricSignatureAlgorithm,
                    contract.AlgorithmSuite.DefaultDigestAlgorithm, ski);
            }
            else
            {
                var clause =
                    new X509SecurityToken(contract.SigningCertificate)
                        .CreateKeyIdentifierClause<X509RawDataKeyIdentifierClause>();
                var ski = new SecurityKeyIdentifier(clause);
                signingCredentials = new X509SigningCredentials(contract.SigningCertificate, ski,
                    contract.AlgorithmSuite.DefaultAsymmetricSignatureAlgorithm,
                    contract.AlgorithmSuite.DefaultDigestAlgorithm);
            }
            return signingCredentials;
        }

        private static SymmetricProofDescriptor CreateSymmetricProofDescriptor(X509Certificate2 encryptingCertificate,
            int keySize)
        {
            return new SymmetricProofDescriptor(keySize, new X509EncryptingCredentials(encryptingCertificate));
        }

        private static AsymmetricProofDescriptor CreateAsymmetricProofDescriptor(X509Certificate2 proofCert)
        {
            var clause =
                new X509SecurityToken(proofCert).CreateKeyIdentifierClause<X509RawDataKeyIdentifierClause>();
            var proofSki = new SecurityKeyIdentifier(clause);
            return new AsymmetricProofDescriptor(proofSki);
        }

        private XmlElement GetElement(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc.DocumentElement;
        }
    }
}