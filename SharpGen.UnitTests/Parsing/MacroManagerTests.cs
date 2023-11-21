﻿using System;
using System.IO;
using SharpGen.Config;
using SharpGen.CppModel;
using SharpGen.Parser;
using Xunit;
using Xunit.Abstractions;

namespace SharpGen.UnitTests.Parsing;

public class MacroManagerTests : ParsingTestBase
{
    public MacroManagerTests(ITestOutputHelper outputHelper)
        : base(outputHelper)
    {
    }

    [Fact]
    public void MacroManagerCollectsIncludedHeaders()
    {
        CreateCppFile("included", "");

        var includeRule = GetTestFileIncludeRule();

        var config = new ConfigFile
        {
            Id = nameof(MacroManagerCollectsIncludedHeaders),
            Namespace = nameof(MacroManagerCollectsIncludedHeaders),
            IncludeDirs = { includeRule },
            Includes =
            {
                CreateCppFile("includer", "#include \"included.h\"")
            }
        };

        config.Load(null, Array.Empty<string>(), Logger);

        var castXml = GetCastXml(config);

        var macroManager = new MacroManager(castXml);

        macroManager.Parse(Path.Combine(includeRule.Path, "includer.h"), new CppModule("SharpGenTestModule"));

        Assert.Contains(includeRule.Path + "/included.h", macroManager.IncludedFiles);
    }
}