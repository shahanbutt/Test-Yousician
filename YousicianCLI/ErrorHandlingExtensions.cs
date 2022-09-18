using System;
using System.Reactive.Linq;

namespace YousicianCLI
{
    public static class ErrorHandlingExtensions
    {
        /// <summary>
        /// Retries observable indefinitely and prints an error on each failure.
        /// If the message factory returns null, a default message will be used.
        /// </summary>
        public static IObservable<T> RetryWithErrorMessages<T>(
            this IObservable<T> observable,
            IUserInterface userInterface,
            Func<Exception, string> messageFactory)
            => observable
                .Do(
                    _ => { },
                    e => userInterface.ShowMessage(messageFactory(e) ?? Messages.UnexpectedError))
                .Retry();
    }
}