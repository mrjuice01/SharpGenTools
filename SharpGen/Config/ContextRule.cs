// Copyright (c) 2010-2014 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace SharpGen.Config;

[XmlType("context")]
public class ContextRule : ExtensionBaseRule
{
    public ContextRule()
    {
        Ids = new List<string>();
    }

    public ContextRule(string context)
    {
        Ids = new List<string> {context};
    }

    public ContextRule(IEnumerable<string> ids)
    {
        Ids = new List<string>(ids);
    }

    [XmlAttribute("id")]
    public string ContextSetId { get; set; }

    [XmlText]
    public List<string> Ids { get; set; }

    [ExcludeFromCodeCoverage]
    public override string ToString()
    {
        return string.Format(CultureInfo.InvariantCulture, "{0} Ids:{1}", base.ToString(), (Ids != null) ? Ids.ToString() : "");
    }
}

[XmlType("context-clear")]
public class ClearContextRule : ContextRule
{
    public ClearContextRule()
    {
        Ids = null;
    }
}