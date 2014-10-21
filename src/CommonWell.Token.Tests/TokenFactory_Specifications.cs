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
using System.IdentityModel.Tokens;
using System.Linq;
using NUnit.Framework;

namespace CommonWell.Token.Tests
{
    [TestFixture]
    public class TokenFactory_specifications
    {
        [SetUp]
        public void Initialize()
        {
            _testInstance = InitializedFixture.Instance;
        }

        private InitializedFixture _testInstance;

        [Test]
        public void Can_Generate_JWT_Token()
        {
            // Arrange
            var contract = Utilities.SetBasicJWTContract(_testInstance.X509Certificate);

            // Act
            var factory = new TokenFactory(_testInstance.IdentityConfigurationForTest);
            var token = factory.GenerateToken<JwtSecurityToken>(contract);

            // Assert
            Assert.IsNotNull(token, "Token came back empty");
            Assert.IsTrue(token.Audience.Equals(TestClaims.Audience));
            Assert.AreSame(token.Issuer, contract.Issuer, "Issuer incorrect");
            Assert.IsTrue(token.Header.Any(h => h.Key.Equals("typ") && h.Value.Equals("JWT")), "Invalid type");
            Assert.IsTrue(token.Header.Any(h => h.Key.Equals("alg") && h.Value.Equals("RS256")), "Invalid algorithm");
            Assert.IsTrue(
                token.Claims.Any(
                    c => c.Type.Equals(XspaClaimTypes.SubjectIdentifier) && c.Value.Equals(TestClaims.SubjectClaim)),
                "Subject Identifier incorrect");
            Assert.IsTrue(
                token.Claims.Any(
                    c =>
                        c.Type.Equals(XspaClaimTypes.OrganizationIdentifier) &&
                        c.Value.Equals(TestClaims.OrganizationIdClaim.ToString())),
                "Organization Identifier incorrect");
            Assert.IsTrue(
                token.Claims.Any(
                    c =>
                        c.Type.Equals(XspaClaimTypes.SubjectOrganization) &&
                        c.Value.Equals(TestClaims.OrganizationClaim)), "Subject Organization incorrect");
            Assert.IsTrue(
                token.Claims.Any(
                    c => c.Type.Equals(XspaClaimTypes.SubjectRole) && c.Value.Equals(TestClaims.SubjectRoleClaim.Code)),
                "Subject Role incorrect");
            Assert.IsTrue(
                token.Claims.Any(
                    c => c.Type.Equals(XspaClaimTypes.PurposeOfUse) && c.Value.Equals(TestClaims.PurposeOfUseClaim.Code)),
                "Purpose of Use incorrect");
            Assert.IsTrue(
                token.Claims.Any(
                    c => c.Type.Equals(XspaClaimTypes.NationalProviderIdentifier) && c.Value.Equals(TestClaims.NPIClaim)),
                "Npi incorrect");
            Assert.IsTrue(token.Expiration.HasValue, "Expiration is null");
            Assert.IsNotNull(token.Expiration);
            Assert.AreEqual(_testInstance.ValidExpiration.ToShortDateString(),
                Utilities.UnixTimeStampToDateTime(token.Expiration.Value).ToShortDateString());
            Assert.AreEqual(_testInstance.ValidExpiration.ToShortTimeString(),
                Utilities.UnixTimeStampToDateTime(token.Expiration.Value).ToShortTimeString());
            Assert.AreNotSame(_testInstance.ValidExpiration, Utilities.UnixTimeStampToDateTime(token.Expiration.Value));
        }

