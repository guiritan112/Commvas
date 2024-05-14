using System;
using System.Linq;
using System.Web.Mvc;
using ThisCommVas.Models;

namespace ThisCommVas.Controllers
{
    public class CartController : Controller
    {
        private readonly CommVasEntities _context;

        public CartController()
        {
            _context = new CommVasEntities(); // Initialize your DbContext
        }

        // GET: Cart
        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Login"); // Redirect to login if user is not authenticated
            }

            try
            {
                var userName = User.Identity.Name;
                var userId = GetUserIdByUsername(userName); // Assuming you have a method to get userId based on username

                // Retrieve cart items for the authenticated user
                var cartItems = _context.users.Where(c => c.id == userId).ToList();
                ViewBag.CartItems = cartItems;
                ViewBag.UserId = userId; // Pass userId to the view

                return View();
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                ViewBag.ErrorMessage = $"An error occurred while fetching cart items: {ex.Message}";
                return View(); // You might want to return an error view here
            }
        }

        [HttpGet]
        public ActionResult GetCartItems()
        {
            try
            {
                // Check if the user is authenticated
                if (!User.Identity.IsAuthenticated)
                {
                    return Json(new { success = false, message = "User is not authenticated." }, JsonRequestBehavior.AllowGet);
                }

                // Get the authenticated user's username
                var userName = User.Identity.Name;

                // Retrieve the user's ID from the database using their username
                var userId = GetUserIdByUsername(userName);

                // Fetch the cart items for the authenticated user
                var cartItems = _context.Cart
                    .Where(c => c.UserId == userId)
                    .ToList(); // Materialize the query

                // Convert image data to Base64 strings
                var cartItemsWithBase64Images = cartItems.Select(c => new
                {
                    c.CartId,
                    c.ProductId,
                    c.Quantity,
                    StoreId = c.StoreId, // Include StoreId from Cart
                    AvailableQuantity = c.AvailableQuantity, // Include AvailableQuantity from Cart
                    ProductName = c.products.name, // Include product name
                    ProductDescription = c.products.description, // Include product description
                    ProductPrice = c.products.price, // Include product price
                    ProductImages = c.products.product_image.Select(pi => Convert.ToBase64String(pi.image_path)), // Convert image data to Base64 strings
                }).ToList(); // Materialize the projection

                // Add this block to increase the maxJsonLength property
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;

                // Convert your data to JSON using the serializer
                var jsonResult = new ContentResult
                {
                    Content = serializer.Serialize(cartItemsWithBase64Images),
                    ContentType = "application/json"
                };

                return jsonResult;
            }
            catch (Exception ex)
            {
                // Return an error message
                return Json(new { success = false, message = $"An error occurred while fetching cart items: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Artist")]
        public ActionResult AddToCart(int productId)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Json(new { success = false, message = "User is not authenticated." });
                }

                var userName = User.Identity.Name;
                var userId = GetUserIdByUsername(userName);

                // Check if the product already exists in the cart for the user
                var existingCartItem = _context.Cart.FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);

                if (existingCartItem != null)
                {
                    // If the product already exists in the cart, return a message
                    return Json(new { success = false, message = "Product already in cart." });
                }
                else
                {
                    // Fetch the product details
                    var product = _context.products.FirstOrDefault(p => p.product_id == productId);

                    if (product != null)
                    {
                        // If the product does not exist in the cart, create a new cart item
                        var newCartItem = new Cart
                        {
                            UserId = userId,
                            ProductId = productId,
                            Quantity = 1, // Set default quantity to 1
                            StoreId = product.store_id, // Set store ID from product
                            AvailableQuantity = product.quantity
                        };

                        _context.Cart.Add(newCartItem);
                        _context.SaveChanges();

                        return Json(new { success = true, message = "Product added to cart." });
                    }
                    else
                    {
                        // Return a message if the product does not exist
                        return Json(new { success = false, message = "Product not found." });
                    }
                }
            }
            catch (Exception ex)
            {
                // Return an error message
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        private int GetUserIdByUsername(string userName)
        {
            var user = _context.users.FirstOrDefault(u => u.username == userName); // Assuming 'Username' is the correct property name in your User entity

            if (user != null)
            {
                return user.id; // Assuming 'Id' is the correct property name for UserId in your User entity
            }

            throw new Exception($"User with username '{userName}' not found in database.");
        }

        public ActionResult Checkout(int userId)
        {
            try
            {
                // Retrieve user information and other transaction data
                var user = _context.users.Find(userId);
                var personalInfo = user?.PersonalInformation.FirstOrDefault();

                // Check if user and personalInfo are not null
                if (user != null && personalInfo != null)
                {
                    // Retrieve the user's cart items
                    var cartItems = _context.Cart.Where(c => c.UserId == userId).ToList();

                    // Calculate the total amount by summing up the prices of all cart items
                    decimal totalAmount = cartItems.Sum(item => (item.products.price ?? 0) * item.Quantity).Value;

                    // Create a new transaction instance
                    var transaction = new ThisCommVas.Transactions
                    {
                        UserId = userId,
                        Timestamp = DateTime.Now,
                        Amount = totalAmount, // Set the total amount
                    };

                    // Add the transaction to the database context
                    _context.Transactions.Add(transaction);

                    // Update the quantity of products in the cart
                    foreach (var cartItem in cartItems)
                    {
                        var product = _context.products.FirstOrDefault(p => p.product_id == cartItem.ProductId);
                        if (product != null)
                        {
                            // Decrease the quantity in the product table
                            product.quantity -= cartItem.Quantity;
                        }
                    }

                    // Save changes to the database
                    _context.SaveChanges();

                    // Set ViewBag properties for view rendering
                    ViewBag.FirstName = personalInfo.FirstName;
                    ViewBag.LastName = personalInfo.LastName;
                    ViewBag.Email = user.email;
                    ViewBag.UserId = userId; // Pass the userId to the view

                    // Pass the total amount to the view
                    ViewBag.TotalAmount = totalAmount;

                    // Return the view for successful transaction
                    return View();
                }
                else
                {
                    // Redirect to error page if user or personalInfo is null
                    return RedirectToAction("Error");
                }
            }
            catch (Exception ex)
            {
                // Handle errors appropriately
                ViewBag.ErrorMessage = $"An error occurred while saving the transaction: {ex.Message}";
                return RedirectToAction("Index", "Home"); // Redirect to home page or error page
            }
        }
        

        // Your other actions...

        public ActionResult Transactions()
        {
            try
            {
                // Get the username of the currently logged-in user
                var userName = User.Identity.Name;

                // Retrieve the user ID based on the username
                var userId = GetUserIdByUsername(userName);

                using (var context = new CommVasEntities()) // Create an instance of your DbContext
                {
                    // Retrieve transactions for the specified user ID
                    var transactions = context.Transactions
                        .Where(t => t.UserId == userId)
                        .OrderByDescending(t => t.Timestamp)
                        .Take(1000)
                        .ToList();

                    // Pass the transactions to the view
                    return View(transactions);
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                ViewBag.ErrorMessage = $"An error occurred while fetching transactions: {ex.Message}";
                return View("Error"); // Return an error view here
            }
        }
  

    }
}
