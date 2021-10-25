using Ratbuddyssey.DTOs;

namespace Ratbuddyssey;

public static class Interactions
{
    public static Interaction<string, Unit> Warning { get; } = new();
    public static Interaction<Exception, Unit> Exception { get; } = new();
    public static Interaction<string, bool> Question { get; } = new();

    public static Interaction<OpenFileArguments, FileData?> OpenFile { get; } = new();
    public static Interaction<SaveFileArguments, string?> SaveFile { get; } = new();
}