        [Test]
        public void Can_Generate_JWT_Token_With_IUA_Claims()
        {
            // Arrange
            var contract = Utilities.SetBasicJWTContract(_testInstance.X509Certificate);
            contract.NameOption = NamingProtocol.IUA;

            // Act
            var factory = new TokenFactory(_testInstance.IdentityConfigurationForTest);
            var token = factory.GenerateToken<JwtSecurityToken>(contract);

            // Assert
            Assert.IsNotNull(token, "Token came back empty");
            Assert.IsTrue(token.Audience.Equals(TestClaims.Audience));
            Assert.IsTrue(token.Issuer.Equals(contract.Issuer), "Issuer incorrect");
            Assert.IsTrue(token.Header.Any(h => h.Key.Equals("typ") && h.Value.Equals("JWT")), "Invalid type");
            Assert.IsTrue(token.Header.Any(h => h.Key.Equals("alg") && h.Value.Equals("RS256")), "Invalid algorithm");
            Assert.IsTrue(
                token.Claims.Any(
                    c => c.Type.Equals(IUAClaimTypes.SubjectIdentifier) && c.Value.Equals(TestClaims.SubjectClaim)),
                "Subject Identifier incorrect");
            Assert.IsTrue(
                token.Claims.Any(
                    c =>
                        c.Type.Equals(IUAClaimTypes.OrganizationIdentifier) &&
                        c.Value.Equals(TestClaims.OrganizationIdClaim.ToString())),
                "Organization Identifier incorrect");
            Assert.IsTrue(
                token.Claims.Any(
                    c =>
                        c.Type.Equals(IUAClaimTypes.SubjectOrganization) &&
                        c.Value.Equals(TestClaims.OrganizationClaim)), "Subject Organization incorrect");
            Assert.IsTrue(
                token.Claims.Any(
                    c => c.Type.Equals(IUAClaimTypes.SubjectRole) && c.Value.Equals(TestClaims.SubjectRoleClaim.Code)),
                "Subject Role incorrect");
            Assert.IsTrue(
                token.Claims.Any(
                    c => c.Type.Equals(IUAClaimTypes.PurposeOfUse) && c.Value.Equals(TestClaims.PurposeOfUseClaim.Code)),
                "Purpose of Use incorrect");
            Assert.IsTrue(
                token.Claims.Any(
                    c => c.Type.Equals(IUAClaimTypes.NationalProviderIdentifier) && c.Value.Equals(TestClaims.NPIClaim)),
                "Npi incorrect");
            Assert.IsNotNull(token.Expiration, "Expiration is null");
            Assert.AreEqual(_testInstance.ValidExpiration.ToShortDateString(),
                Utilities.UnixTimeStampToDateTime(token.Expiration.Value).ToShortDateString());
            Assert.AreEqual(_testInstance.ValidExpiration.ToShortTimeString(),
                Utilities.UnixTimeStampToDateTime(token.Expiration.Value).ToShortTimeString());
            Assert.AreNotEqual(_testInstance.ValidExpiration, Utilities.UnixTimeStampToDateTime(token.Expiration.Value));
            // offset by skew
        }

        [Test]
        public void Can_Generate_Saml2_Asymmetric_Token()
        {
            // Arrange
            var contract = Utilities.SetBasicSAMLContract<AsymmetricSamlTokenContract>(_testInstance.X509Certificate);

            // Act
            var factory = new TokenFactory(_testInstance.IdentityConfigurationForTest);
            var wsTrustToken = factory.GenerateToken<GenericXmlSecurityToken>(contract);

            // Assert
            Assert.IsNotNull(wsTrustToken, "Outer token came back empty");
            Assert.IsNotEmpty(wsTrustToken.SecurityKeys);
            Assert.IsNotNull(wsTrustToken.SecurityKeys);
            Assert.IsTrue(wsTrustToken.SecurityKeys.Count > 0);
            var samlToken = (Saml2SecurityToken) wsTrustToken.ToSecurityToken();
            Assert.IsNotNull(samlToken, "SAML token came back empty");
            var statement =
                samlToken.Assertion.Statements.Where(s => s is Saml2AttributeStatement).Select(s => s).FirstOrDefault()
                    as
                    Saml2AttributeStatement;
            Assert.IsNotNull(statement);

            Assert.IsTrue(
                statement.Attributes.Any(
                    c =>
                        c.Name.Equals(XspaClaimTypes.SubjectIdentifier) &&
                        c.Values.Any(v => v.Equals(TestClaims.SubjectClaim))), "Subject Identifier incorrect");
            Assert.IsTrue(
                statement.Attributes.Any(
                    c =>
                        c.Name.Equals(XspaClaimTypes.OrganizationIdentifier) &&
                        c.Values.Any(v => v.Equals(TestClaims.OrganizationIdClaim.ToString()))),
                "Organization Identifier incorrect");
            Assert.IsTrue(
                statement.Attributes.Any(
                    c =>
                        c.Name.Equals(XspaClaimTypes.SubjectOrganization) &&
                        c.Values.Any(v => v.Equals(TestClaims.OrganizationClaim))), "Subject Organization incorrect");
            Assert.IsTrue(
                statement.Attributes.Any(
                    c =>
                        c.Name.Equals(XspaClaimTypes.SubjectRole) &&
                        c.Values.Any(v => v.Contains(TestClaims.SubjectRoleClaim.Code))),
                "Subject Role incorrect");
            Assert.IsTrue(
                statement.Attributes.Any(
                    c =>
                        c.Name.Equals(XspaClaimTypes.PurposeOfUse) &&
                        c.Values.Any(v => v.Contains(TestClaims.PurposeOfUseClaim.Code))),
                "Purpose of Use incorrect");
            Assert.IsTrue(
                statement.Attributes.Any(
                    c =>
                        c.Name.Equals(XspaClaimTypes.NationalProviderIdentifier) &&
                        c.Values.Any(v => v.Equals(TestClaims.NPIClaim))), "Npi incorrect");
        }

