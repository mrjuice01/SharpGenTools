﻿using System.Linq;
using SharpGen.Config;
using SharpGen.CppModel;
using SharpGen.Model;
using SharpGen.Transform;
using Xunit;
using Xunit.Abstractions;

namespace SharpGen.UnitTests.Mapping;

public class Function : MappingTestBase
{
    public Function(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    [Fact]
    public void Basic()
    {
        var config = new ConfigFile
        {
            Id = nameof(Basic),
            Namespace = nameof(Basic),
            Includes =
            {
                new IncludeRule
                {
                    Attach = true,
                    File = "func.h",
                    Namespace = nameof(Basic)
                }
            },
            Extension =
            {
                new CreateExtensionRule
                {
                    NewClass = $"{nameof(Basic)}.Functions",
                }
            },
            Bindings =
            {
                new BindRule("int", "System.Int32")
            },
            Mappings =
            {
                new MappingRule
                {
                    Function = "Test",
                    FunctionDllName = "\"Test.dll\"",
                    Group = $"{nameof(Basic)}.Functions"
                }
            }
        };

        var function = new CppFunction("Test")
        {
            ReturnValue = new CppReturnValue
            {
                TypeName = "int",
            }
        };

        var include = new CppInclude("func");

        var module = new CppModule("SharpGenTestModule");

        include.Add(function);
        module.Add(include);

        var (solution, _) = MapModel(module, config);

        Assert.Single(solution.EnumerateDescendants<CsGroup>());

        var group = solution.EnumerateDescendants<CsGroup>().First();
        Assert.Equal("Functions", group.Name);

        Assert.Single(group.Functions);

        var csFunc = group.Functions.First();
        Assert.Equal(TypeRegistry.Int32, csFunc.ReturnValue.PublicType);
        Assert.Empty(csFunc.Parameters);
        Assert.Equal(Visibility.Static, csFunc.Visibility & Visibility.Static);
    }

    [Fact]
    public void PointerSizeReturnValueNotLarge()
    {
        var config = new ConfigFile
        {
            Id = nameof(PointerSizeReturnValueNotLarge),
            Namespace = nameof(PointerSizeReturnValueNotLarge),
            Includes =
            {
                new IncludeRule
                {
                    File = "pointerSize.h",
                    Attach = true,
                    Namespace = nameof(PointerSizeReturnValueNotLarge)
                }
            },
            Extension =
            {
                new DefineExtensionRule
                {
                    Struct = "SharpGen.Runtime.PointerSize",
                    SizeOf = 8,
                    IsNativePrimitive = true,
                }
            },
            Bindings =
            {
                new BindRule("int", "SharpGen.Runtime.PointerSize")
            }
        };

        var iface = new CppInterface("Interface");

        iface.Add(new CppMethod("method")
        {
            ReturnValue = new CppReturnValue
            {
                TypeName = "int"
            }
        });

        var include = new CppInclude("pointerSize");

        include.Add(iface);

        var module = new CppModule("SharpGenTestModule");
        module.Add(include);

        var (solution, _) = MapModel(module, config);

        Assert.Single(solution.EnumerateDescendants<CsInterface>());

        var csIface = solution.EnumerateDescendants<CsInterface>().First();

        Assert.Single(csIface.Methods);

        var method = csIface.Methods.First();

        Assert.False(method.IsReturnStructLarge);
    }

    [Fact]
    public void NativePrimitiveTypeNotLarge()
    {
        var config = new ConfigFile
        {
            Id = nameof(NativePrimitiveTypeNotLarge),
            Namespace = nameof(NativePrimitiveTypeNotLarge),
            Includes =
            {
                new IncludeRule
                {
                    File = "pointerSize.h",
                    Attach = true,
                    Namespace = nameof(NativePrimitiveTypeNotLarge)
                }
            },
            Extension =
            {
                new DefineExtensionRule
                {
                    Struct = "NativePrimitiveType",
                    SizeOf = 16,
                    IsNativePrimitive = true,
                }
            },
            Bindings =
            {
                new BindRule("NativePrimitive", "NativePrimitiveType")
            }
        };

        var iface = new CppInterface("Interface");

        iface.Add(new CppMethod("method")
        {
            ReturnValue = new CppReturnValue
            {
                TypeName = "NativePrimitive"
            }
        });

        var include = new CppInclude("pointerSize");

        include.Add(iface);

        var module = new CppModule("SharpGenTestModule");
        module.Add(include);

        var (solution, _) = MapModel(module, config);

        Assert.Single(solution.EnumerateDescendants<CsInterface>());

        var csIface = solution.EnumerateDescendants<CsInterface>().First();

        Assert.Single(csIface.Methods);

        var method = csIface.Methods.First();

        Assert.False(method.IsReturnStructLarge);

        Assert.False(Logger.HasErrors);
    }
}