using System;
using System.Reactive;
using ReactiveUI;

namespace Audyssey
{
    public static class Interactions
    {
        public static Interaction<string, Unit> Warning { get; } = new();
        public static Interaction<Exception, Unit> Exception { get; } = new();
    }
}
