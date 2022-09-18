using System;
using System.Reactive;
using YousicianCLI.Experiments;
using YousicianCLI.Models;

namespace YousicianCLI.Backend
{
    public interface IBackend
    {
        IObservable<ExperimentGroup> GetExperimentGroup();
        IObservable<Unit> RegisterUser(Credentials credentials, Instrument? instrument = null);
        IObservable<Unit> SetUserInstrument(Credentials credentials, Instrument instrument);
    }
}