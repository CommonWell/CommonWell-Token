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

using System.Text;
using System.Xml;

namespace CommonWell.Token
{
    public class RoleClaim : NestedClaim
    {
        public RoleClaim()
            : base()
        {
        }

        public RoleClaim(string name, string code)
            : base(name, code)
        {
            CodeSystem = "2.16.840.1.113883.6.96";
            CodeSystemName = "SNOMED_CT";
        }

        public override string ToString()
        {
            var settings = new XmlWriterSettings
            {
                Indent = false,
                Encoding = new UTF8Encoding(false),
                NamespaceHandling = NamespaceHandling.OmitDuplicates,
                OmitXmlDeclaration = true,
                NewLineOnAttributes = false,
                DoNotEscapeUriAttributes = true
            };
            var sb = new StringBuilder();
            using (var xw = XmlWriter.Create(sb, settings))
            {
                xw.WriteStartElement("Role", Xmlnamespace);
                xw.WriteAttributeString("xmlns", "xsi", null, Xsinamespace);
                xw.WriteAttributeString("xsi", "type", Xsinamespace, Type);
                xw.WriteAttributeString("code", Code);
                xw.WriteAttributeString("codeSystem", CodeSystem);
                xw.WriteAttributeString("codeSystemName", CodeSystemName);
                xw.WriteAttributeString("displayName", DisplayName);
                xw.WriteEndElement();
                xw.Flush();
                return sb.ToString();
            }
        }
    }
}