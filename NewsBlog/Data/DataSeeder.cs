using NewsBlog.Models;

namespace NewsBlog.NewsBlogData
{
    public static class DataSeeder
    {
        /*--------------------------------------
        Инициализации базы данных
        --------------------------------------*/
        public static void Seed(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<DbContext>();

            context.Database.EnsureCreated();
            AddRoles(context);
            AddCategories(context);
            AddNews(context);
            AddComments(context);
        }
        /*--------------------------------------
Заполнение базы данных 
--------------------------------------*/
        private static void AddRoles(DbContext context)
        {
            var roles = context.Roles.FirstOrDefault();

            if (roles != null)
            { return; }

            context.Roles.Add(new Role
            {
                RoleName = "User",
            });

            context.Roles.Add(new Role
            {
                RoleName = "SuperUser",
            });

            context.SaveChanges();
        }
        private static void AddCategories(DbContext context)
        {
            var categories = context.Categories.FirstOrDefault();

            if (categories != null)
            { return; }

            context.Categories.Add(new Category
            {
                CategoryName = "ООП",
            });
            context.Categories.Add(new Category
            {
                CategoryName = "Книги",
            });
            context.Categories.Add(new Category
            {
                CategoryName = "Программирование",
            });
            context.Categories.Add(new Category
            {
                CategoryName = "С++",
            });
            context.Categories.Add(new Category
            {
                CategoryName = "Обучение",
            });
            context.SaveChanges();
        }
        private static void AddNews(DbContext context)
        {          
            var news = context.News.FirstOrDefault();
            int saltSize = 16;
            if (news != null)
            { return; }

            context.News.Add(new News
            {
                Title = "Как программировать на C++ - Х. М. Дейтел, П. Дж. Дейтел",
                Content = "Книга является одним из самых популярных и мире учебников по С++.Характерной ее особенностью является \"раннее введение\" в классы и объекты, т. е. начала объектно-ориентированного программирования вводятся уже в 3-й главе, без предварительного изложения унаследованных от языка С элементов процедурного и структурного программирования, как это делается в большинстве курсов по С++. Большое внимание уделяется объектно-ориентированному проектированию (ООD) программных систем с помощью графического языка UML2, чему посвящен ряд факультативных разделов, описывающих последовательную разработку большого учебного проекта.\r\nВ текст книги включена масса примеров \"живого кода\" - подробно комментированных работающих программ с образцами их запуска, а также несколько подробно разбираемых интересных примеров. В конце каждой главы имеется обширный набор контрольных вопросов и упражнений.\r\nКнига может служить учебным пособием для начальных курсов но С++, а также будет полезна широкому кругу как начинающих программистов, так и более опытных, не работавших прежде с С++. Это и многое другое вы найдете в книге Как программировать на C++ (Х. М. Дейтел, П. Дж. Дейтел). Напишите свою рецензию о книге Х. М. Дейтел, П. Дж. Дейтел «Как программировать на C++»\r\nhttp://izbe.ru/book/8196-kak-programmirovat-na-c-h-m-deytel-p-dzh-deytel/",

                Published = DateTime.UtcNow,
                Author = new User
                {
                    UserName = "userName_1",
                    FirstName = "Обсуждаем книги и слова",
                    PasswordHash = PasswordHashing.GetHashString("password1!"),
                    PasswordSalt = PasswordSaltGenerator.GenerateSalt(saltSize),
                    Role = context.Roles.Where(role => role.RoleName == "User").First(),
                },
                Categories = new List<Category>
                {
                    context.Categories.Where(category => category.CategoryName == "Книги").First(),
                    context.Categories.Where(category => category.CategoryName == "С++").First(),
                    context.Categories.Where(category => category.CategoryName == "Программирование").First()
                },
                ResourcePath = "/css/Resources/1.jpg"
            });

            context.News.Add(new News
            {
                Title = "Основы программирования",
                Content = "Может быть вам и не нужно знать о тех вещах, о которых речь пойдет ниже для того, чтобы начать изучать программирование для чайников. Однако для того, чтобы лучше понимать основы программирования и разбираться, что происходит за занавесом, лучше прочитать.\r\n\r\nЕсли вы собираетесь написать полноценную программу на таком языке как Visual Basic, C, C++ или Java, вы пишете, что называется на языке высокого уровня. Это язык, который создан, чтобы читать в легко понимаемом и удобно отформатированном виде, хотя это может и не показаться таковым первых порах для увлеченных Программирование для начинающих! При компиляции программы компилятор сначала проверяет, чтобы убедиться, что программа написана в соответствии со структурой и правилами языка. После т, ваша программа переводится в машинный код, который можно прочитать на компьютере. По сути, машинный код, то, что вы, возможно, видели, а называлось двоичным кодом: \"00101101\". Все, что вы напишете в программе переводится до базового уровня - наборы из нулей и единиц, что может быть понято до конца машиной (компьютером) - это основа программирования.\r\n\r\nЕсли вы пишете на веб-скриптовом языке, который пользуется популярностью при веб-программировании, как HTML или PHP, процесс немного отличается. В конечном счете, вся программа по-прежнему разбивается на машинный код, так что процессор может его интерпретировать, не смотря на то, что вы создаете сценарий, а не программу, составленную на полноценном языке программирования. Скрипт запускается через специальную программу , которая называется транслятором , а результат выводится на экрана браузера. Такие сценарии (наборы команд) компилируются только по запросу браузера..\r\n\r\nВещи, которые вам не нужно знать на данный момент , но вы можете посчитать их интересными.\r\n\r\nЕсть два основных процесса проектирования программ используемых сегодня: Функциональный (алгоритмический) и объектно-ориентированный:\r\n\r\nФункциональный дизайн программы был стандартным на протяжении многих лет, и даже сегодня многие вещи можно выполнить с помощью простого функционального подхода. Функциональный подход можно рассматривать таким образом: Хорошо, у меня есть данные; делаю шаг 1; ладно, теперь делаю шаг 2; теперь я должен ли я перейти к шагу 3 или 4?; ладно, переходим к шагу 4. Снова и снова, пока выполнение программы будет завершено. Звучит логично, да? Я думаю так легче всего понять что такое программирование.\r\n\r\nФункциональному программированию противопоставляют объектно-ориентированное программирование (ООП), оно считается новой парадигмой в разработке программ. C++ и Java являются наиболее ориентированными на ООП языками, хотя вы все еще можете использовать их в качестве функциональных языков программирования. Я не думаю, что изучение объектно-ориентированного дизайна должно стать приоритетной задачей для изучающих программирование для начинающих. Этот сайт предназначен для вас, тех, кто хочет идти без путаницы. ООП, откровенно говоря, довольно быстро запутывает новичка. После того как новичок понимает функциональное программирование и как оно работает, то, возможно, может начинать изучать ООП и пользоваться его благами. Откровенно говоря, большинству людей никогда не понадобится ООП, чтобы выполнить то, что они хотят. Если у вас есть любознательность, или карьерная необходимость, вы будете держать курс на объектно-ориентированного программирования, в противном случае он будет просто затягягивать появление ваши успехов в написании рабочих программ, которые станут полезными для вас как начинающего в программировании. Если так можно выразиться, то ООП (при хорошем изучении и использовании) позволяет разрабатывать более «элегантный» дизайн и увеличивать полезность своей программы для программистов (не для новичков!). Когда вы начнете переходить к написанию более сложных программ , то вероятно на вашем пути появится ООП. Независимо от того, что скажут снобы или прошедшие путь программирования, можно утверждать, что хорошую карьеру программиста можно сделать, не касаясь темы объектно-ориентированного программирования.",
                Published = DateTime.UtcNow,
                Author = new User
                {
                    UserName = "userName_2",
                    FirstName = "Записки программиста",
                    PasswordHash = PasswordHashing.GetHashString("password1!"),
                    PasswordSalt = PasswordSaltGenerator.GenerateSalt(saltSize),
                    Role = context.Roles.Where(role => role.RoleName == "User").First(),
                },
                Categories = new List<Category>
                {
                    context.Categories.Where(category => category.CategoryName == "Обучение").First(),
                    context.Categories.Where(category => category.CategoryName == "ООП").First(),
                    context.Categories.Where(category => category.CategoryName == "Программирование").First()
                },
                ResourcePath = "/css/Resources/2.png"
            });

            context.SaveChanges();
        }
        private static void AddComments(DbContext context)
        {
           /* var comments = context.Comments.FirstOrDefault();
            int saltSize = 16;
            if (comments != null)
            { return; }

            context.Comments.Add(new Comment
            {                
                Published = DateTime.UtcNow,
                Author = new User
                {
                    UserName = "userName_3",
                    FirstName = "Иван",
                    PasswordHash = PasswordHashing.GetHashString("password1!"),
                    PasswordSalt = PasswordSaltGenerator.GenerateSalt(saltSize),
                    Role = context.Roles.Where(role => role.RoleName == "User").First(),
                },
                News = context.News.Where(news => news.NewsId == 2).First(),
                Content = "Интересная новость! Хотелось бы увидеть больше ссылок на доп материал"
            });

            context.Comments.Add(new Comment
            {
                Published = DateTime.UtcNow,
                Author = new User
                {
                    UserName = "userName_4",
                    FirstName = "Елена",
                    LastName = "Петрова",
                    PasswordHash = PasswordHashing.GetHashString("password1!"),
                    PasswordSalt = PasswordSaltGenerator.GenerateSalt(saltSize),
                    Role = context.Roles.Where(role => role.RoleName == "User").First(),
                },
                News = context.News.Where(news => news.NewsId == 2).First(),
                Content = "Хочется еще статей на эту тему"
            });
           
            context.SaveChanges();
           */
        }
    }
}
