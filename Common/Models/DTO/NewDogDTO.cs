using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.DTO
{
    public class NewDogDTO
    {
        [Required]
        public byte[] Avatar { get; set; }
        [Required(ErrorMessage ="Please provide name")]
        
        public string Name { get; set; }

        public string CreatedById { get; set; }
    }
}
