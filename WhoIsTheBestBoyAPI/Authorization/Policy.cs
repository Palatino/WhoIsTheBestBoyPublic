using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhoIsTheBestBoyAPI.Authorization
{
    /// <summary>
    /// Class containing the names of the different policies
    /// </summary>
    public static class Policy
    {
        public const string DogAprove = "dogs:approve";
        public const string DeleteAny = "dogs:deleteAny";
    }
}
