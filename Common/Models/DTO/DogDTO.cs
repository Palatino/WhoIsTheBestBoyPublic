using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.DTO
{
    public class DogDTO
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string AvatarURL { get; set; }
        public bool Approved { get; set; }
        public int NumberOfMatches { get; set; }
        public int NumberOfWins { get; set; }
        public int DaysAtNumberOne { get; set; }
        public int DaysAtTopTen { get; set; }
    }
}
