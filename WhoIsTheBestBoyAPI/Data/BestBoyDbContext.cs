using Common.Models.BussinesModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhoIsTheBestBoyAPI.Data
{
    public class BestBoyDbContext : DbContext
    {

        public BestBoyDbContext(DbContextOptions<BestBoyDbContext> options)
            : base(options)
        { }

        public DbSet<Dog> Dogs { get; set; }
    }
}
