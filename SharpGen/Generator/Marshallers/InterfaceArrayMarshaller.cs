﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpGen.Model;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpGen.Generator.Marshallers;

internal sealed class InterfaceArrayMarshaller : MarshallerBase, IMarshaller
{
    public bool CanMarshal(CsMarshalBase csElement) => csElement.IsInterfaceArray;

    public ArgumentSyntax GenerateManagedArgument(CsParameter csElement) =>
        Argument(IdentifierName(csElement.Name));

    public ParameterSyntax GenerateManagedParameter(CsParameter csElement) => Parameter(Identifier(csElement.Name))
       .WithType(ParseTypeName(csElement.PublicType.QualifiedName));

    public StatementSyntax GenerateManagedToNative(CsMarshalBase csElement, bool singleStackFrame) => null;

    public IEnumerable<StatementSyntax> GenerateManagedToNativeProlog(CsMarshalCallableBase csElement) =>
        Enumerable.Empty<StatementSyntax>();

    public ArgumentSyntax GenerateNativeArgument(CsMarshalCallableBase csElement) => Argument(
        MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName(csElement.Name),
            IdentifierName("NativePointer")
        )
    );

    public StatementSyntax GenerateNativeCleanup(CsMarshalBase csElement, bool singleStackFrame) => null;

    public StatementSyntax GenerateNativeToManaged(CsMarshalBase csElement, bool singleStackFrame) => null;

    public IEnumerable<StatementSyntax> GenerateNativeToManagedExtendedProlog(CsMarshalCallableBase csElement)
    {
        yield return GenerateArrayNativeToManagedExtendedProlog(csElement);
    }

    public FixedStatementSyntax GeneratePin(CsParameter csElement) => null;

    public bool GeneratesMarshalVariable(CsMarshalCallableBase csElement) => true;

    public TypeSyntax GetMarshalTypeSyntax(CsMarshalBase csElement) => IntPtrType;

    public InterfaceArrayMarshaller(Ioc ioc) : base(ioc)
    {
    }
}