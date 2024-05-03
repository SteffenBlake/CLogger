namespace CLogger.Common.Model;

public record RunTestsArgs(bool Discover, bool Debug, IList<string> TestIds);
