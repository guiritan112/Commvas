using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace ThisCommVas.Controllers
{
    public class EditController : Controller
    {
        private CommVasEntities context = new CommVasEntities();

        // GET: Edit
        public ActionResult Index(int productId)
        {
            var categories = GetCategories();
            ViewBag.Categories = categories;

            var stores = GetStores();
            ViewBag.Stores = stores;

            var product = GetProduct(productId);
            return View(product);
        }

        // Method to fetch a specific product by ID
        private products GetProduct(int productId)
        {
            return context.products
                .Include(p => p.product_image)
                .Include(p => p.categories)
                .SingleOrDefault(p => p.product_id == productId);
        }


        // Redirect to AddProduct controller for adding new product
        public ActionResult AddProduct()
        {
            return RedirectToAction("Add", "AddProduct");
        }

        // Fetch product by ID for editing
        public ActionResult EditProductAction(int productId)
        {
            var product = context.products
                .Include(p => p.product_image)
                .Include(p => p.categories)
                .SingleOrDefault(p => p.product_id == productId);

            if (product == null)
            {
                return HttpNotFound();
            }

            ViewBag.Categories = GetCategories();
            ViewBag.Stores = GetStores();

            // Pass the single product for editing
            return View(product);
        }

        // Save edited product
        // Save edited product
        [HttpPost]
        public ActionResult SaveEditedProduct(products product, int category_Id, int quantity, int store_Id, IEnumerable<HttpPostedFileBase> images)
        {
            // Check if the user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                // Redirect to the login page if not authenticated
                return RedirectToAction("Index", "Login");
            }

            try
            {
                // Check model state
                if (ModelState.IsValid)
                {
                    string userName = User.Identity.Name;
                    int userId = GetUserIdByUsername(userName);
                    product.userId = userId;
                    product.category_Id = category_Id;
                    product.quantity = quantity;
                    product.store_id = store_Id;

                    // Fetch the original created_at value from the database
                    var originalProduct = context.products.FirstOrDefault(p => p.product_id == product.product_id);
                    if (originalProduct != null)
                    {
                        product.created_at = originalProduct.created_at; // Assign the original created_at value
                    }

                    // Check if the product ID is valid
                    if (product.product_id != 0)
                    {
                        using (var db = new CommVasEntities())
                        {
                            db.Entry(product).State = EntityState.Modified;
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

                        // Redirect to the edited product page after successful save
                        return RedirectToAction("EditProductAction", new { productId = product.product_id });
                    }
                    else
                    {
                        // Redirect to the index page if the product ID is invalid
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions by displaying an error message
                ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
            }

            // Fetch categories and return to the edit product view
            ViewBag.Categories = GetCategories();
            ViewBag.Stores = GetStores();
            return RedirectToAction("Index", "Home");
        }



        // Method to fetch products
        private List<products> GetProducts()
        {
            return context.products.ToList();
        }

        // Method to fetch categories
        private List<categories> GetCategories()
        {
            return context.categories.ToList();
        }

        // Method to fetch stores
        private List<stores> GetStores()
        {
            return context.stores.ToList();
        }

        // Method to get user ID by username
        private int GetUserIdByUsername(string userName)
        {
            var user = context.users.FirstOrDefault(u => u.username == userName);
            if (user != null)
            {
                return user.id;
            }

            throw new Exception($"User with username '{userName}' not found in database.");
        }

        // Method to save or update images
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
    }
}