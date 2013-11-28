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
using System.IO;
using System.Xml;

namespace CommonWell.Token
{
    public abstract class NestedClaim
    {
        public const string Xmlnamespace = "urn:hl7-org:v3";
        public const string Xsinamespace = "http://www.w3.org/2001/XMLSchema-instance";

        public string Code;
        public string CodeSystem;
        public string CodeSystemName;
        public string DisplayName;
        public string Type;

        protected NestedClaim()
        {
        }

        protected NestedClaim(string name, string code)
        {
            DisplayName = name;
            Code = code;
            Type = "CE";
        }

        public virtual void Load(string xmlString)
        {
            if (String.IsNullOrEmpty(xmlString)) return;

            using (XmlReader reader = XmlReader.Create(new StringReader(xmlString)))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element & reader.HasAttributes)
                    {
                        Code = reader.GetAttribute("code");
                        CodeSystem = reader.GetAttribute("codeSystem");
                        CodeSystemName = reader.GetAttribute("codeSystemName");
                        DisplayName = reader.GetAttribute("displayName");
                        Type = reader.GetAttribute("xsi:type");
                    }
                }
            }
        }
    }
}