using System;
using System.Reactive.Linq;

namespace YousicianCLI
{
    public static class ProgressIndicatorExtensions
    {
        public static IObservable<T> ShowProgressIndicator<T>(
            this IObservable<T> operation,
            string description,
            IUserInterface userInterface)
            => Observable.Defer(() =>
            {
                var indicator = userInterface.ShowProgressIndicator(description);
                return operation
                    .Do(_ => indicator.Dispose())
                    .Finally(indicator.Dispose);
            });
    }
}