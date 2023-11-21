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

#nullable enable

using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace SharpGen.Config;

/// <summary>
/// Binding rule. Bind a cpp type to a C#/.Net type.
/// Usage: bind from="CPP_TYPE_NAME"  to="C#_FULLTYPE_NAME"
/// </summary>
[XmlType("bind")]
public class BindRule : ConfigBaseRule
{
    public BindRule()
    {
    }

    /// <param name="from">The C++ type</param>
    /// <param name="to">The C# public type</param>
    /// <param name="marshal">The C# marshal type</param>
    /// <param name="source">The source of the definition</param>
    public BindRule(string from, string to, string? marshal = null, string? source = null)
    {
        From = from;
        To = to;
        Marshal = marshal;
        Source = source;
    }

    /// <summary>
    /// Gets or sets the cpp from type.
    /// </summary>
    /// <value>From cpp type</value>
    [XmlAttribute("from")]
    public string From { get; set; }

    /// <summary>
    /// Gets or sets the C# to type
    /// </summary>
    /// <value>To.</value>
    [XmlAttribute("to")]
    public string To { get; set; }

    /// <summary>
    /// Gets or sets the C# the marshal type
    /// </summary>
    [XmlAttribute("marshal")]
    public string? Marshal { get; set; }

    /// <value>The source of the definition</value>
    [XmlAttribute("source")]
    [DefaultValue(null)]
    public string? Source { get; set; }

    /// <summary>
    ///     Provides an ability to override an existing mapping
    /// </summary>
    [XmlIgnore]
    public bool? Override { get; set; }

    [XmlAttribute("override")]
    public bool _Override_
    {
        get => Override.Value;
        set => Override = value;
    }

    public bool ShouldSerialize_Override_() => Override != null;

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => string.Format(
        CultureInfo.InvariantCulture,
        "{0} from:{1} to:{2} marshal:{3} source:{4} override:{5}", base.ToString(), From, To, Marshal, Source, Override
    );
}