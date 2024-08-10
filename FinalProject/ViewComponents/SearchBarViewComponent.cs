using FinalProject.Context;
using FinalProject.Models;
using FinalProject.ViewModels.LayoutViewModels;
using FinalProject.ViewModels.Searc;
using FinalProject.ViewModels.Search;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.ViewComponents;

public class SearchBarViewComponent : ViewComponent
{

    [HttpPost]
    public async Task<IViewComponentResult> InvokeAsync()
    {
        SearchViewModel search = new SearchViewModel();
        return View(search);
    }
}