        [Test]
        public void Can_Generate_Saml2_Bearer_Token()
        {
            // Arrange
            var contract = Utilities.SetBasicSAMLContract<BearerSamlTokenContract>(_testInstance.X509Certificate);

            // Act
            var factory = new TokenFactory(_testInstance.IdentityConfigurationForTest);
            var wsTrustToken = factory.GenerateToken<GenericXmlSecurityToken>(contract);

            // Assert
            Assert.IsNotNull(wsTrustToken, "Outer token came back empty");
            Assert.IsEmpty(wsTrustToken.SecurityKeys);
            var samlToken = (Saml2SecurityToken) wsTrustToken.ToSecurityToken();
            Assert.IsNotNull(samlToken, "SAML token came back empty");

            var statement =
                samlToken.Assertion.Statements.Where(s => s is Saml2AttributeStatement).Select(s => s).FirstOrDefault()
                    as
                    Saml2AttributeStatement;
            Assert.IsNotNull(statement);

            Assert.IsTrue(
                statement.Attributes.Any(
                    c =>
                        c.Name.Equals(XspaClaimTypes.SubjectIdentifier) &&
                        c.Values.Any(v => v.Equals(TestClaims.SubjectClaim))), "Subject Identifier incorrect");
            Assert.IsTrue(
                statement.Attributes.Any(
                    c =>
                        c.Name.Equals(XspaClaimTypes.OrganizationIdentifier) &&
                        c.Values.Any(v => v.Equals(TestClaims.OrganizationIdClaim.ToString()))),
                "Organization Identifier incorrect");
            Assert.IsTrue(
                statement.Attributes.Any(
                    c =>
                        c.Name.Equals(XspaClaimTypes.SubjectOrganization) &&
                        c.Values.Any(v => v.Equals(TestClaims.OrganizationClaim))), "Subject Organization incorrect");
            Assert.IsTrue(
                statement.Attributes.Any(
                    c =>
                        c.Name.Equals(XspaClaimTypes.SubjectRole) &&
                        c.Values.Any(v => v.Contains(TestClaims.SubjectRoleClaim.Code))),
                "Subject Role incorrect");
            Assert.IsTrue(
                statement.Attributes.Any(
                    c =>
                        c.Name.Equals(XspaClaimTypes.PurposeOfUse) &&
                        c.Values.Any(v => v.Contains(TestClaims.PurposeOfUseClaim.Code))),
                "Purpose of Use incorrect");
            Assert.IsTrue(
                statement.Attributes.Any(
                    c =>
                        c.Name.Equals(XspaClaimTypes.NationalProviderIdentifier) &&
                        c.Values.Any(v => v.Equals(TestClaims.NPIClaim))), "Npi incorrect");

            var output = samlToken.Write(_testInstance.IdentityConfigurationForTest);
            Console.WriteLine(output);
        }

        [Test]
        public void Can_Generate_Saml2_Symmetric_Token()
        {
            // Arrange
            var contract = Utilities.SetBasicSAMLContract<SymmetricSamlTokenContract>(_testInstance.X509Certificate);

            // Act
            var factory = new TokenFactory(_testInstance.IdentityConfigurationForTest);
            var wsTrustToken = factory.GenerateToken<GenericXmlSecurityToken>(contract);

            // Assert
            Assert.IsNotNull(wsTrustToken, "Outer token came back empty");
            Assert.IsNotEmpty(wsTrustToken.SecurityKeys);
            Assert.IsNotNull(wsTrustToken.SecurityKeys);
            Assert.IsTrue(wsTrustToken.SecurityKeys.Count > 0);
            var samlToken = (Saml2SecurityToken) wsTrustToken.ToSecurityToken();
            Assert.IsNotNull(samlToken, "SAML token came back empty");
            var statement =
                samlToken.Assertion.Statements.Where(s => s is Saml2AttributeStatement).Select(s => s).FirstOrDefault()
                    as
                    Saml2AttributeStatement;
            Assert.IsNotNull(statement);

            Assert.IsTrue(
                statement.Attributes.Any(
                    c =>
                        c.Name.Equals(XspaClaimTypes.SubjectIdentifier) &&
                        c.Values.Any(v => v.Equals(TestClaims.SubjectClaim))), "Subject Identifier incorrect");
            Assert.IsTrue(
                statement.Attributes.Any(
                    c =>
                        c.Name.Equals(XspaClaimTypes.OrganizationIdentifier) &&
                        c.Values.Any(v => v.Equals(TestClaims.OrganizationIdClaim.ToString()))),
                "Organization Identifier incorrect");
            Assert.IsTrue(
                statement.Attributes.Any(
                    c =>
                        c.Name.Equals(XspaClaimTypes.SubjectOrganization) &&
                        c.Values.Any(v => v.Equals(TestClaims.OrganizationClaim))), "Subject Organization incorrect");
            Assert.IsTrue(
                statement.Attributes.Any(
                    c =>
                        c.Name.Equals(XspaClaimTypes.SubjectRole) &&
                        c.Values.Any(v => v.Contains(TestClaims.SubjectRoleClaim.Code))),
                "Subject Role incorrect");
            Assert.IsTrue(
                statement.Attributes.Any(
                    c =>
                        c.Name.Equals(XspaClaimTypes.PurposeOfUse) &&
                        c.Values.Any(v => v.Contains(TestClaims.PurposeOfUseClaim.Code))),
                "Purpose of Use incorrect");
            Assert.IsTrue(
                statement.Attributes.Any(
                    c =>
                        c.Name.Equals(XspaClaimTypes.NationalProviderIdentifier) &&
                        c.Values.Any(v => v.Equals(TestClaims.NPIClaim))), "Npi incorrect");
        }

