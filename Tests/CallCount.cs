namespace Tests
{
    /// <summary>
    /// Essentially a wrapper for an int, as lambdas can't use ref parameters
    /// </summary>
    public class CallCount
    {
        public int Value { get; private set; }

        // The signature is like this to be easily use with the Do operator
        public void Increment<T>(T unused = default) => ++Value;
    }
}