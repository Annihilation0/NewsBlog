/*--------------------------------------
ассинхронный поиск новостей по заголовку
--------------------------------------*/
/*--------------------------------------
Добавление таймаута в событие keyup,
который будет сброшен, если пользователь
наберет другую букву.
--------------------------------------*/
var timeout = null;
document.getElementById('searchNews').addEventListener('keyup', function (e) {
    // Clear existing timeout      
    clearTimeout(timeout);
    // Reset the timeout to start again
    timeout = setTimeout(function () {
        SearchNews()
    }, 800);
});

function SearchNews() {
    /*--------------------------------------
    Передаем строку поиска в контроллер
    --------------------------------------*/
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

//document.getElementById('readNews').addEventListener("click", ReadNews());
function ReadNews(newsId) {

    let value = newsId;
    $.ajax({
        type: "POST",
        // You can use the absolute url eg www.site.com/MyControllerName/LiveTagSearch or the relative path live below  
        url: "/News/ReadNews",
        // Attach the value to a parameter called search
        data: { newsId: value },
        datatype: "html",
        success: function (data) {
            // Insert the returned search results html into the result element 
            $('#searchNewsResult').html(data);
        }
    });
}

function SearchByCategoryNews(category) {
    let categories = document.getElementsByClassName("blog-category")
    for (i = 0; i < categories.length; i++) {
        if (categories.item(i).innerHTML == category)
            categories.item(i).classList.add('blog-category-selected')
    }
    let value = category;
    $.ajax({
        type: "POST",
        // You can use the absolute url eg www.site.com/MyControllerName/LiveTagSearch or the relative path live below  
        url: "/News/SearchByCategoryNews",
        // Attach the value to a parameter called search
        data: { category: value },
        datatype: "html",
        success: function (data) {
            // Insert the returned search results html into the result element 
            $('#searchNewsResult').html(data);
        }
    });
}