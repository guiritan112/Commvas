// Function to update the summary section
function updateSummary() {
    var cartItems = getCartItems();
    // Calculate total price
    var totalPrice = 0;
    cartItems.forEach(function (item) {
        totalPrice += item.ProductPrice * item.Quantity;
    });

    // Apply discount if applicable
    var discount = 0; // You can implement discount functionality here

    // Calculate total including discount
    var totalPriceWithDiscount = totalPrice - discount;

    // Add shipping cost
    var shippingCost = 5.00;
    totalPriceWithDiscount += shippingCost;

    // Update summary section
    $('#summary-item-count').text(cartItems.length);
    $('#summary-total-price1').text('₱ ' + totalPriceWithDiscount.toFixed(2));
    $('#summary-total-price2').text('₱ ' + totalPriceWithDiscount.toFixed(2));

    // Show success message using Sweet Alert

}

$(document).ready(function () {
    // Fetch cart items and update UI
    fetchCartItems();

    // Handle quantity increase
    $(document).on('click', '.quantity-plus', function (e) {
        e.preventDefault();
        var $quantityElement = $(this).siblings('.quantity');
        var currentQuantity = parseInt($quantityElement.text());
        var maxQuantity = parseInt($(this).closest('.main').find('.row.text-muted .col').eq(1).text().trim()); // Get the available quantity
        if (currentQuantity < maxQuantity) { // Check if current quantity is less than available quantity
            var newQuantity = currentQuantity + 1;
            $quantityElement.text(newQuantity);
            // Update product price when quantity changes
            updateProductPrice($(this).closest('.main').find('.product-price'), newQuantity);
            // Update summary
            updateSummary();
        } else {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'You cannot add more than the available quantity.'
            });
        }
    });

    // Handle quantity decrease
    $(document).on('click', '.quantity-minus', function (e) {
        e.preventDefault();
        var $quantityElement = $(this).siblings('.quantity');
        var currentQuantity = parseInt($quantityElement.text());
        if (currentQuantity > 1) {
            var newQuantity = currentQuantity - 1;
            $quantityElement.text(newQuantity);
            // Update product price when quantity changes
            updateProductPrice($(this).closest('.main').find('.product-price'), newQuantity);
            // Update summary
            updateSummary();
        }
    });

    // Function to update product price based on quantity
    function updateProductPrice($priceElement, newQuantity) {
        var productPrice = parseFloat($priceElement.data('price'));
        var newPrice = productPrice * newQuantity;
        $priceElement.text('₱ ' + newPrice.toFixed(2));
    }

    // Enable carousel auto-sliding
    $('.carousel').carousel({
        interval: 2000 // Set the interval to 2 seconds (2000 milliseconds)
    });

    // Delete product from cart
    $(document).on('click', '.delete-product-btn', function (e) {
        e.preventDefault();
        var deleteUrl = "/DeleteProduct/Delete"; // URL to Delete action in DeleteProductController
        var productId = $(this).data('product-id');

        // Confirm deletion with Sweet Alert
        Swal.fire({
            icon: 'question',
            title: 'Confirmation',
            text: 'Are you sure you want to delete this product?',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.isConfirmed) {
                // Proceed with deletion
                $.ajax({
                    url: deleteUrl,
                    type: 'POST',
                    data: { productId: productId },
                    success: function (result) {
                        if (result.success) {
                            Swal.fire({
                                icon: 'success',
                                title: 'Success',
                                text: 'Product deleted successfully!'
                            }).then(function () {
                                location.reload();

                                fetchCartItems(); // Fetch latest cart items
                            });
                        } else {
                            Swal.fire({
                                icon: 'error',
                                title: 'Error',
                                text: result.message
                            });
                        }
                    },
                    error: function (error) {
                        console.log(error);
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: 'Error deleting the product.'
                        });
                    }
                });
            }
        });
    });

    // Handle removing item from cart
    $(document).on('click', '.remove-from-cart-btn', function (e) {
        e.preventDefault();
        var deleteCartUrl = "/DeleteProduct/DeleteCart"; // URL to DeleteCart action in DeleteProductController
        var cartItemId = $(this).data('cart-item-id'); // Use CartId instead of ProductId

        // Confirm removal with Sweet Alert
        Swal.fire({
            icon: 'question',
            title: 'Confirmation',
            text: 'Are you sure you want to remove this item from the cart?',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, remove it!'
        }).then((result) => {
            if (result.isConfirmed) {
                // Proceed with removal
                $.ajax({
                    url: deleteCartUrl,
                    type: 'POST',
                    data: { cartItemId: cartItemId }, // Use cartItemId instead of productId
                    success: function (result) {
                        if (result.success) {
                            Swal.fire({
                                icon: 'success',
                                title: 'Success',
                                text: 'Item removed from cart!'
                            }).then(function () {
                                fetchCartItems(); // Update cart items after removal
                            });
                        } else {
                            Swal.fire({
                                icon: 'error',
                                title: 'Error',
                                text: result.message
                            });
                        }
                    },
                    error: function (error) {
                        console.log(error);
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: 'Error removing item from cart.'
                        });
                    }
                });
            }
        });
    });

    // Handle adding product to cart
    $(document).on('click', '.add-to-cart', function (e) {
        e.preventDefault();
        var productId = $(this).data('product-id');
        var addToCartUrl = "/Cart/AddToCart";

        $.ajax({
            url: addToCartUrl,
            type: 'POST',
            data: { productId: productId },
            success: function (result) {
                if (result.success) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Success',
                        text: 'Product added to cart!'
                    }).then(function () {
                        fetchCartItems(); // Update cart items after adding to cart
                    });
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: result.message
                    });
                }
            },
            error: function (error) {
                console.log(error);
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'Error adding the product to cart.'
                });
            }
        });
    });
});

