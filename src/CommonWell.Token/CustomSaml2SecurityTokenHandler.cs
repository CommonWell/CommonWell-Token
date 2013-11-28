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
using System.Text;
using System.Xml;

namespace CommonWell.Token
{
    /// <summary>Custom SAML2 Security Token Handler supporting complex attributes.</summary>
    public class CustomSaml2SecurityTokenHandler : Saml2SecurityTokenHandler
    {
        //public CustomSaml2SecurityTokenHandler(SecurityTokenHandlerConfiguration configuration)
        //{
        //    SecurityTokenHandlerConfiguration handlerConfig = configuration ?? new SecurityTokenHandlerConfiguration
        //    {
        //        AudienceRestriction = {AudienceMode = AudienceUriMode.Never},
        //        CertificateValidationMode = X509CertificateValidationMode.None,
        //        RevocationMode = X509RevocationMode.NoCheck,
        //        MaxClockSkew = new TimeSpan(50000000),
        //        CertificateValidator = X509CertificateValidator.None
        //    };
        //    Configuration = handlerConfig;
        //}

        //public CustomSaml2SecurityTokenHandler()
        //    : this(null)
        //{
        //}

        protected override void WriteAttributeValue(XmlWriter writer, string value, Saml2Attribute attribute)
        {
            var sb = new StringBuilder("<a>");
            sb.Append(value);
            sb.Append("</a>");
            byte[] rawValue = new UTF8Encoding().GetBytes(sb.ToString());
            using (
                XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(rawValue,
                    XmlDictionaryReaderQuotas.Max))
            {
                reader.ReadStartElement("a");
                while (reader.NodeType != XmlNodeType.EndElement ||
                       (reader.NodeType == XmlNodeType.EndElement && reader.Name != "a"))
                {
                    writer.WriteNode(reader, false);
                }
                reader.ReadEndElement();
                reader.Close();
            }
        }
    }
}