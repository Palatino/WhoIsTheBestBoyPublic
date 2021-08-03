using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhoIsTheBestBoyAPI.Services.IServices
{
    public interface IDogScoreCalculator
    {
        public double[] CalculateMatchResult(double scoreFirstContendant, double scoreSecondContendant, bool FirstContendantWon);
    }
}
