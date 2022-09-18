using System;

namespace YousicianCLI
{
    public static class RandomExtensions
    {
        public static bool NextBool(this Random random, double trueProbability) =>
            Math.Abs(trueProbability - 1) < double.Epsilon ||
            (trueProbability > 0 && random.NextDouble() <= trueProbability);
    }
}