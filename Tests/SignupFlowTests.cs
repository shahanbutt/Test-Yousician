using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using NSubstitute;
using NUnit.Framework;
using YousicianCLI;
using YousicianCLI.Backend;
using YousicianCLI.Models;

namespace Tests
{
    public class SignupFlowTests : CLITest
    {
        private static readonly Credentials TestCredentials =
            new Credentials("TestUser", "TestPassword");

        [Test]
        public void HappyFlow_ProducesExpectedOutput()
        {
            const Instrument instrument = Instrument.Ukulele;

            // Backend returns success
            Backend
                .RegisterUser(TestCredentials, Arg.Any<Instrument?>())
                .Returns(Observable.Return(Unit.Default));

            Backend
                .SetUserInstrument(TestCredentials, instrument)
                .Returns(Observable.Return(Unit.Default));

            SetUpCredentialsInputOnce(TestCredentials);
            SetUpInstrumentInput(instrument);

            var result = SignupFlow.Run().ExpectSingleValueSynchronously();
            Assert.AreEqual(new UserData(TestCredentials, instrument), result);
        }

        [TestCase(typeof(Exception), Messages.UnexpectedError)]
        [TestCase(typeof(UsernameAlreadyInUseException), Messages.UsernameInUse)]
        public void Signup_ProducesCorrectMessage_WhenErrorReturned(Type errorType, string expectedMessage)
        {
            var exception = (Exception) Activator.CreateInstance(errorType, "Test");

            Backend
                .RegisterUser(default, default)
                .ReturnsForAnyArgs(Observable.Throw<Unit>(exception));

            SetUpCredentialsInputOnce(TestCredentials);

            SignupFlow.Run().Subscribe().Dispose();

            UserInterface.Received(1).ShowMessage(expectedMessage);
        }

        [Test]
        public void Signup_ShowsProgressIndicator_WhileWaitingForBackend()
        {
            var completion = new Subject<Unit>();
            var progressIndicatorHandle = new BooleanDisposable();

            Backend
                .RegisterUser(default, default)
                .ReturnsForAnyArgs(completion);

            UserInterface
                .ShowProgressIndicator(Messages.SigningUp)
                .Returns(progressIndicatorHandle);

            SetUpCredentialsInputOnce(TestCredentials);

            using (SignupFlow.Run().Subscribe())
            {
                UserInterface.Received(1).ShowProgressIndicator(Messages.SigningUp);
                Assert.IsFalse(progressIndicatorHandle.IsDisposed);

                completion.OnNext(Unit.Default);
                completion.OnCompleted();
                Assert.IsTrue(progressIndicatorHandle.IsDisposed);
            }
        }

        [Test]
        public void InstrumentSelection_ShowsProgressIndicator_WhileWaitingForBackend()
        {
            var completion = new Subject<Unit>();
            var progressIndicatorHandle = new BooleanDisposable();

            Backend
                .RegisterUser(default, default)
                .ReturnsForAnyArgs(Observable.Return(Unit.Default));

            Backend
                .SetUserInstrument(default, default)
                .ReturnsForAnyArgs(completion);

            UserInterface
                .ShowProgressIndicator(Messages.SelectingInstrument)
                .Returns(progressIndicatorHandle);

            SetUpCredentialsInputOnce(TestCredentials);
            SetUpInstrumentInput(default);

            using (SignupFlow.Run().Subscribe())
            {
                UserInterface.Received(1).ShowProgressIndicator(Messages.SelectingInstrument);
                Assert.IsFalse(progressIndicatorHandle.IsDisposed);

                completion.OnNext(Unit.Default);
                completion.OnCompleted();
                Assert.IsTrue(progressIndicatorHandle.IsDisposed);
            }
        }

        [Test]
        public void Signup_Retries_WhenErrorEncountered()
        {
            Backend
                .RegisterUser(default, default)
                .ReturnsForAnyArgs(
                    // First error, then success
                    Observable.Throw<Unit>(new Exception()),
                    Observable.Return(Unit.Default));

            Backend
                .SetUserInstrument(default, default)
                .ReturnsForAnyArgs(Observable.Return(Unit.Default));

            var callCounts = SetUpCredentialsInputRepeated(TestCredentials);
            SetUpInstrumentInput(Instrument.Ukulele);

            Assert.AreEqual(
                new UserData(TestCredentials, Instrument.Ukulele),
                SignupFlow.Run().ExpectSingleValueSynchronously());

            Assert.AreEqual(2, callCounts.usernamePrompts.Value, "Should prompt for username twice");
            Assert.AreEqual(2, callCounts.passwordPrompts.Value, "Should prompt for password twice");
        }

        [Test]
        public void InstrumentSelection_Retries_WhenErrorEncountered()
        {
            Backend
                .SetUserInstrument(default, default)
                .ReturnsForAnyArgs(
                    // First error, then success
                    Observable.Throw<Unit>(new Exception()),
                    Observable.Return(Unit.Default));

            Backend
                .RegisterUser(default, default)
                .ReturnsForAnyArgs(Observable.Return(Unit.Default));

            SetUpCredentialsInputOnce(TestCredentials);
            SetUpInstrumentInput(Instrument.Ukulele);

            Assert.AreEqual(
                new UserData(TestCredentials, Instrument.Ukulele),
                SignupFlow.Run().ExpectSingleValueSynchronously());
        }

        private void SetUpInstrumentInput(Instrument instrument)
        {
            UserInterface
                .RequestChoice<Instrument>(Messages.SelectInstrument)
                .Returns(Observable.Return(instrument));
        }

        /// <summary>
        /// Mocks user input for credentials once (to prevent retry-loops)
        /// </summary>
        private void SetUpCredentialsInputOnce(Credentials credentials)
        {
            UserInterface
                .RequestInput(Messages.EnterUsername)
                .Returns(Observable.Return(credentials.Username), Observable.Never<string>());

            UserInterface
                .RequestInput(Messages.EnterPassword)
                .Returns(Observable.Return(credentials.Password), Observable.Never<string>());
        }

        /// <summary>
        /// Mocks user input for credentials forever (retries)
        /// </summary>
        private (CallCount usernamePrompts, CallCount passwordPrompts) SetUpCredentialsInputRepeated(
            Credentials credentials)
        {
            var (usernamePrompts, passwordPrompts) = (new CallCount(), new CallCount());

            UserInterface
                .RequestInput(Messages.EnterUsername)
                .Returns(Observable.Return(credentials.Username).Do(usernamePrompts.Increment));

            UserInterface
                .RequestInput(Messages.EnterPassword)
                .Returns(Observable.Return(credentials.Password).Do(passwordPrompts.Increment));

            return (usernamePrompts, passwordPrompts);
        }
    }
}