using NSubstitute;
using NUnit.Framework;
using YousicianCLI;
using YousicianCLI.Backend;

namespace Tests
{
    public abstract class CLITest
    {
        protected IBackend Backend { get; private set; }
        protected IUserInterface UserInterface { get; private set; }
        protected SignupFlow SignupFlow { get; private set; }

        [SetUp]
        public void BaseSetup()
        {
            Backend = Substitute.For<IBackend>();
            UserInterface = Substitute.For<IUserInterface>();
            SignupFlow = new SignupFlow(Backend, UserInterface);
        }
    }
}