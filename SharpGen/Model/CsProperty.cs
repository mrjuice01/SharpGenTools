﻿// Copyright (c) 2010-2014 SharpDX - Alexandre Mutel
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

using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SharpGen.CppModel;

namespace SharpGen.Model;

public sealed class CsProperty : CsMarshalBase
{
    private static readonly Regex MatchGet = new(@"^\s*(\<[Pp]\>)?\s*(Gets?|Retrieves?|Returns)", RegexOptions.Compiled);

    public CsProperty(Ioc ioc, CppMethod cppCallable, string name, CsMethod getter, CsMethod setter,
                      bool isPropertyParam = false) : base(ioc, cppCallable, name)
    {
        Getter = getter;
        Setter = setter;
        IsPropertyParam = isPropertyParam;
        IsPersistent = getter?.IsPersistent ?? IsPersistent;
    }

    public CsMethod Getter { get; }

    public CsMethod Setter { get; }

    public bool IsPropertyParam { get; }

    public bool IsPersistent { get; }

    public override string Description
    {
        get
        {
            var description = base.Description;

            // If we have a both getter and a setter, then we need to modify the documentation
            // in order to print that we have both.
            if (Getter != null && Setter != null && !string.IsNullOrEmpty(description))
                description = MatchGet.Replace(description, "$1$2 or sets");

            return description;
        }
        set => base.Description = value;
    }

    public bool IsDisposeBlockNeeded => IsPersistent && Getter != null && !IsPrimitive;

    public SyntaxToken PersistentFieldIdentifier => SyntaxFactory.Identifier($"{Name}__");

    public override string DocUnmanagedName =>
        FormatDocUnmanagedName(Getter?.DocUnmanagedName, Setter?.DocUnmanagedName);

    public override string DocUnmanagedShortName =>
        FormatDocUnmanagedName(Getter?.DocUnmanagedShortName, Setter?.DocUnmanagedShortName);

    private static string FormatDocUnmanagedName(string getter, string setter)
    {
        if (!string.IsNullOrEmpty(getter) && !string.IsNullOrEmpty(setter))
            return $"{getter} / {setter}";
        if (!string.IsNullOrEmpty(getter))
            return getter;
        if (!string.IsNullOrEmpty(setter))
            return setter;
        return "Unknown";
    }
}