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
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;

namespace CommonWell.Token
{
    /// <summary>In SAML 2.0, the Subject Confirmation element provides the means for a relying party to verify the correspondence of the subject of the assertion with the party with whom the relying party is communicating. The Method attribute indicates the specific method that the relying party should use to make this determination.</summary>
    /// <remarks>The SAML profile of WSS: SOAP Message Security requires that systems support the holder-of-key and sender-vouches methods of subject confirmation. It is strongly RECOMMENDED that an XML signature be used to establish the relationship between the message and the statements of the attached assertions. This is especially RECOMMENDED whenever the SOAP message exchange is conducted over an unprotected transport.</remarks>
    public enum SubjectConfirmationMethod
    {
        /// <summary>In the bearer model, the relying party will allow any party that bears the Assertion (assuming any other constraints are also met) to use the assertion (and thereby lay claim to some relationship with the subject within). Value: <c>urn:oasis:names:tc:SAML:2.0:cm:bearer</c></summary>
        Bearer,

        /// <summary>For this confirmation method, the outgoing soap envelope inlcudes a digital signature  signed by the SOAP client using either a symmetric or asymmetric digital signature. Value: <c>urn:oasis:names:tc:SAML:2.0:cm:holder-of-key</c></summary>
        HolderOfKey,

        /// <summary>A user is authenticated with an intermediary using another mechanism such as username/password tokens. After this is done, the intermediary requests a SAML token from an STS. The SAML token will contain the user's identity but, unlike the HOK method, no key information. Value: <c>urn:oasis:names:tc:SAML:2.0:cm:sender-vouches</c></summary>
        SenderVouches
    }

    /// <summary>For <see cref="HolderOfKey">HolderOfKey</see> tokens, specifies how the associated digital signature will be signed and verified.</summary>
    /// <remarks>A digital signature is a value computed with a cryptographic algorithm and bound to data in such a way that intended recipients of the data can use the digital signature to verify that the data has not been altered and/or has originated from the signer of the message, providing message integrity and authentication. The digital signature can be computed and verified with symmetric key algorithms, where the same key is used for signing and verifying, or with asymmetric key algorithms, where different keys are used for signing and verifying (a private and public key pair are used).</remarks>
    public enum HolderOfKey
    {
        /// <summary>The same public key is used for signing and verifying the digital signature.</summary>
        PublicKey,

        /// <summary>A private key is used for signing the digital signature, and verified using the public key held by the relying party.</summary>
        SymmetricKey
    }

    /// <summary>An abstract class representing a specification for creating a SAML 2 security token.</summary>
    public abstract class SamlTokenContract : TokenContract
    {
        public SecurityAlgorithmSuite AlgorithmSuite;
        public X509Certificate2 EncryptingCertificate;
        public bool UseRsa;
        private string _authenticationContext;
        public abstract SubjectConfirmationMethod Confirmation { get; }

        /// <summary>The authentication context for the SAML 2 token.</summary>
        /// <value>Must be one of the constants defined in <see cref="AuthenticationContextClasses" />.</value>
        /// <remarks>If not specified, defaults to <see cref="AuthenticationContextClasses.Unspecified" />.</remarks>
        public string AuthenticationContext
        {
            get
            {
                return String.IsNullOrEmpty(_authenticationContext)
                    ? AuthenticationContextClasses.Unspecified
                    : _authenticationContext;
            }
            set
            {
                bool isValid = false;
                if (
                    typeof (AuthenticationContextClasses).GetFields()
                        .Any(constant => constant.GetRawConstantValue().ToString().Equals(value)))
                {
                    _authenticationContext = value;
                    isValid = true;
                }
                if (!isValid)
                    throw new ArgumentException(
                        String.Format("'{0}' is not a valid authentication context (see {1}).", value,
                            typeof (AuthenticationContextClasses).Name));
            }
        }
    }

    /// <summary>A contract for creating a SAML 2 token with the <see cref="SubjectConfirmationMethod.Bearer">Bearer</see> confirmation method.</summary>
    /// <remarks>For this confirmation method, proof of the relationship between the attesting entity and the subject of the statements in the assertion is implicit and no steps need be taken by the receiver to establish this relationship.</remarks>
    public class BearerSamlTokenContract : SamlTokenContract
    {
        public override SubjectConfirmationMethod Confirmation
        {
            get { return SubjectConfirmationMethod.Bearer; }
        }
    }

    /// <summary>A contract for creating a SAML 2 token with the <see cref="SubjectConfirmationMethod.HolderOfKey">Holder-of-Key</see> confirmation method. The token will also include the symmetric security key for computing the digitial signature of the associated message.</summary>
    public class SymmetricSamlTokenContract : SamlTokenContract
    {
        public string SymmetricKeyValue;

        public override SubjectConfirmationMethod Confirmation
        {
            get { return SubjectConfirmationMethod.HolderOfKey; }
        }
    }

    /// <summary>A contract for creating a SAML 2 token with the <see cref="SubjectConfirmationMethod.HolderOfKey">Holder-of-Key</see> confirmation method. The token will also include the public key of the asymmetric key pair used for computing the digitial signature of the associated message.</summary>
    public class AsymmetricSamlTokenContract : SamlTokenContract
    {
        public override SubjectConfirmationMethod Confirmation
        {
            get { return SubjectConfirmationMethod.HolderOfKey; }
        }
    }

    /// <summary>A contract for creating a SAML 2 token with the Sender-Vouches subject confirmation method.</summary>
    /// <remarks>
    ///     <para>An attesting entity uses the sender-vouches confirmation method to assert that it is acting on behalf of the subject of SAML statements attributed with a sender-vouches SubjectConfirmation element. Statements attested for by the sender-vouches method MUST be associated, within their containing assertion, with one or more sender-vouches SubjectConfirmation elements.</para>
    ///     <para>Of the SAML assertions it selects for processing, a message receiver MUST NOT accept statements of these assertions based on a sender-vouches SubjectConfirmation element defined for the statements (within the assertion) unless the assertions and SOAP message content being vouched for are protected by an attesting entity who is trusted by the receiver to act as the subjects and with the claims of the statements.</para>
    /// </remarks>
    public class SenderVouchesSamlTokenContract : SamlTokenContract
    {
        public override SubjectConfirmationMethod Confirmation
        {
            get { return SubjectConfirmationMethod.SenderVouches; }
        }
    }
}