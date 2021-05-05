using System;
using System.Reactive.Linq;
using Audyssey.ViewModels;
using ReactiveUI;
using Splat;

#nullable enable

namespace Audyssey.Initialization
{
    public static class ObservableExtensions
    {
        public static void DefaultCatch(
            this IObservable<Exception> observable, 
            ViewModelBase viewModel)
        {
            observable = observable ?? throw new ArgumentNullException(nameof(observable));
            viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

            observable.Subscribe(async exception =>
            {
                viewModel.Log().Warn(exception);

                await Interactions.Exception.Handle(exception);
            });
        }

        public static void DefaultCatch(
            this IObservable<Exception> observable)
        {
            observable = observable ?? throw new ArgumentNullException(nameof(observable));

            observable.Subscribe(async exception =>
            {
                await Interactions.Exception.Handle(exception);
            });
        }

        public static ReactiveCommand<T1, T2> WithDefaultCatch<T1, T2>(
            this ReactiveCommand<T1, T2> command,
            ViewModelBase viewModel)
        {
            command = command ?? throw new ArgumentNullException(nameof(command));
            viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

            command.ThrownExceptions.DefaultCatch(viewModel);

            return command;
        }

        public static ReactiveCommand<T1, T2> WithDefaultCatch<T1, T2>(
            this ReactiveCommand<T1, T2> command)
        {
            command = command ?? throw new ArgumentNullException(nameof(command));

            command.ThrownExceptions.DefaultCatch();

            return command;
        }
    }
}
