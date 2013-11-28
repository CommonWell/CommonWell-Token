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
    /// <summary>Claims defined in the IHE Internet User Authorization (IUA) profile.</summary>
    public static class IUAClaimTypes
    {
        /// <summary>The jti (JWT ID) claim provides a unique identifier for the JWT.</summary>
        /// <remarks>The identifier value MUST be assigned in a manner that ensures that there is a negligible probability that the same value will be accidentally assigned to a different data object. The jti claim can be used to prevent the JWT from being replayed. The jti value is a case sensitive string. Use of this claim is REQUIRED per the IHE Internet User Authorization (IUA) profile.</remarks>
        public const string JWTId = "jti";

        /// <summary>The sub (subject) claim identifies the principal that is the subject of the JWT.</summary>
        /// <remarks>The Claims in a JWT are normally statements about the subject. The processing of this claim is generally application specific. The sub value is a case sensitive string containing a StringOrURI value. Use of this claim is REQUIRED per the IHE Internet User Authorization (IUA) profile.</remarks>
        public const string Subject = "sub";

        /// <summary>Plain text user's name.</summary>
        public const string SubjectIdentifier = "SubjectID";

        /// <summary>Plain text description of the organization.</summary>
        public const string SubjectOrganization = "SubjectOrganization";

        /// <summary>SNOMED Code identifying subject role.</summary>
        public const string SubjectRole = "SubjectRole";

        /// <summary>Purpose of use for the request.</summary>
        public const string PurposeOfUse = "PurposeOfUse";

        /// <summary>The Organization Identifer value is an OID in urn format (e.g., urn:oid:1.2.3.4.5.6).</summary>
        public const string OrganizationIdentifier = "SubjectOrganizationID";

        /// <summary>Home Community ID where request originated. MUST be in urn format.</summary>
        public const string HomeCommunityID = "HomeCommunityID";

        /// <summary>Phsyician's NPI.</summary>
        public const string NationalProviderIdentifier = "NationalProviderIdentifier";

        /// <summary>The address that a claim applies to.</summary>
        public const string AppliesToAddress = "urn:commonwellalliance.org";
    }
}