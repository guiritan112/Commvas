$(document).ready(function () {
    // Function to handle form submission
    $("#product-search-form").submit(function (event) {
        // Prevent default form submission
        event.preventDefault();

        // Get the search term
        var searchTerm = $("#product-search-input").val();

        // Check if the search term is empty
        if (searchTerm.trim() === "") {
            // If search term is empty, reload the page to show all products
            window.location.href = indexUrl;
        } else {
            // If search term is not empty, submit the form via AJAX
            $.ajax({
                url: searchUrl,
                type: "POST",
                data: { searchTerm: searchTerm },
                success: function (data) {
                    // Replace the content of #image-grid with the search results
                    $("#image-grid").html(data);
                },
                error: function () {
                    // Handle error
                    console.log("Error occurred while searching.");
                }
            });
        }
    });
});
