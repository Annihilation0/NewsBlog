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
    let value = category;
    let categories = document.getElementsByClassName("blog-category");
    $.ajax({
        type: "POST",
        // You can use the absolute url eg www.site.com/MyControllerName/LiveTagSearch or the relative path live below  
        url: "/News/SearchByCategoryNews",
        // Attach the value to a parameter called search
        data: { category: value },
        datatype: "html",
        success: function (data) {
            
            for (i = 0; i < categories.length; i++) {
                if (categories.item(i).innerHTML == category)
                    categories.item(i).classList.add('blog-category-selected')
            }
            // Insert the returned search results html into the result element 
            $('#searchNewsResult').html(data);
        }
    });
    
    
}
function RegistrationUser() {
    let userName = document.getElementById("userName").value;
    let firstName = document.getElementById("firstName").value;
    let lastName = document.getElementById("lastName").value;
    let password = document.getElementById("password").value;
    $.ajax({
        type: "POST",
        // You can use the absolute url eg www.site.com/MyControllerName/LiveTagSearch or the relative path live below  
        url: "/Registration/RegistrationUser",
        // Attach the value to a parameter called search
        data: {
            userName,
            firstName,
            lastName,
            password
        },
        datatype: "html",
        success: function (data) {
            // Insert the returned search results html into the result element 
            $('#searchNewsResult').html(data);
        }
    });
}
function LoginUser() {
    let userName = document.getElementById("userName").value;
    let password = document.getElementById("password").value;
    $.ajax({
        type: "POST",
        // You can use the absolute url eg www.site.com/MyControllerName/LiveTagSearch or the relative path live below  
        url: "/Login/Login",
        // Attach the value to a parameter called search
        data: {
            userName,
            password
        },
        datatype: "html",
        success: function (data) {
            // Insert the returned search results html into the result element 
            $('#searchNewsResult').html(data);
        }
    });
}
function AddComment(newsId) {
    let commentText = document.getElementById("commentText").value;
    $.ajax({
        type: "POST",
        // You can use the absolute url eg www.site.com/MyControllerName/LiveTagSearch or the relative path live below  
        url: "/News/AddComment",
        // Attach the value to a parameter called search
        data: {
            commentText,
            newsId
        },
        datatype: "html",
        success: function (data) {
            // Insert the returned search results html into the result element 
            $('#searchNewsResult').html(data);
        }
    });
}
function AddNews() {
    let newsTitle = document.getElementById("newsTitle").value;
    let newsText = document.getElementById("newsText").value;
    let path = document.getElementById("newsImage").value.replace("C:\\fakepath\\", "/css/Resources/");
    $.ajax({
        type: "POST",
        // You can use the absolute url eg www.site.com/MyControllerName/LiveTagSearch or the relative path live below  
        url: "/News/AddNews",
        // Attach the value to a parameter called search
        data: {
            newsTitle,
            newsText,
            path
        },
        datatype: "html",
        success: function (data) {
            // Insert the returned search results html into the result element 
            $('#searchNewsResult').html(data);
        }
    });
}