// Function to fetch cart items
function fetchCartItems() {
    $.ajax({
        url: '/Cart/GetCartItems',
        type: 'GET',
        dataType: 'json',
        success: function (cartItems) {
            // Update cart count
            var totalItems = cartItems.length;
            $('#cart-count').text(totalItems + ' items');

            // Update cart items
            var cartItemsHtml = '';

            cartItems.forEach(function (item) {
                // Convert product images to Base64 strings
                var imageUrls = item.ProductImages.map(function (image) {
                    return 'data:image;base64,' + image;
                });

                // Get the first image URL if available
                var imageUrl = imageUrls.length > 0 ? imageUrls[0] : '';

                cartItemsHtml += `
                    <div class ="row border-top border-bottom">
                        <div class ="row main align-items-center">
                            <div class ="col-2">
                                <img class ="img-fluid" src="${imageUrl}" alt="Product Image">
                            </div>
                            <div class ="col">
                                <div class ="row text-muted">
                                    <div class ="col">${item.ProductName}</div>
                                    <div class ="col">${item.AvailableQuantity}</div>
                                    <div class ="col">${item.StoreId}</div>
                                </div>
                            </div>
                            <div class ="col">
                                <a href="#" class ="quantity-minus">-</a>
                                <span class ="quantity">${item.Quantity}</span>
                                <a href="#" class ="quantity-plus">+</a>
                            </div>
                            <div class ="col product-price" id="product-price-${item.ProductId}" data-price="${item.ProductPrice}">₱ ${item.ProductPrice.toFixed(2)}</div>
                            <div class ="col">
                                <button class ="remove-from-cart-btn btn btn-danger btn-sm" data-cart-item-id="${item.CartId}">&times; </button>
                            </div>
                        </div>
                    </div>`;
            });

            $('#cart-items').html(cartItemsHtml);

            // Update summary section
            updateSummary();


        },
        error: function (error) {
            console.log(error);
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Error fetching cart items.'
            });
        }
    });
}

// Function to retrieve cart items
function getCartItems() {
    var cartItems = [];
    $('.main').each(function () {
        var productId = $(this).find('.product-price').attr('id').split('-')[1];
        var productName = $(this).find('.row.text-muted').eq(0).text().trim();
        var quantity = parseInt($(this).find('.quantity').text().trim());
        var productPrice = parseFloat($(this).find('.product-price').data('price'));
        var cartId = $(this).find('.remove-from-cart-btn').data('cart-item-id');
        var storeId = parseInt($(this).find('.row.text-muted').eq(2).text().trim()); // Get StoreId from HTML
        var availableQuantity = parseInt($(this).find('.row.text-muted .col').eq(1).text().trim()); // Get AvailableQuantity from HTML
        cartItems.push({
            ProductId: productId,
            ProductName: productName,
            Quantity: quantity,
            ProductPrice: productPrice,
            CartId: cartId,
            StoreId: storeId, // Include StoreId in cart item
            AvailableQuantity: availableQuantity // Include AvailableQuantity in cart item
        });
    });
    return cartItems;
}
