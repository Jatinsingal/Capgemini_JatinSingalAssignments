using Microsoft.AspNetCore.Mvc;
using CosmosDB_Demo.Models;
using CosmosDB_Demo.Data;

namespace CosmosDB_Demo.Controllers
{
    public class ItemsController : Controller
    {
        private readonly CosmosDbService _cosmosDbService;

        public ItemsController(CosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        // GET: Items
        public async Task<IActionResult> Index()
        {
            var items = await _cosmosDbService.GetItemsAsync("SELECT * FROM c");
            return View(items);
        }

        // DETAILS
        public async Task<IActionResult> Details(string id)
        {
            var item = await _cosmosDbService.GetItemAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        // CREATE (GET)
        public IActionResult Create()
        {
            return View();
        }

        // CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Itemmodel item)
        {
            if (ModelState.IsValid)
            {
                await _cosmosDbService.AddItemAsync(item);
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        // EDIT (GET)
        public async Task<IActionResult> Edit(string id)
        {
            var item = await _cosmosDbService.GetItemAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        // EDIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Itemmodel item)
        {
            if (id != item.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _cosmosDbService.UpdateItemAsync(id, item);
                return RedirectToAction(nameof(Index));
            }

            return View(item);
        }

        // DELETE (GET)
        public async Task<IActionResult> Delete(string id)
        {
            var item = await _cosmosDbService.GetItemAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        // DELETE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _cosmosDbService.DeleteItemAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}