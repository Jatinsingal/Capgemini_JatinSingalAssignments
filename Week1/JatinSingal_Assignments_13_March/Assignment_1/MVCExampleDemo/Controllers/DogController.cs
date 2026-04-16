using Microsoft.AspNetCore.Http;
using MVCExampleDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;

namespace MVCExampleDemo.Controllers
{
    public class DogController : Controller
    {
        // GET: DogController
        static private List<Dog> dogs = new List<Dog>();
        public ActionResult Index()
        {
            return View(dogs);
        }

        [HttpGet]
        public ActionResult DirectDelete(int id)
        {
            Dog d = new Dog();
            foreach(var dog in dogs)
            {
                if(dog.ID == id)
                {
                    dogs.Remove(dog);
                    break;
                }
            }
            return RedirectToAction("Index");
        }
        // GET: DogController/Details/5
        public ActionResult Details(int id)
        {
            Dog d = new Dog();
            foreach(Dog dog in dogs)
            {
                if(dog.ID == id)
                {
                    d.ID = dog.ID;
                    d.Name = dog.Name;
                    d.Age = dog.Age;
                }
            }
            return View(d);
        }

        // GET: DogController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DogController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Dog d)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dogs.Add(d);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View("Create", d);
                }
            }
            catch
            {
                return View("Create", d);
            }
        }

        // GET: DogController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DogController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Dog d)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Edit", d);
                }
                else
                {
                    foreach(Dog dog in dogs)
                    {
                        if(dog.ID == d.ID)
                        {
                            dog.Name = d.Name;
                            dog.Age = d.Age;
                        }
                    }
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                return View("Edit", d);
            }
        }

        // GET: DogController/Delete/5
        public ActionResult Delete(int id)
        {
            Dog d = dogs.FirstOrDefault(x => x.ID == id);

            return View(d);
        }

        // POST: DogController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                Dog d = dogs.FirstOrDefault(x => x.ID == id);

                if (d != null)
                {
                    dogs.Remove(d);
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
