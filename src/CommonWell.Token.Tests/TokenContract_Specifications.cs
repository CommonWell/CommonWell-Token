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
    public class TokenContract_specifications
    {
        [SetUp]
        public void Initialize()
        {
            _testInstance = InitializedFixture.Instance;
        }

        private InitializedFixture _testInstance = null;

        [Test]
        public void Can_Set_HomeCommunityId_To_Valid_Uri()
        {
            // Arrange
            var contract = Utilities.SetBasicSAMLContract<BearerSamlTokenContract>(null);

            // Act
            contract.HomeCommunityId = TestClaims.HomeCommunityIdClaim;

            //Assert
            Assert.AreSame(contract.HomeCommunityId, TestClaims.HomeCommunityIdClaim);
        }

        [Test]
        public void Can_Set_OrganizationId_To_Valid_Uri()
        {
            // Arrange
            var contract = Utilities.SetBasicJWTContract(null);

            // Act
            contract.OrganizationId = TestClaims.OrganizationIdClaim;

            //Assert
            Assert.AreSame(contract.OrganizationId, TestClaims.OrganizationIdClaim);
        }
    }
}