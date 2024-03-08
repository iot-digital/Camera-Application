using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ParkingDemo.Data;
using ParkingDemo.Models;
using ParkingDemo.Utilities;
using ParkingDemo.ViewModels;

namespace ParkingDemo.Pages.CameraLogs
{
    //[Authorize(Roles = "ADMIN,USER")]
    public class IndexModel : PageModel
    {
        private readonly AppRepository _repository;

        [BindProperty]
        public DateTime? StartDate { get; set; }
        [BindProperty]
        public DateTime? EndDate { get; set; }
        [BindProperty]
        public string? Search { get; set; }
        [BindProperty]
        public int? ParkingSlotId { get; set; }

        [BindProperty]
        public int PageNumber { get; set; } = 1;
        [BindProperty]
        public int NextPageNumber { get; set; } = 0;
        [BindProperty]
        public int PrevPageNumber { get; set; } = 2;
        [BindProperty]
        public int PageSize { get; set; } = 50;
        [BindProperty]
        public int TotalCount { get; set; } = 0;

        [BindProperty]
        public int LocaleUtcOffset { get; set; } 

        public int NumberOfPages { get; set; } = 1;
        public bool IsNextPageDisabled { get; set; } = false;
        public bool IsPrevPageDisabled { get; set; } = false;

        public IList<CameraLogVM> CameraLogs { get; set; }
        public IEnumerable<Device>? DevicesList { get; set; }

        public IndexModel(AppRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            await PopulateUserDevices();

            if (id > 0)
                ParkingSlotId = id;
            else
                id = null;

            DevicesList = await _repository.GetDevicesAsync();

            TotalCount = await _repository.GetCameraLogsCountAsync(id);

            CalculatePageNumbers(PageNumber, TotalCount);

            CameraLogs = await _repository.GetCameraLogsAsync(
            id,
            pageNumber: PageNumber,
            pageSize: PageSize);

            return Page();
        }

        public async Task<IActionResult> OnPostFilterAsync(int pageNumber = 1)
        {
            await PopulateUserDevices();

            TotalCount = await _repository.GetCameraLogsCountAsync(
                ParkingSlotId,
                Search,
                StartDate,
                EndDate,
                LocaleUtcOffset);

            CalculatePageNumbers(pageNumber, TotalCount);

            CameraLogs = await _repository.GetCameraLogsAsync(
                ParkingSlotId,
                Search,
                StartDate,
                EndDate,
                pageNumber,
                PageSize,
                LocaleUtcOffset);

            return Page();
        }

        public async Task<IActionResult> OnPostFilterPrevAsync()
        {
            return await OnPostFilterAsync(PrevPageNumber);
        }

        public async Task<IActionResult> OnPostFilterNextAsync()
        {
            return await OnPostFilterAsync(NextPageNumber);
        }



        public IActionResult OnPostClearFilter()
        {
            return RedirectToPage("Index");
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var result = await _repository.DeleteCameraLogAsync(id);

            Notify.Add(TempData, result, "Log deleted successfully", "Couldn't delete the log. Try again.");

            CameraLogs = await _repository.GetCameraLogsAsync(
                ParkingSlotId,
                Search,
                StartDate,
                EndDate,
                PageNumber,
                PageSize,
                LocaleUtcOffset);

            await PopulateUserDevices();

            return Page();
        }

        private async Task PopulateUserDevices()
        {
            var slots = new List<string>();

            ViewData["slots"] = new SelectList(slots, "Id", "Name");
        }

       
        private void CalculatePageNumbers(int pageNumber, int totalCount)
        {
            PageNumber = pageNumber;
            NumberOfPages = (int)Math.Ceiling((double)(totalCount / PageSize)) + 1;

            if (pageNumber > 1)
            {
                PrevPageNumber = pageNumber - 1;
            }
            else
            {
                PrevPageNumber = 0;
                IsPrevPageDisabled = true;
            }

            if (pageNumber < NumberOfPages)
            {
                NextPageNumber = pageNumber + 1;
            }
            else
            {
                NextPageNumber = NumberOfPages;
                IsNextPageDisabled = true;
            }
        }
    }
}
