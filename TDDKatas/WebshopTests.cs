using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using Xunit;

namespace TDDKatas
{
    public class WebshopTests
    {
        [Theory]
        [InlineData("Laptop", 5.33, "Laptop 5,33")]
        [InlineData("Book", 100.55, "Book 100,55")]
        public void TestCreateSimpleProduct(string name, decimal price, string expected)
        {
            var product = new Product(name, price);
            Assert.Equal(expected, product.ToString());
        }

        [Theory]
        [InlineData("Book", 100.55, 2, "2x Book 201,10")]
        [InlineData("Laptop", 20.40, 10, "10x Laptop 204,0")]
        public void TestCreateOrderItem(string productName, decimal price, int quantity, string expected)
        {

            var product = new Product(productName, price);
            var orderItem = new OrderItem(product, quantity);
            Assert.Equal(expected, orderItem.ToString());
        }

        [Fact]
        public void TestCreateOrderItemNegativeQuantityThrowsException()
        {

            var product = new Product("Tea", 123.22M);
            Assert.Throws<ArgumentException>(() => new OrderItem(product, -1));
        }

        [Fact]
        public void TestCreateOrderItemZeroQuantityThrowsException()
        {
            var product = new Product("Tea", 123.22M);
            Assert.Throws<ArgumentException>(() => new OrderItem(product, 0));
        }

        //Currency formatting
        //Product equals
        //Order string presentation
        //Product with negative price?
        //Add Product to order
        //Remove product from order
        //Add same product twice adds together quantity
    }

    public class OrderItem
    {
        public Product Product { get;  }
        public int Quantity { get; set; }
        public decimal Subtotal => Product.Price * Quantity;

        public OrderItem(Product product, int quantity)
        {
            if (quantity <= 0) throw new ArgumentException();
            Product = product;
            Quantity = quantity;
        }

        public override string ToString()
        {
            return $"{Quantity}x {Product.Name} {Subtotal}";
        }

    }

    public class Product
    {
        public Product(string name, decimal price)
        {
            Name = name;
            Price = price;
        }

        public string Name { get; }
        public decimal Price { get; }

        public override string ToString()
        {
            return $"{Name} {Price}";
        }
    }
}