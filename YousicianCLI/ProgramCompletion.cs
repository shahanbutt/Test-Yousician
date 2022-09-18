using System;
using System.Threading;
using YousicianCLI.Models;

namespace YousicianCLI
{
    public class ProgramCompletion
    {
        private readonly AutoResetEvent _resetEvent = new AutoResetEvent(false);

        public void WaitForCompleted() => _resetEvent.WaitOne();

        public void CompleteWithSuccess(UserData userData)
        {
            Console.WriteLine(
                $"Created user '{userData.Credentials.Username}' on {userData.Instrument} " +
                $"and a password {userData.Credentials.Password.Length} characters long.");
            _resetEvent.Set();
        }

        public void CompleteWithError(Exception e)
        {
            Console.WriteLine($"Failed with error: {e}");
            _resetEvent.Set();
        }
    }
}