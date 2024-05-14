$(document).ready(function () {
    // Check if there's a Sweet Alert message and display it
    if (sweetAlertMessage !== '') {
        Swal.fire({
            icon: sweetAlertIcon,
            title: sweetAlertTitle,
            text: sweetAlertMessage
        });
    }
});

$(document).ready(function () {
    $(".signin-form").submit(function (e) {
        e.preventDefault(); // Prevent the form from submitting normally

        // Get form data
        var formData = $(this).serialize();

        // Submit form data using AJAX
        $.ajax({
            type: "POST",
            url: $(this).attr("action"),
            data: formData,
            success: function (response) {
                // Check the response from the server
                if (response.success) {
                    // If login is successful, redirect to the home page
                    window.location.href = "/Home/Index";
                } else {
                    // If login fails, show an error message using Sweet Alert
                    Swal.fire({
                        icon: 'error',
                        title: response.title,
                        text: response.message
                    });
                }
            },
            error: function (xhr, status, error) {
                // If an error occurs during AJAX request, show an error message using Sweet Alert
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'An error occurred while processing your request. Please try again later.'
                });
            }
        });
    });
});
