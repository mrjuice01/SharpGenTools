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

using System;
using System.Collections.Generic;
using System.Linq;
using SharpGen.CppModel;

namespace SharpGen.Model;

public sealed class CsEnum : CsTypeBase
{
    public CsFundamentalType UnderlyingType { get; }

    public override uint Size => UnderlyingType.Size;
    protected override uint? AlignmentCore => UnderlyingType.Alignment;
    public override bool IsBlittable => UnderlyingType.IsBlittable;

    public bool IsFlag { get; }

    public IEnumerable<CsEnumItem> EnumItems => Items.OfType<CsEnumItem>();

    public CsEnum(CppEnum cppEnum, string name, CsFundamentalType underlyingType) : base(cppEnum, name)
    {
        UnderlyingType = underlyingType ?? throw new ArgumentNullException(nameof(underlyingType));

        if (cppEnum == null)
            return;

        // If C++ enum name is ending with FLAG OR FLAGS, then tag this enum as flags
        var cppEnumName = cppEnum.Name;
        if (cppEnumName.EndsWith("FLAG") || cppEnumName.EndsWith("FLAGS"))
            IsFlag = true;

        var tag = cppEnum.Rule;

        IsFlag = tag.EnumHasFlags ?? IsFlag;
    }
}