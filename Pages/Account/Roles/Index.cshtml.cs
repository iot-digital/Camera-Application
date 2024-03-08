using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ParkingDemo.Data;
using ParkingDemo.Models;

namespace ParkingDemo.Pages.Roles;

[Authorize(Roles = "ADMIN")]
public class IndexModel : PageModel
{
    private readonly AppRepository _repository;

    public IEnumerable<AppRole> AppGroups { get; set; } = default!;

    public IndexModel(AppRepository repository)
    {
        _repository = repository;
    }

    public async Task OnGetAsync()
    {
        AppGroups = await _repository.GetAppRolesAsync();
    }
}
