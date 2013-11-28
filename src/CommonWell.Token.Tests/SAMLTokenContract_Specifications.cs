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
using NUnit.Framework;

namespace CommonWell.Token.Tests
{
    [TestFixture]
    public class SAMLTokenContract_specifications
    {
        [SetUp]
        public void Initialize()
        {
            _testInstance = InitializedFixture.Instance;
        }

        private InitializedFixture _testInstance = null;

        [Test]
        public void Authentication_Context_Is_Set_Correctly()
        {
            // Arrange and Act
            var contract = Utilities.SetBasicSAMLContract<BearerSamlTokenContract>(null);
            contract.AuthenticationContext = AuthenticationContextClasses.TLSClient;

            // Assert
            Assert.AreEqual(contract.AuthenticationContext, AuthenticationContextClasses.TLSClient);
        }

        [Test]
        public void Authentication_Context_Is_Valid_Uri()
        {
            foreach (var fi in typeof (AuthenticationContextClasses).GetFields())
            {
                Uri testUri;
                if (Uri.TryCreate(fi.GetRawConstantValue().ToString(), UriKind.Absolute, out testUri) == false)
                {
                    Assert.Fail(fi.GetRawConstantValue() + " is not a valid Uri.");
                }
            }
        }

        [Test]
        public void Invalid_Authentication_Context_Causes_Error()
        {
            // Arrange
            var contract = Utilities.SetBasicSAMLContract<BearerSamlTokenContract>(null);

            // Act
            Assert.Throws<ArgumentException>(() => contract.AuthenticationContext = "Some Bogus Value",
                "Argument exception not thrown for invalid Authentication Context");
        }

        [Test]
        public void Missing_Authentication_Context_Defaults_To_Unspecified()
        {
            // Arrange and Act
            var contract = Utilities.SetBasicSAMLContract<BearerSamlTokenContract>(null);

            // Assert
            Assert.AreEqual(contract.AuthenticationContext, AuthenticationContextClasses.Unspecified);
        }
    }
}