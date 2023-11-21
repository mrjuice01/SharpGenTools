#nullable enable

using System.Collections.Generic;
using SharpGen.Config;

namespace SharpGen.Parser;

public interface IIncludeDirectoryResolver
{
    void Configure(ConfigFile config);
    void AddDirectories(IEnumerable<IncludeDirRule> directories);
    void AddDirectories(params IncludeDirRule[] directories);
    IEnumerable<string> IncludeArguments { get; }
}