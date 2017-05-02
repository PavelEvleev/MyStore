using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyStore.Domain.Entities;
using Moq;
using MyStore.Domain.Abstract;
using System.Collections.Generic;
using MyStore.WebUI.Controllers;
using System.Web.Mvc;
using System.Linq;

namespace MyStore.UnitTests
{
    [TestClass]
    public class ImageTest
    {
        [TestMethod]
        public void Can_Retrieve_Image_Data()
        {
            // Организация - создание объекта Game с данными изображения
            Product game = new Product
            {
                ProductId = 2,
                Name = "Игра2",
                ImageData = new byte[] { },
                ImageMimeType = "image/png"
            };

            // Организация - создание имитированного хранилища
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new List<Product> {
                new Product {ProductId = 1, Name = "Игра1"},
                game,
                new Product {ProductId = 3, Name = "Игра3"}
            }.AsQueryable());

            // Организация - создание контроллера
            ProductController controller = new ProductController(mock.Object);

            // Действие - вызов метода действия GetImage()
            ActionResult result = controller.GetImage(2);

            // Утверждение
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(FileResult));
            Assert.AreEqual(game.ImageMimeType, ((FileResult)result).ContentType);
        }

        [TestMethod]
        public void Cannot_Retrieve_Image_Data_For_Invalid_ID()
        {
            // Организация - создание имитированного хранилища
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new List<Product> {
                new Product {ProductId = 1, Name = "Игра1"},
                new Product {ProductId = 2, Name = "Игра2"}
            }.AsQueryable());

            // Организация - создание контроллера
            ProductController controller = new ProductController(mock.Object);

            // Действие - вызов метода действия GetImage()
            ActionResult result = controller.GetImage(10);

            // Утверждение
            Assert.IsNull(result);
        }
    }
}
