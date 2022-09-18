using System;
using YousicianCLI.Backend;

namespace YousicianCLI
{
    static class Program
    {
        static void Main(string[] args)
        {
            var completion = new ProgramCompletion();
            new SignupFlow(new UnreliableBackend(), new ConsoleInterface())
                .Run()
                .Subscribe(completion.CompleteWithSuccess, completion.CompleteWithError);
            completion.WaitForCompleted();
        }
    }
}
