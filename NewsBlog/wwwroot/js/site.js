// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
let timeout = null;
document.getElementById('searchNews').addEventListener('keyup', function (e) {
    // Clear existing timeout      
    clearTimeout(timeout);

    // Reset the timeout to start again
    timeout = setTimeout(function () {
        LiveSearch()
    }, 800);
});

function LiveSearch() {
    //Get the input value
    let value = document.getElementById('searchNews').value

    $.ajax({
        type: "POST",
        // You can use the absolute url eg www.site.com/MyControllerName/LiveTagSearch or the relative path live below  
        url: "/News/SearchNews",
        // Attach the value to a parameter called search
        data: { search: value },
        datatype: "html",
        success: function (data) {
            // Insert the returned search results html into the result element 
            $('#searchNewsResult').html(data);
        }
    });
}