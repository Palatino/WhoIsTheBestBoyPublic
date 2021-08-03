using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Common.Extensions
{
    public static class ListExtensions
    {
        public static List<T> CreateShuffled<T>(this List<T> list)
        {
            Random rng = new Random();
            return list.OrderBy(a => rng.Next()).ToList();
        }
    }

}
