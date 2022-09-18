using System;
using System.Reactive.Linq;
using YousicianCLI.Backend;
using YousicianCLI.Experiments;
using YousicianCLI.Models;

namespace YousicianCLI
{
    public class SignupFlow
    {
        private readonly IBackend _backend;
        private readonly IUserInterface _userInterface;
        private IObservable<UserData> _observableUserData;


        public SignupFlow(IBackend backend, IUserInterface userInterface)
        {
            _backend = backend;
            _userInterface = userInterface;
        }

        public IObservable<UserData> Run()
        {
            IObservable<Experiments.ExperimentGroup> observableUG =
                _backend.GetExperimentGroup();
            observableUG.Subscribe(ReceiveGroupWithSuccess);
            _userInterface.ShowProgressIndicator(Messages.Loading);
            observableUG.Wait();

            return _observableUserData;

        }

        private void ReceiveGroupWithSuccess(ExperimentGroup obj)
        {
            if(obj == ExperimentGroup.A)
            {
                CreateUser();
            }
            else if (obj == ExperimentGroup.B)
            {
                CreateUserWithGroupB();
            }
                    
        }

        private IObservable<Credentials> CreateUserWithGroupB()
            => _userInterface
                .RequestInput(Messages.SelectInstrument)
                .SelectMany(instrument =>_userInterface
                           .RequestInput(Messages.EnterUsername)
                           .SelectMany(username => _userInterface
                               .RequestInput(Messages.EnterPassword)
                               .Select(password => new Credentials(username, password)))
                           .SelectMany(credentials => _backend
                               .RegisterUser(credentials)
                               .ShowProgressIndicator(Messages.SigningUp, _userInterface)
                               .Select(_ => credentials)));

        private IObservable<Credentials> CreateUser()
            => _userInterface
                .RequestInput(Messages.EnterUsername)
                .SelectMany(username => _userInterface
                    .RequestInput(Messages.EnterPassword)
                    .Select(password => new Credentials(username, password)))
                .SelectMany(credentials => _backend
                    .RegisterUser(credentials)
                    .ShowProgressIndicator(Messages.SigningUp, _userInterface)
                    .Select(_ => credentials));

        private IObservable<UserData> SelectInstrument(Credentials credentials)
            => _userInterface
                .RequestChoice<Instrument>(Messages.SelectInstrument)
                .RetryWithErrorMessages(_userInterface, GetInstrumentError)
                .SelectMany(instrument => _backend
                    .SetUserInstrument(credentials, instrument)
                    .ShowProgressIndicator(Messages.SelectingInstrument, _userInterface)
                    .Select(_ => new UserData(credentials, instrument)));

        private static string GetBackendError(Exception e)
            => e is UsernameAlreadyInUseException ? Messages.UsernameInUse : null;

        private static string GetInstrumentError(Exception e)
            => e is ArgumentException ? Messages.InvalidInstrument : null;


    }
}