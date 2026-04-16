using AzureBlobProject.Models;
using AzureBlobProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace AzureBlobProject.Controllers
{
    public class BlobController : Controller
    {
        private readonly IBlobService _blobService;

        public BlobController(IBlobService blobService)
        {
            _blobService = blobService;
        }

        [HttpGet]
        public async Task<IActionResult> Manage(string containerName)
        {
            ViewBag.ContainerName = containerName;

            try
            {
                var blobsObj = await _blobService.GetAllBlobs(containerName);
                return View(blobsObj);
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Container");
            }
        }

        [HttpGet]
        public IActionResult AddFile(string containerName)
        {
            ViewBag.ContainerName = containerName;
            return View(new BlobModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFile(string containerName, IFormFile? file, BlobModel blobModel)
        {
            ViewBag.ContainerName = containerName;

            if (string.IsNullOrWhiteSpace(containerName))
            {
                return RedirectToAction("Index", "Container");
            }

            if (file == null || file.Length < 1)
            {
                ModelState.AddModelError("file", "Please choose a file to upload.");
                return View(blobModel);
            }

            string fileName = Path.GetFileNameWithoutExtension(file.FileName)
                + "_"
                + Guid.NewGuid()
                + Path.GetExtension(file.FileName);

            try
            {
                var result = await _blobService.CreateBlob(fileName, file, containerName, blobModel);

                if (result)
                {
                    return RedirectToAction(nameof(Manage), new { containerName });
                }
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(blobModel);
            }

            ModelState.AddModelError(string.Empty, "The blob could not be uploaded.");
            return View(blobModel);
        }

        [HttpGet]
        public async Task<IActionResult> ViewFile(string name, string containerName)
        {
            string blobUrl = await _blobService.GetBlob(name, containerName);

            if (string.IsNullOrWhiteSpace(blobUrl))
            {
                TempData["ErrorMessage"] = "BlobConnection is missing. Add it to appsettings.json or user secrets before using blob features.";
                return RedirectToAction("Index", "Container");
            }

            return Redirect(blobUrl);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteFile(string name, string containerName)
        {
            try
            {
                await _blobService.DeleteBlob(name, containerName);
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Container");
            }

            return RedirectToAction(nameof(Manage), new { containerName });
        }
    }
}
