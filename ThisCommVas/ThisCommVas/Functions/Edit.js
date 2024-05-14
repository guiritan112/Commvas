// edit.js

// Display Sweet Alert notification
function displaySweetAlert(icon, title, text) {
    Swal.fire({
        icon: icon,
        title: title,
        text: text
    });
}
    $(document).ready(function () {
        $('.edit-product-btn').click(function () {
            var productId = $(this).data('product-id');
            var editUrl = $(this).data('edit-url');

            // Send an AJAX request
            $.ajax({
                type: 'GET',
                url: editUrl,
                data: { productId: productId },
                success: function (response) {
                    // Handle the response, for example, you can display the edit form in a modal
                    $('#edit-modal-body').html(response);
                    $('#edit-modal').modal('show');
                },
                error: function (xhr, status, error) {
                    // Handle any errors
                    console.error(xhr.responseText);
                }
            });
        });
    });
