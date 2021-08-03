using System;
using WhoIsTheBestBoyAPI.Services;
using Xunit;

namespace BestBoyTests
{
    public class ScoreCalculatorTest
    {
        [Fact]
        public void ScoreCalculator1()
        {
            var calculator = new DogScoreCalculator();

            var result = calculator.CalculateMatchResult(2600, 2300, true);

            Assert.Equal(2602, result[0], 0);
            Assert.Equal(2298, result[1], 0);
        }

        [Fact]
        public void ScoreCalculator2()
        {
            var calculator = new DogScoreCalculator();

            var result = calculator.CalculateMatchResult(2600, 2300, false);

            Assert.Equal(2586, result[0], 0);
            Assert.Equal(2314, result[1], 0);
        }
    }
}