        [Test]
        public void Can_Write_JWT_Token()
        {
            // Arrange
            var contract = Utilities.SetBasicJWTContract(_testInstance.X509Certificate);

            // Act
            var factory = new TokenFactory(_testInstance.IdentityConfigurationForTest);
            var token = factory.GenerateToken<JwtSecurityToken>(contract);
            string tokenString = token.Write();

            // Assert
            Assert.IsNotEmpty(tokenString);
            Assert.AreEqual(tokenString.Count(i => i.Equals('.')), 2, "Encoded token string delimiter count failure");
        }

        [Test]
        public void Can_Generate_JWT_Token_With_Custom_Claims()
        {
            // Arrange
            var contract = Utilities.SetCustomJWTContract(_testInstance.X509Certificate);
            contract.NameOption = NamingProtocol.Custom;

            // Act
            var factory = new TokenFactory(_testInstance.IdentityConfigurationForTest);
            var token = factory.GenerateToken<JwtSecurityToken>(contract);

            // Assert
            Assert.IsNotNull(token, "Token came back empty");
            Assert.IsTrue(token.Audience.Equals(TestClaims.Audience));
            Assert.IsTrue(token.Issuer.Equals(contract.Issuer), "Issuer incorrect");
            Assert.IsTrue(token.Header.Any(h => h.Key.Equals("typ") && h.Value.Equals("JWT")), "Invalid type");
            Assert.IsTrue(token.Header.Any(h => h.Key.Equals("alg") && h.Value.Equals("RS256")), "Invalid algorithm");
            Assert.IsTrue(
                token.Claims.Any(
                    c => c.Type.Equals(XspaClaimTypes.SubjectIdentifier) && c.Value.Equals(TestClaims.SubjectClaim)),
                "Subject Identifier incorrect");
            Assert.IsTrue(
                token.Claims.Any(
                    c =>
                        c.Type.Equals(CustomXspaClaimTypes.OrganizationIdentifier) &&
                        c.Value.Equals(TestClaims.OrganizationIdClaim.ToString())),
                "Organization Identifier incorrect");
            Assert.IsTrue(
                token.Claims.Any(
                    c =>
                        c.Type.Equals(CustomXspaClaimTypes.SubjectOrganization) &&
                        c.Value.Equals(TestClaims.OrganizationClaim)), "Subject Organization incorrect");
            Assert.IsTrue(
                token.Claims.Any(
                    c => c.Type.Equals(CustomXspaClaimTypes.SubjectRole) && c.Value.Equals(TestClaims.SubjectRoleClaim.Code)),
                "Subject Role incorrect");
            Assert.IsTrue(
                token.Claims.Any(
                    c => c.Type.Equals(CustomXspaClaimTypes.PurposeOfUse) && c.Value.Equals(TestClaims.PurposeOfUseClaim.Code)),
                "Purpose of Use incorrect");
            Assert.IsTrue(
                token.Claims.Any(
                    c => c.Type.Equals(CustomXspaClaimTypes.NationalProviderIdentifier) && c.Value.Equals(TestClaims.NPIClaim)),
                "Npi incorrect");
            Assert.IsTrue(
             token.Claims.Any(
                 c => c.Type.Equals(CustomXspaClaimTypes.PayLoadHash) && c.Value.Equals(TestClaims.PayLoadHash)),
             "Npi incorrect");
            Assert.IsNotNull(token.Expiration, "Expiration is null");
            Assert.AreEqual(_testInstance.ValidExpiration.ToShortDateString(),
                Utilities.UnixTimeStampToDateTime(token.Expiration.Value).ToShortDateString());
            Assert.AreEqual(_testInstance.ValidExpiration.ToShortTimeString(),
                Utilities.UnixTimeStampToDateTime(token.Expiration.Value).ToShortTimeString());
            Assert.AreNotEqual(_testInstance.ValidExpiration, Utilities.UnixTimeStampToDateTime(token.Expiration.Value));
            // offset by skew
        }

        
    }
}