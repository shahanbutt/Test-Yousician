using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace YousicianCLI
{
    public class ConsoleInterface : IUserInterface
    {
        private static readonly string[] ProgressIndicatorComponents = { "/", "-", "\\", "|" };

        // Prevent funky lines when accessing Console from multiple threads.
        private static readonly object SyncObject = new object();

        public void ShowMessage(string message)
        {
            lock (SyncObject)
            {
                Console.WriteLine(message);
            }
        }

        public IDisposable ShowProgressIndicator(string operationDescription)
        {
            ShowMessage($"{operationDescription}... ");

            return new CompositeDisposable(
                Observable
                    .Timer(TimeSpan.Zero, TimeSpan.FromMilliseconds(100))
                    .Subscribe(i =>
                    {
                        lock (SyncObject)
                        {
                            Console.Write(ProgressIndicatorComponents[i % ProgressIndicatorComponents.Length]);
                            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                        }
                    }),
                Disposable.Create(() =>
                {
                    lock (SyncObject)
                    {
                        Console.WriteLine();
                    }
                }));
        }

        public IObservable<string> RequestInput(string prompt) => Observable.Defer(() =>
        {
            lock (SyncObject)
            {
                Console.WriteLine($"{prompt}: ");
                return Observable.Return(Console.ReadLine());
            }
        });

        public IObservable<T> RequestChoice<T>(string prompt)
            where T : struct, Enum
        {
            var options = Enum
                .GetValues(typeof(T))
                .Cast<T>()
                .Select(val => val.ToString());

            return RequestInput($"{prompt} [{string.Join("|", options)}]")
                .Select(input => Enum.Parse<T>(input, false));
        }
    }
}