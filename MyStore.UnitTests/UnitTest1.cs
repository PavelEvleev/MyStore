using System;
using Moq;
using MyStore.Domain.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyStore.Domain.Entities;
using System.Collections.Generic;
using MyStore.WebUI.Controllers;
using System.Linq;
using System.Web.Mvc;
using MyStore.WebUI.Models;
using MyStore.WebUI.HtmlHelpers;

namespace MyStore.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_Paginate()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new List<Product>
            {
                new Product { ProductId = 1, Name = "Игра1" },
                new Product { ProductId = 2, Name = "Игра2" },
                new Product { ProductId = 3, Name = "Игра3" },
                new Product { ProductId = 4, Name = "Игра4" },
                new Product { ProductId = 5, Name = "Игра5" },
            });

            ProductController controller = new ProductController(mock.Object);
            controller.pageSize = 3;

            ProductListViewModel result = (ProductListViewModel)controller.List(null, 2).Model;

            List<Product> products = result.Products.ToList();

            Assert.IsTrue(products.Count == 2);
            Assert.AreEqual(products[0].Name, "Игра4");
            Assert.AreEqual(products[1].Name, "Игра5");
        }

        [TestMethod]
        public void Can_Generate_Page_Links()
        {
            HtmlHelper myHelper = null;

            PagingInfo pagingInfo = new PagingInfo
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };
            Func<int, string> pageUrlDelegate = i => "Page" + i;

            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);
            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Page1"">1</a>"
                + @"<a class=""btn btn-default btn-primary selected"" href=""Page2"">2</a>"
                + @"<a class=""btn btn-default"" href=""Page3"">3</a>", result.ToString());
        }
        [TestMethod]
        public void Can_Send_Paginftion_View_Model()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new List<Product>
            {
                new Product {ProductId = 1,Name = "Игра1" },
                new Product {ProductId = 2,Name = "Игра2" },
                new Product {ProductId = 3,Name = "Игра3" },
                new Product {ProductId = 4,Name = "Игра4" },
                new Product {ProductId = 5,Name = "Игра5" }
            });

            ProductController controller = new ProductController(mock.Object);
            controller.pageSize = 3;

            ProductListViewModel result = (ProductListViewModel)controller.List(null, 2).Model;

            PagingInfo pageInfo = result.PagingInfo;

            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemsPerPage, 3);
            Assert.AreEqual(pageInfo.TotalItems, 5);
            Assert.AreEqual(pageInfo.TotalPages, 2);
        }
        [TestMethod]
        public void Can_Filter_Product()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new List<Product>
            {
                new Product { ProductId = 1, Name = "Игра1", Category="Cat1"},
                new Product { ProductId = 2, Name = "Игра2", Category="Cat2"},
                new Product { ProductId = 3, Name = "Игра3", Category="Cat1"},
                new Product { ProductId = 4, Name = "Игра4", Category="Cat2"},
                new Product { ProductId = 5, Name = "Игра5", Category="Cat3"}
            });
            ProductController controller = new ProductController(mock.Object);
            controller.pageSize = 3;

            List<Product> result = ((ProductListViewModel)controller.List("Cat2", 1).Model).Products.ToList();

            Assert.AreEqual(result.Count(), 2);
            Assert.IsTrue(result[0].Name == "Игра2" && result[0].Category == "Cat2");
            Assert.IsTrue(result[1].Name == "Игра4" && result[0].Category == "Cat2");

        }

        [TestMethod]
        public void Can_Create_Categories()
        {
            // Организация - создание имитированного хранилища
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new List<Product> {
                new Product { ProductId = 1, Name = "Игра1", Category="Симулятор"},
                new Product { ProductId = 2, Name = "Игра2", Category="Симулятор"},
                new Product { ProductId = 3, Name = "Игра3", Category="Шутер"},
                new Product { ProductId = 4, Name = "Игра4", Category="RPG"},
            });

            // Организация - создание контроллера
            NavController target = new NavController(mock.Object);

            // Действие - получение набора категорий
            List<string> results = ((IEnumerable<string>)target.Menu().Model).ToList();

            // Утверждение
            Assert.AreEqual(results.Count(), 3);
            Assert.AreEqual(results[0], "RPG");
            Assert.AreEqual(results[1], "Симулятор");
            Assert.AreEqual(results[2], "Шутер");
        }

        [TestMethod]
        public void Indicates_Selected_Category()
        {
            // Организация - создание имитированного хранилища
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product { ProductId = 1, Name = "Игра1", Category="Симулятор"},
                new Product { ProductId = 2, Name = "Игра2", Category="Шутер"}
            });

            // Организация - создание контроллера
            NavController target = new NavController(mock.Object);

            // Организация - определение выбранной категории
            string categoryToSelect = "Шутер";

            // Действие
            string result = target.Menu(categoryToSelect).ViewBag.SelectedCategory;

            // Утверждение
            Assert.AreEqual(categoryToSelect, result);
        }

        [TestMethod]
        public void Generate_Category_Specific_Game_Count()
        {
            /// Организация (arrange)
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new List<Product>
            {
                new Product { ProductId = 1, Name = "Игра1", Category="Cat1"},
                new Product { ProductId = 2, Name = "Игра2", Category="Cat2"},
                new Product { ProductId = 3, Name = "Игра3", Category="Cat1"},
                new Product { ProductId = 4, Name = "Игра4", Category="Cat2"},
                new Product { ProductId = 5, Name = "Игра5", Category="Cat3"}
            });
            ProductController controller = new ProductController(mock.Object);
            controller.pageSize = 3;

            // Действие - тестирование счетчиков товаров для различных категорий
            int res1 = ((ProductListViewModel)controller.List("Cat1").Model).PagingInfo.TotalItems;
            int res2 = ((ProductListViewModel)controller.List("Cat2").Model).PagingInfo.TotalItems;
            int res3 = ((ProductListViewModel)controller.List("Cat3").Model).PagingInfo.TotalItems;
            int resAll = ((ProductListViewModel)controller.List(null).Model).PagingInfo.TotalItems;

            // Утверждение
            Assert.AreEqual(res1, 2);
            Assert.AreEqual(res2, 2);
            Assert.AreEqual(res3, 1);
            Assert.AreEqual(resAll, 5);
        }


    }
}
