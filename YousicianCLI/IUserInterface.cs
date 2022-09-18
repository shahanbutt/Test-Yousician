using System;

namespace YousicianCLI
{
    public interface IUserInterface
    {
        /// <summary>
        /// Shows a message to the user.
        /// </summary>
        void ShowMessage(string message);

        /// <summary>
        /// Show a progress indicator for an operation, until the returned disposable is disposed.
        /// </summary>
        IDisposable ShowProgressIndicator(string operationDescription);

        /// <summary>
        /// Asynchronously request input from the user.
        /// </summary>
        IObservable<string> RequestInput(string prompt);

        /// <summary>
        /// Asynchronously request the user to type in the name of an enum member, and return it.
        /// If the name does not match an enum name, an error will be published.
        /// </summary>
        IObservable<T> RequestChoice<T>(string prompt) where T : struct, Enum;
    }
}