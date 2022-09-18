using System;
using NUnit.Framework;

namespace Tests
{
    public static class TestExtensions
    {
        public static T ExpectSingleValueSynchronously<T>(this IObservable<T> observable)
        {
            var completed = false;
            var numValues = 0;
            T value = default;
            observable
                .Subscribe(
                    val =>
                    {
                        value = val;
                        ++numValues;
                    },
                    () => completed = true)
                .Dispose();

            Assert.IsTrue(completed, "Observable should complete synchronously");
            Assert.AreEqual(1, numValues, "Observable should produce a single value synchronously");

            return value;
        }
    }
}