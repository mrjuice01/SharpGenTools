﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SharpGen.CppModel;
using SharpGen.Logging;
using SharpGen.Model;

namespace SharpGen.Transform;

public sealed class NamespaceRegistry
{
    private readonly Dictionary<string, CsNamespace> _mapIncludeToNamespace = new(StringComparer.InvariantCultureIgnoreCase);
    private readonly Dictionary<Regex, CsNamespace> _mapTypeToNamespace = new();
    private readonly Dictionary<string, CsNamespace> _namespaces = new();
    private readonly Ioc ioc;

    public IEnumerable<CsNamespace> Namespaces => _namespaces.Values;
    private Logger Logger => ioc.Logger;

    public NamespaceRegistry(Ioc ioc)
    {
        this.ioc = ioc ?? throw new ArgumentNullException(nameof(ioc));
    }

    /// <summary>
    /// Gets the C# namespace by its name.
    /// </summary>
    public CsNamespace GetOrCreateNamespace(string namespaceName)
    {
        if (_namespaces.TryGetValue(namespaceName, out var selectedNamespace))
            return selectedNamespace;

        selectedNamespace = new CsNamespace(namespaceName);
        _namespaces.Add(namespaceName, selectedNamespace);
        return selectedNamespace;
    }

    /// <summary>
    /// Maps a particular C++ include to a C# namespace.
    /// </summary>
    /// <param name="includeName">Name of the include.</param>
    /// <param name="nameSpace">The namespace.</param>
    public void MapIncludeToNamespace(string includeName, string nameSpace)
    {
        _mapIncludeToNamespace.Add(includeName, GetOrCreateNamespace(nameSpace));
    }

    /// <summary>
    /// Attaches C++ to a C# namespace using a regular expression query.
    /// </summary>
    /// <param name="typeNameRegex">The C++ regex selection.</param>
    /// <param name="namespaceName">The namespace.</param>
    public void AttachTypeToNamespace(string typeNameRegex, string namespaceName)
    {
        _mapTypeToNamespace.Add(new Regex(typeNameRegex), GetOrCreateNamespace(namespaceName));
    }

    private bool TryGetNamespaceForInclude(string includeName, out CsNamespace cSharpNamespace)
    {
        return _mapIncludeToNamespace.TryGetValue(includeName, out cSharpNamespace);
    }

    private (bool match, CsNamespace nameSpace) GetCsNamespaceForCppElement(CppElement element)
    {
        foreach (var regExp in _mapTypeToNamespace)
        {
            if (regExp.Key.Match(element.Name).Success)
                return (true, regExp.Value);
        }
        return (false, default);
    }


    /// <summary>
    /// Resolves the namespace for a C++ element.
    /// </summary>
    /// <param name="element">The C++ element.</param>
    /// <returns>The attached namespace for this C++ element.</returns>
    internal CsNamespace ResolveNamespace(CppElement element)
    {
        var tag = element.Rule;

        // If a type is redispatched to another namespace
        if (!string.IsNullOrEmpty(tag.Namespace))
            return GetOrCreateNamespace(tag.Namespace);

        var (match, nameSpace) = GetCsNamespaceForCppElement(element);
        if (match)
            return nameSpace;

        var parentInclude = element.ParentInclude;
        if (parentInclude == null)
        {
            Logger.Fatal("Unable to find parent include for element [{0}]", element);
            return null;
        }

        if (!TryGetNamespaceForInclude(parentInclude.Name, out var ns))
        {
            Logger.Fatal("Unable to find namespace for element [{0}] from include [{1}]", element, parentInclude.Name);
            return null;
        }

        return ns;
    }
}