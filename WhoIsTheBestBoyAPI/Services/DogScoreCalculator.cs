using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhoIsTheBestBoyAPI.Services.IServices;

namespace WhoIsTheBestBoyAPI.Services
{
    /// <summary>
    /// This class implements the ELO Rating algorithm
    /// </summary>
    public class DogScoreCalculator : IDogScoreCalculator
    {
        public int KValue = 16;
        public double[] CalculateMatchResult(double scoreFirstContender, double scoreSecondContender, bool firstContenderWon)
        {
            double firstContenderProbablity = 1.0 / (1.0 + Math.Pow(10.0 , (scoreSecondContender - scoreFirstContender)/400.0));
            double secondContenderProbablity = 1.0 / (1.0 + Math.Pow(10.0, (scoreFirstContender - scoreSecondContender)/400.0));


            double firstContenderNewScore;
            double secondContenderNewScore;

            if (firstContenderWon)
            {
                firstContenderNewScore = scoreFirstContender + KValue * (1 - firstContenderProbablity);
                secondContenderNewScore = scoreSecondContender + KValue * (0 - secondContenderProbablity);
            }
            else
            {
                firstContenderNewScore = scoreFirstContender + KValue * (0 - firstContenderProbablity);
                secondContenderNewScore = scoreSecondContender + KValue * (1 - secondContenderProbablity);
            }

            return new double[] { firstContenderNewScore, secondContenderNewScore };
        }
    }
}
