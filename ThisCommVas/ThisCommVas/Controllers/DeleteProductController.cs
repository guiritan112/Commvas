using System;
using System.Linq;
using System.Web.Mvc;

namespace ThisCommVas.Controllers
{
    public class DeleteProductController : Controller
    {
        // GET: DeleteProduct
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int productId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, message = "User is not authenticated." });
            }

            try
            {
                using (var db = new CommVasEntities())
                {
                    // Delete product images
                    var productImages = db.product_image.Where(pi => pi.productID == productId).ToList();
                    foreach (var image in productImages)
                    {
                        db.product_image.Remove(image);
                    }

                    // Delete cart items related to the product
                    var cartItems = db.Cart.Where(ci => ci.ProductId == productId).ToList();
                    foreach (var cartItem in cartItems)
                    {
                        db.Cart.Remove(cartItem);
                    }

                    // Delete the product itself
                    var product = db.products.Find(productId);
                    if (product != null)
                    {
                        db.products.Remove(product);
                    }

                    db.SaveChanges();

                    // Return success response
                    return Json(new { success = true });
                }
            }
            catch (Exception ex)
            {
                // Return error response
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Artist")]
        public ActionResult DeleteCart(int cartItemId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, message = "User is not authenticated." });
            }

            try
            {
                using (var db = new CommVasEntities())
                {
                    // Find the cart item based on the cartItemId
                    var cartItem = db.Cart.FirstOrDefault(ci => ci.CartId == cartItemId);
                    if (cartItem != null)
                    {
                        // Remove the cart item
                        db.Cart.Remove(cartItem);
                        db.SaveChanges();
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Cart item not found." });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

    }
}

   