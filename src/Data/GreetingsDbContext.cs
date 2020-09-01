using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public class GreetingsDbContext : DbContext
    {
        public GreetingsDbContext(DbContextOptions<GreetingsDbContext> options) : base(options)
        {
        }

        public DbSet<Greeting> Greetings { get; set; }
    }
}
