using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static Api.Greeter;

namespace Frontend.Features.Greetings
{
    public class CreateModel : PageModel
    {
        private readonly GreeterClient _greeterClient;

        public CreateModel(GreeterClient greeterClient)
        {
            _greeterClient = greeterClient;
        }

        [BindProperty] public string Name { get; set; }
        
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _greeterClient.SayHelloAsync(new Api.HelloRequest { Name = Name });
            return RedirectToPage("/Greetings/List");
        }
    }
}
