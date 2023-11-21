﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpGen.Model;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpGen.Generator;

public sealed class RoslynGenerator
{
    private static readonly SyntaxTokenList ModuleInitModifiers = TokenList(Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.StaticKeyword));
    private const string AutoGeneratedCommentText = "// <auto-generated/>\n";
    private static readonly AttributeListSyntax ModuleInitializerAttributeList = AttributeList(
        SingletonSeparatedList(Attribute(ParseName("System.Runtime.CompilerServices.ModuleInitializerAttribute")))
    );

    public SyntaxTree Run(CsAssembly csAssembly, Ioc ioc)
    {
        var logger = ioc.Logger;
        var generators = ioc.Generators;

        logger.Message("Generating Roslyn syntax tree...");

        var resultConstants = csAssembly.Namespaces
                                        .SelectMany(x => x.EnumerateDescendants<CsResultConstant>(withAdditionalItems: false))
                                        .ToArray();

        var moduleInitializer = resultConstants.Length > 0
                                    ? new[]
                                    {
                                        ClassDeclaration("ModuleDataInitializer")
                                           .WithModifiers(ModuleInitModifiers)
                                           .AddMembers(GenerateResultDescriptor(resultConstants, ioc))
                                    }
                                    : Enumerable.Empty<MemberDeclarationSyntax>();

        MemberDeclarationSyntax NamespaceSelector(CsNamespace ns)
        {
            MemberSyntaxList list = new(ioc);
            list.AddRange(ns.Enums.OrderBy(element => element.Name), generators.Enum);
            list.AddRange(ns.Structs.OrderBy(element => element.Name), generators.Struct);
            list.AddRange(ns.Classes.OrderBy(element => element.Name), generators.Group);
            list.AddRange(ns.Interfaces.OrderBy(element => element.Name), generators.Interface);
            return NamespaceDeclaration(ParseName(ns.Name), default, default, List(list))
               .WithLeadingTrivia(Comment(AutoGeneratedCommentText));
        }

        return CSharpSyntaxTree.Create(
            RoslynSyntaxNormalizer.Normalize(
                CompilationUnit(
                    default, default, default,
                    List(csAssembly.Namespaces.Select(NamespaceSelector)).AddRange(moduleInitializer)
                ),
                "    ",
                "\r\n",
                true
            )
        );
    }

    private MethodDeclarationSyntax GenerateResultDescriptor(CsResultConstant[] descriptors, Ioc ioc)
    {
        StatementSyntaxList list = new(ioc);
        list.AddRange(descriptors, ioc.Generators.ResultRegistration);
        return MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), "RegisterResultCodes")
              .WithModifiers(ModuleInitModifiers)
              .AddAttributeLists(ModuleInitializerAttributeList)
              .WithBody(list.ToBlock());
    }
}