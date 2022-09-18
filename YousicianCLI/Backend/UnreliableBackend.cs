using System;
using System.Reactive;
using System.Reactive.Linq;
using YousicianCLI.Experiments;
using YousicianCLI.Models;

namespace YousicianCLI.Backend
{
    public class UnreliableBackend : IBackend
    {
        // Feel free to configure these to perform runtime tests
        private const double MaxDelaySeconds = 10;
        private const double ErrorProbability = 0.5;
        private const double AGroupProbability = 0.5;
        private const double UsernameInUseProbability = 0.6;

        private readonly Random _random = new Random();

        public IObservable<ExperimentGroup> GetExperimentGroup() => Observable.Defer(() =>
            MakeUnreliable(Observable.Return(RandomExperimentGroup())));

        public IObservable<Unit> RegisterUser(Credentials credentials, Instrument? instrument = null)
        {
            return MakeUnreliable(Observable.Return(Unit.Default))
                .Select(result =>
                {
                    if (_random.NextBool(UsernameInUseProbability))
                    {
                        throw new UsernameAlreadyInUseException(credentials.Username);
                    }
                    else
                    {
                        return result;
                    }
                });
        }

        public IObservable<Unit> SetUserInstrument(Credentials credentials, Instrument instrument)
        {
            return MakeUnreliable(Observable.Return(Unit.Default));
        }

        private IObservable<T> MakeUnreliable<T>(IObservable<T> source) => Observable.Defer(() =>
        {
            // Cubic shape to make shorter delays more likely
            var delayFactor = _random.NextDouble() * _random.NextDouble() * _random.NextDouble();

            return Observable
                .Timer(TimeSpan.FromSeconds(delayFactor * MaxDelaySeconds))
                .SelectMany(_ => _random.NextBool(ErrorProbability)
                    ? Observable.Throw<T>(new Exception("Fake network error"))
                    : source);
        });

        private ExperimentGroup RandomExperimentGroup() =>
            _random.NextBool(AGroupProbability) ? ExperimentGroup.A : ExperimentGroup.B;
    }
}