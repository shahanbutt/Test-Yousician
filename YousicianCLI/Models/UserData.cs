namespace YousicianCLI.Models
{
    public readonly struct UserData
    {
        public readonly Credentials Credentials;
        public readonly Instrument Instrument;

        public UserData(Credentials credentials, Instrument instrument)
        {
            Credentials = credentials;
            Instrument = instrument;
        }
    }
}