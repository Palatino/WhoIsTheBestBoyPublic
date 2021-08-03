using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.BussinesModels
{
    public class Dog
    {
        public int Id { get; set; }
        public bool Approved { get; set; }
        public string AvatarName { get; set; }
        public string Name { get; set; }
        public string CreatedById { get; set; }
        public double Score { get; set; } = 500;
        public int NumberOfMatches { get; set; }
        public int NumberOfWins { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;
        public int DaysAtNumberOne { get; set; }
        public int DaysAtTopTen { get; set; }
    }
}
