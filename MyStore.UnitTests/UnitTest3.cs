using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyStore.Domain.Abstract;
using MyStore.Domain.Entities;
using MyStore.WebUI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MyStore.UnitTests
{
    [TestClass]
    public class AdminTests
    {
        [TestMethod]
        public void Index_Contains_All_Product()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new List<Product>
            {
                new Product {ProductId = 1, Name = "Игра1" },
                new Product {ProductId = 2, Name = "Игра2" },
                new Product {ProductId = 3, Name = "Игра3" },
                new Product {ProductId = 4, Name = "Игра4" },
                new Product {ProductId = 5, Name = "Игра5" }
            });

            AdminController controller = new AdminController(mock.Object);

            List<Product> result = ((IEnumerable<Product>)controller.Index().Model).ToList();

            Assert.AreEqual(result.Count(), 5);
            Assert.AreEqual("Игра1", result[0].Name);
            Assert.AreEqual("Игра2", result[1].Name);
            Assert.AreEqual("Игра3", result[2].Name);
        }

        [TestMethod]
        public void Can_Edit_Product()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new List<Product>
            {
                new Product {ProductId = 1, Name = "Игра1" },
                new Product {ProductId = 2, Name = "Игра2" },
                new Product {ProductId = 3, Name = "Игра3" },
                new Product {ProductId = 4, Name = "Игра4" },
                new Product {ProductId = 5, Name = "Игра5" }
            });

            AdminController controller = new AdminController(mock.Object);

            Product prod1 = controller.Edit(1).ViewData.Model as Product;
            Product prod2 = controller.Edit(2).ViewData.Model as Product;
            Product prod3 = controller.Edit(3).ViewData.Model as Product;


            Assert.AreEqual(1, prod1.ProductId);
            Assert.AreEqual(2, prod2.ProductId);
            Assert.AreEqual(3, prod3.ProductId);
        }

        [TestMethod]
        public void Cannot_Edit_Nonexistent_Game()
        {
            // Организация - создание имитированного хранилища данных
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new List<Product>
            {
                new Product {ProductId = 1, Name = "Игра1" },
                new Product {ProductId = 2, Name = "Игра2" },
                new Product {ProductId = 3, Name = "Игра3" },
                new Product {ProductId = 4, Name = "Игра4" },
                new Product {ProductId = 5, Name = "Игра5" }
            });

            // Организация - создание контроллера
            AdminController controller = new AdminController(mock.Object);

            // Действие
            Product result = controller.Edit(6).ViewData.Model as Product;

            // Assert
            Assert.AreEqual(result, null);
        }

        [TestMethod]
        public void Can_Save_Valid_Changes()
        {
            // Организация - создание имитированного хранилища данных
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            // Организация - создание контроллера
            AdminController controller = new AdminController(mock.Object);

            // Организация - создание объекта Game
            Product game = new Product { Name = "Test" };

            // Действие - попытка сохранения товара
            ActionResult result = controller.Edit(game);

            // Утверждение - проверка того, что к хранилищу производится обращение
            mock.Verify(m => m.SaveProduct(game));

            // Утверждение - проверка типа результата метода
            Assert.IsNotInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Cannot_Save_Invalid_Changes()
        {
            // Организация - создание имитированного хранилища данных
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            // Организация - создание контроллера
            AdminController controller = new AdminController(mock.Object);

            // Организация - создание объекта Game
            Product game = new Product { Name = "Test" };

            // Организация - добавление ошибки в состояние модели
            controller.ModelState.AddModelError("error", "error");

            // Действие - попытка сохранения товара
            ActionResult result = controller.Edit(game);

            // Утверждение - проверка того, что обращение к хранилищу НЕ производится 
            mock.Verify(m => m.SaveProduct(It.IsAny<Product>()), Times.Never());

            // Утверждение - проверка типа результата метода
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Can_Delete_Valid_Games()
        {
            // Организация - создание объекта Game
            Product game = new Product { ProductId = 2, Name = "Игра2" };

            // Организация - создание имитированного хранилища данных
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new List<Product>
            {
                new Product { ProductId = 1, Name = "Игра1"},
                new Product { ProductId = 2, Name = "Игра2"},
                new Product { ProductId = 3, Name = "Игра3"},
                new Product { ProductId = 4, Name = "Игра4"},
                new Product { ProductId = 5, Name = "Игра5"}
            });

            // Организация - создание контроллера
            AdminController controller = new AdminController(mock.Object);

            // Действие - удаление игры
            controller.Delete(game.ProductId);

            // Утверждение - проверка того, что метод удаления в хранилище
            // вызывается для корректного объекта Game
            mock.Verify(m => m.DeleteProduct(game.ProductId));
        }
    }
}
