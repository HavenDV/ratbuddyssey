using System;
using System.Reactive;
using ReactiveUI;

#nullable enable

namespace Audyssey
{
    public static class Interactions
    {
        public static Interaction<string, Unit> Warning { get; } = new();
        public static Interaction<Exception, Unit> Exception { get; } = new();
        public static Interaction<string, string?> OpenFile { get; } = new();
        public static Interaction<string, string?> SaveFile { get; } = new();
    }
}
