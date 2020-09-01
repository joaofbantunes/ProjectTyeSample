using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Frontend.Features.Greetings
{
    public class ListModel : PageModel
    {
        private readonly GreetingsDbContext _db;

        public ListModel(GreetingsDbContext db)
        {
            _db = db;
        }

        public class GreetingViewModel
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public DateTime SubmittedAt { get; set; }
        }

        public IReadOnlyCollection<GreetingViewModel> Greetings { get; set; }

        public async Task OnGetAsync()
        {
            var greetings = await _db.Set<Greeting>().ToListAsync();
            Greetings = greetings.Select(g => new GreetingViewModel { Id = g.Id, Name = g.Name, SubmittedAt = g.SubmittedAt }).ToList();
        }
    }
}
