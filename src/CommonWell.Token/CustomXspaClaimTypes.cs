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
    /// <summary>Claims defined (mostly) in the Cross-Enterprise Security and Privacy Authorization (XSPA) profile, and referenced in the NHIN/Healtheway Authorization Framework (v3.0) specification.</summary>
    public class CustomXspaClaimTypes
    {
        /// <summary>urn:oasis:names:tc:xspa:1.0:subject:subject-id</summary>
        /// <remarks>The value name of the user as required by HIPAA Privacy Disclosure Accounting.</remarks>
        public const string SubjectIdentifier = "urn:oasis:names:tc:xspa:1.0:subject:subject-id";

        /// <summary>urn:oasis:names:tc:xspa:1.0:subject:organization</summary>
        /// <remarks>In plain text, the organization that the user belongs to as required by HIPAA Privacy Disclosure Accounting.</remarks>
        public const string SubjectOrganization = "urn:oasis:names:tc:xspa:1.0:subject:organization";

        /// <summary>urn:oasis:names:tc:xacml:2.0:subject:role</summary>
        /// <remarks>The SNOMED CT value representing the role that the user is playing when making the request.</remarks>
        public const string SubjectRole = "urn:oasis:names:tc:xacml:2.0:subject:role";

        /// <summary>urn:oasis:names:tc:xspa:1.0:subject:purposeofuse</summary>
        /// <remarks>The coded representation of the Purpose for Use that is in effect for the request.</remarks>
        public const string PurposeOfUse = "urn:oasis:names:tc:xspa:1.0:subject:purposeofuse";

        /// <summary>urn:oasis:names:tc:xspa:1.0:subject:organization-id</summary>
        /// <remarks>This organization ID shall be consistent with the plain-text name of the organization provided in the User Organization Attribute. For CommonWell, the organization ID is an Object Identifier (OID); and is provided here in urn format (that is, "urn:oid:" appended with the OID).</remarks>
        public const string OrganizationIdentifier = "urn:oasis:names:tc:xspa:1.0:subject:organization-id";

        /// <summary>urn:oasis:names:tc:xspa:2.0:subject:npi</summary>
        /// <remarks>A unique 10-digit identification number issued to health care providers in the United States by the Centers for Medicare and Medicaid Services (CMS).</remarks>
        public const string NationalProviderIdentifier = "urn:oasis:names:tc:xspa:2.0:subject:npi";

        /// <summary>urn:nhin:names:saml:homeCommunityId</summary>
        /// <remarks>The Home Community ID assigned to the organization that is initiating the request, using the urn format (that is, "urn:oid:" appended with the OID).</remarks>
        public const string HomeCommunityId = "urn:nhin:names:saml:homeCommunityId";

        /// <summary>urn:commonwellalliance.org</summary>
        /// <remarks>The address that a claim applies to.</remarks>
        public const string AppliesToAddress = "urn:commonwellalliance.org";
        /// <summary>urn:payloadhash</summary>
        /// <remarks>Hash of the pay load.</remarks>
        public const string PayLoadHash = "urn:payload-hash";
    }
}