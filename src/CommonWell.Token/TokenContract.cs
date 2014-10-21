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
using System.Security.Cryptography.X509Certificates;

namespace CommonWell.Token
{
    public abstract class TokenContract
    {
        public DateTime Expiration;
        public Uri HomeCommunityId = null;
        public string Issuer;
        public string Npi;
        public string Organization;
        public Uri OrganizationId = null;
        public PurposeOfUseClaim PurposeOfUse;
        public X509Certificate2 SigningCertificate;
        public string Subject;
        public string SubjectId;
        public RoleClaim SubjectRole;
        public string PayLoadHash;
    }
}