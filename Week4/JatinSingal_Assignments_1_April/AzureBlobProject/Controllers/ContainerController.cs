using AzureBlobProject.Models;
using AzureBlobProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace AzureBlobProject.Controllers
{
    public class ContainerController : Controller
    {
        private readonly IContainerService _containerService;

        public ContainerController(IContainerService containerService)
        {
            _containerService = containerService;
        }

        public async Task<IActionResult> Index()
        {
            var allContainer = await _containerService.GetAllContainer();
            return View(allContainer);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new ContainerModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContainerModel container)
        {
            if (!ModelState.IsValid)
            {
                return View(container);
            }

            try
            {
                await _containerService.CreateContainer(container.Name);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(container);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string containerName)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(containerName))
                {
                    await _containerService.DeleteContainer(containerName);
                }
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
