using Microsoft.AspNetCore.Mvc;
using Moq;
using SportsStore.Controllers;
using SportsStore.Models;
using Xunit;

namespace SportsStore.Tests {

    public class OrderControllerTests {

        [Fact]
        public void Cannot_Checkout_Empty_Cart() {
            // Create a mock repository
            Mock<IOrderRepository> mock = new Mock<IOrderRepository>();
            // Create an empty cart
            Cart cart = new Cart();
            // Create the order
            Order order = new Order();
            // Create an instance of the controller
            OrderController target = new OrderController(mock.Object, cart);

            ViewResult result = target.Checkout(order) as ViewResult;

            // Check that the order hasn't been stored 
            mock.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Never);
            // Check that the method is returning the default view
            Assert.True(string.IsNullOrEmpty(result.ViewName));
            // Check that I am passing an invalid model to the view
            Assert.False(result.ViewData.ModelState.IsValid);
        }

        [Fact]
        public void Cannot_Checkout_Invalid_ShippingDetails() {

            // Create a mock order repository
            Mock<IOrderRepository> mock = new Mock<IOrderRepository>();
            // Create a cart with one item
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);
            // Create an instance of the controller
            OrderController target = new OrderController(mock.Object, cart);
            // Add an error to the model
            target.ModelState.AddModelError("error", "error");

            // Try to checkout
            ViewResult result = target.Checkout(new Order()) as ViewResult;

            // Check that the order hasn't been passed stored
            mock.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Never);
            // Check that the method is returning the default view
            Assert.True(string.IsNullOrEmpty(result.ViewName));
            // Check that I am passing an invalid model to the view
            Assert.False(result.ViewData.ModelState.IsValid);
        }

        [Fact]
        public void Can_Checkout_And_Submit_Order() {
            // Create a mock order repository
            Mock<IOrderRepository> mock = new Mock<IOrderRepository>();
            // Create a cart with one item
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);
            // Create an instance of the controller
            OrderController target = new OrderController(mock.Object, cart);

            // Try to checkout
            RedirectToActionResult result =
                 target.Checkout(new Order()) as RedirectToActionResult;

            // Check that the order has been stored
            mock.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Once);
            // Check that the method is redirecting to the Completed action
            Assert.Equal("Completed", result.ActionName);
        }

    }
}
