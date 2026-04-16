using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Dog_App.Models;

namespace Dog_App.Controllers
{
    public class DogController : Controller
    {
        private static readonly List<Dog> dogs = new List<Dog>();
        private readonly IWebHostEnvironment _environment;

        public DogController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        // GET: DogController
        public ActionResult Index(string? search)
        {
            IEnumerable<Dog> filteredDogs = dogs;

            if (!string.IsNullOrWhiteSpace(search))
            {
                filteredDogs = dogs.Where(d =>
                    !string.IsNullOrWhiteSpace(d.Name) &&
                    d.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            ViewBag.Search = search;

            return View(filteredDogs.ToList());
        }

        // GET: DogController/Details/5
        public ActionResult Details(int id)
        {
            var dog = GetDogById(id);
            if (dog == null)
            {
                return NotFound();
            }

            return View(dog);
        }

        // GET: DogController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DogController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Dog d, IFormFile? imagefile)
        {
            if (ModelState.IsValid)
            {
                d.ID = dogs.Count == 0 ? 1 : dogs.Max(x => x.ID) + 1;
                d.ImagePath = SaveImage(imagefile);

                dogs.Add(d);
                return RedirectToAction(nameof(Index));
            }

            return View(d);
        }

        // GET: DogController/Edit/5
        public ActionResult Edit(int id)
        {
            var dog = GetDogById(id);
            if (dog == null)
            {
                return NotFound();
            }

            return View(new Dog
            {
                ID = dog.ID,
                Name = dog.Name,
                Age = dog.Age,
                Description = dog.Description,
                ImagePath = dog.ImagePath
            });
        }

        // POST: DogController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Dog d, IFormFile? imagefile)
        {
            var existingDog = GetDogById(id);
            if (existingDog == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                d.ImagePath = string.IsNullOrWhiteSpace(d.ImagePath) ? existingDog.ImagePath : d.ImagePath;
                return View(d);
            }

            existingDog.Name = d.Name;
            existingDog.Age = d.Age;
            existingDog.Description = d.Description;

            var updatedImagePath = SaveImage(imagefile);
            if (!string.IsNullOrWhiteSpace(updatedImagePath))
            {
                existingDog.ImagePath = updatedImagePath;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: DogController/Delete/5
        public ActionResult Delete(int id)
        {
            var dog = GetDogById(id);
            if (dog == null)
            {
                return NotFound();
            }

            return View(dog);
        }

        // POST: DogController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var dog = GetDogById(id);
            if (dog == null)
            {
                return NotFound();
            }

            dogs.Remove(dog);
            return RedirectToAction(nameof(Index));
        }

        private Dog? GetDogById(int id)
        {
            return dogs.FirstOrDefault(d => d.ID == id);
        }

        private string? SaveImage(IFormFile? imagefile)
        {
            if (imagefile == null || imagefile.Length == 0)
            {
                return null;
            }

            var imageName = Guid.NewGuid().ToString() + Path.GetExtension(imagefile.FileName);
            var imageFolder = Path.Combine(_environment.WebRootPath, "images");
            Directory.CreateDirectory(imageFolder);

            var path = Path.Combine(imageFolder, imageName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                imagefile.CopyTo(stream);
            }

            return "/images/" + imageName;
        }
    }
}
