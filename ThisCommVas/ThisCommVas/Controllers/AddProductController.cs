using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace ThisCommVas.Controllers
{
    public class AddProductController : Controller
    {
        // GET: AddProduct
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Artist")]
        public ActionResult SaveProduct(products product, int category_Id, int quantity, int Store_Id, IEnumerable<HttpPostedFileBase> images)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Login");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string userName = User.Identity.Name;
                    int userId = GetUserIdByUsername(userName);
                    product.userId = userId;
                    product.category_Id = category_Id;
                    product.quantity = quantity;
                    product.created_at = DateTime.Now;
                    product.store_id = Store_Id; // Set the selected store_id

                    using (var db = new CommVasEntities())
                    {
                        // Check if the product is being added
                        if (product.product_id == 0) // Adding a new product
                        {
                            db.products.Add(product);
                            db.SaveChanges();

                            int productId = product.product_id;

                            if (productId > 0)
                            {
                                SaveOrUpdateImages(images, productId, db); // Save or update images
                            }
                            else
                            {
                                ViewBag.ErrorMessage = "Failed to save the product.";
                            }
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Invalid product ID.";
                        }
                    }

                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
                }
            }

            using (var db = new CommVasEntities())
            {
                ViewBag.Categories = db.categories.ToList();
                ViewBag.Stores = db.stores.ToList();
            }

            return View("AddProduct", product);
        }




        private void SaveOrUpdateImages(IEnumerable<HttpPostedFileBase> images, int productId, CommVasEntities db)
        {
            foreach (var image in images)
            {
                if (image != null && image.ContentLength > 0)
                {
                    byte[] imageData = null;
                    using (var binaryReader = new BinaryReader(image.InputStream))
                    {
                        imageData = binaryReader.ReadBytes(image.ContentLength);
                    }

                    var imageEntity = new product_image
                    {
                        image_path = imageData,
                        productID = productId
                    };

                    db.product_image.Add(imageEntity);
                }
            }

            db.SaveChanges();
        }


        public ActionResult Add()
        {
            using (var db = new CommVasEntities())
            {
                ViewBag.Categories = db.categories.ToList();
                ViewBag.Stores = db.stores.ToList();
            }

            return View();
        }

        private int GetUserIdByUsername(string userName)
        {
            using (var db = new CommVasEntities())
            {
                var user = db.users.FirstOrDefault(u => u.username == userName);
                if (user != null)
                {
                    return user.id;
                }
            }

            throw new Exception($"User with username '{userName}' not found in database.");
        }
    }
}
