using Core.DomainObjects;
using Sales.Domain;
using Sales.Domain.VoucherEntities;
using System;
using System.Linq;
using Xunit;

namespace Domain.Tests
{
    public class OrderTests
    {
        [Fact(DisplayName = "Add Item In New Order")]
        [Trait("Category", "Sales - Order")]
        public void AddItem_NewOrder_ShouldUpdateValue()
        {
            // Arrange
            var order = Order.OrderFactory.NewDraftOrder(Guid.NewGuid());
            var item = new Item(Guid.NewGuid(), "product x", 2, 100.00M);

            // Act
            order.AddItem(item);

            // Assert
            Assert.Equal(200, order.TotalValue);
        }

        [Fact(DisplayName = "Add Item Already In Order")]
        [Trait("Category", "Sales - Order")]
        public void AddItem_OrderWithItem_ShouldAddItemQuantity()
        {
            // Arrange
            var order = Order.OrderFactory.NewDraftOrder(Guid.NewGuid());
            var itemId = Guid.NewGuid();
            var item = new Item(itemId, "product x", 2, 100.00M);
            order.AddItem(item);
            var item2 = new Item(itemId, "product x", 1, 100.00M);

            // Act
            order.AddItem(item2);

            // Assert
            Assert.Equal(300, order.TotalValue);
            Assert.Single(order.Items);
            Assert.Equal(3, order.Items.FirstOrDefault(p => p.Id == itemId)?.Quantity);
        }

        [Fact(DisplayName = "Add Item With Quantity More Than Allowed")]
        [Trait("Category", "Sales - Order")]
        public void AddOrderItem_ItemWithMoreAllowedQuantity_ShouldReturnException()
        {
            // Arrange
            var order = Order.OrderFactory.NewDraftOrder(Guid.NewGuid());
            var itemId = Guid.NewGuid();
            var item = new Item(itemId, "product x", Order.MAX_ITEM_QUANTITY_PER_ITEM + 1, 100.00M);

            // Act & Assert
            Assert.Throws<DomainException>(() => order.AddItem(item));
        }

        [Fact(DisplayName = "Add Item Already In Order With More Quantity Than Allowed")]
        [Trait("Category", "Sales - Order")]
        public void AddItem_OrderWithItemMoreThanAllowedQuantity_ShouldAddItemQuantity()
        {
            // Arrange
            var order = Order.OrderFactory.NewDraftOrder(Guid.NewGuid());
            var itemId = Guid.NewGuid();
            var item = new Item(itemId, "product x", 2, 100.00M);
            order.AddItem(item);
            var item2 = new Item(itemId, "product x", Order.MAX_ITEM_QUANTITY_PER_ITEM, 100.00M);

            // Act & Assert
            Assert.Throws<DomainException>(() => order.AddItem(item2));
        }

        [Fact(DisplayName = "Update Item Not in Order")]
        [Trait("Category", "Sales - Order")]
        public void UpdateOrderItem_ItemNotInOrder_ShouldReturnException()
        {
            // Arrange
            var order = Order.OrderFactory.NewDraftOrder(Guid.NewGuid());
            var item = new Item(Guid.NewGuid(), "product x", 2, 100.00M);

            // Act & Assert
            Assert.Throws<DomainException>(() => order.UpdateItem(item));
        }

        [Fact(DisplayName = "Update Item in Order")]
        [Trait("Category", "Sales - Order")]
        public void UpdateOrderItem_ItemInOrder_ShouldReplaceItem()
        {
            // Arrange
            var order = Order.OrderFactory.NewDraftOrder(Guid.NewGuid());
            var itemId = Guid.NewGuid();
            var item = new Item(itemId, "product x", 2, 100.00M);
            order.AddItem(item);
            var item2 = new Item(itemId, "product x", 4, 100.00M);
            var newQuantity = item2.Quantity;

            // Act 
            order.UpdateItem(item2);

            //Assert
            Assert.Equal(newQuantity, order.Items.FirstOrDefault(i => i.Id == itemId).Quantity);
        }

        [Fact(DisplayName = "Update Item in Order - Recalculate")]
        [Trait("Category", "Sales - Order")]
        public void UpdateOrderItem_ItemInOrder_ShouldRecalculateOrderTotal()
        {
            //Arrange
            var order = Order.OrderFactory.NewDraftOrder(Guid.NewGuid());
            var itemId = Guid.NewGuid();
            var item = new Item(Guid.NewGuid(), "product a", 2, 100.00M);
            var item2 = new Item(itemId, "product x", 4, 120.00M);

            order.AddItem(item);
            order.AddItem(item2);

            var item2Updated = new Item(itemId, "product x", 6, 100.00M);
            decimal totalPedido = item.Quantity * item.Value + item2Updated.Quantity * item2Updated.Value;

            //Act
            order.UpdateItem(item2Updated);

            //Assert
            Assert.Equal(totalPedido, order.TotalValue);
        }

        [Fact(DisplayName = "Update Item in Order - Quantity Should Be Valid")]
        [Trait("Category", "Sales - Order")]
        public void UpdateOrderItem_ItemInOrder_ShouldQuantityBeValid()
        {
            // Arrange
            var order = Order.OrderFactory.NewDraftOrder(Guid.NewGuid());
            var itemId = Guid.NewGuid();
            var item = new Item(itemId, "product x", 2, 100.00M);
            order.AddItem(item);
            var item2 = new Item(itemId, "product x", Order.MAX_ITEM_QUANTITY_PER_ITEM + 1, 100.00M);

            // Act 
            Assert.Throws<DomainException>(() => order.UpdateItem(item2));
        }

        [Fact(DisplayName = "Remove Item Not Order")]
        [Trait("Category", "Sales - Order")]
        public void RemoveOrderItem_ItemNotInOrder_ShouldReturnException()
        {
            // Arrange
            var order = Order.OrderFactory.NewDraftOrder(Guid.NewGuid());
            var itemId = Guid.NewGuid();
            var item = new Item(itemId, "product x", 2, 100.00M);

            // Act 
            Assert.Throws<DomainException>(() => order.RemoveItem(item));
        }

        [Fact(DisplayName = "Remove Item in Order - Recalculate")]
        [Trait("Category", "Sales - Order")]
        public void RemoveOrderItem_ItemInOrder_ShouldRecalculateOrderTotal()
        {
            //Arrange
            var order = Order.OrderFactory.NewDraftOrder(Guid.NewGuid());
            var itemId = Guid.NewGuid();
            var item = new Item(Guid.NewGuid(), "product a", 2, 100.00M);
            var item2 = new Item(itemId, "product x", 4, 120.00M);

            order.AddItem(item);
            order.AddItem(item2);

            var item2ToRemove = new Item(itemId, "product x", 4, 120.00M);
            var totalPedido = item.Quantity * item.Value;

            //Act
            order.RemoveItem(item2ToRemove);

            //Assert
            Assert.Equal(totalPedido, order.TotalValue);
        }

        [Fact(DisplayName = "Apply Valid Voucher")]
        [Trait("Category", "Sales - Voucher")]
        public void Order_ApplyValidVoucher_ShouldApplyDiscount()
        {
            // Arrange
            var order = Order.OrderFactory.NewDraftOrder(Guid.NewGuid());
            var voucher = new Voucher("PROMO 15 DOLLARS", 15, null, 1, DateTime.Now.AddYears(1), true, false, VoucherType.Value);

            // Act
            var result = order.ApplyVoucher(voucher);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact(DisplayName = "Apply Invalid Voucher")]
        [Trait("Category", "Sales - Voucher")]
        public void Order_ApplyInvalidVoucher_ShouldNotApplyDiscount()
        {
            // Arrange
            var order = Order.OrderFactory.NewDraftOrder(Guid.NewGuid());
            var voucher = new Voucher("PROMO 15 DOLLARS", 15, null, 1, DateTime.Now.AddYears(-1), true, false, VoucherType.Value);

            // Act
            var result = order.ApplyVoucher(voucher);

            // Assert
            Assert.False(result.IsValid);
        }


        [Fact(DisplayName = "Apply Valid Voucher - Discount Value From Total")]
        [Trait("Category", "Sales - Voucher")]
        public void Order_ApplyValidVoucher_ShouldDiscountFromTotal()
        {
            // Arrange
            var order = Order.OrderFactory.NewDraftOrder(Guid.NewGuid());
            var item = new Item(Guid.NewGuid(), "product x", 2, 100.00M);
            order.AddItem(item);
            var voucher = new Voucher("PROMO 15 DOLLARS", 15, null, 1, DateTime.Now.AddYears(1), true, false, VoucherType.Value);
            var valueWithDiscount = order.TotalValue - voucher.DiscountValue;

            // Act
            var result = order.ApplyVoucher(voucher);

            // Assert
            Assert.Equal(valueWithDiscount, order.TotalValue);
        }

        [Fact(DisplayName = "Apply Valid Voucher - Discount Percentual From Total")]
        [Trait("Category", "Sales - Voucher")]
        public void Order_ApplyValidVoucher_ShouldDiscountPercentualFromTotal()
        {
            // Arrange
            var order = Order.OrderFactory.NewDraftOrder(Guid.NewGuid());
            var item = new Item(Guid.NewGuid(), "product x", 2, 100.00M);
            order.AddItem(item);
            var voucher = new Voucher("PROMO 15 DOLLARS", null, 10, 1, DateTime.Now.AddYears(1), true, false, VoucherType.Percentage);
            var valueWithDiscount = order.TotalValue - (order.TotalValue * voucher.DiscountPercentual.Value / 100);

            // Act
            var result = order.ApplyVoucher(voucher);

            // Assert
            Assert.Equal(valueWithDiscount, order.TotalValue);
        }

        [Fact(DisplayName = "Validate Valid Voucher - Voucher Bigger Than Order Cost (value type)")]
        [Trait("Category", "Sales - Voucher")]
        public void Order_ApplyValidVoucherBiggerThenTotalValueType_ShouldReturnOrderTotalValueZero()
        {
            // Arrange
            var order = Order.OrderFactory.NewDraftOrder(Guid.NewGuid());
            var item = new Item(Guid.NewGuid(), "product x", 2, 100.00M);
            order.AddItem(item);
            var voucher = new Voucher("PROMO 15 DOLLARS", 500, null, 1, DateTime.Now.AddYears(1), true, false, VoucherType.Value);

            // Act
            var result = order.ApplyVoucher(voucher);

            // Assert
            Assert.Equal(0, order.TotalValue);
        }
        
        [Fact(DisplayName = "Validate Valid Voucher - Voucher Bigger Than Order Cost (percentage type)")]
        [Trait("Category", "Sales - Voucher")]
        public void Order_ApplyValidVoucherBiggerThenTotalPercentageType_ShouldReturnOrderTotalValueZero()
        {
            // Arrange
            var order = Order.OrderFactory.NewDraftOrder(Guid.NewGuid());
            var item = new Item(Guid.NewGuid(), "product x", 2, 100.00M);
            order.AddItem(item);
            var voucher = new Voucher("PROMO 15 DOLLARS", null, 110, 1, DateTime.Now.AddYears(1), true, false, VoucherType.Percentage);

            // Act
            var result = order.ApplyVoucher(voucher);

            // Assert
            Assert.Equal(0, order.TotalValue);
        }

        [Fact(DisplayName = "Validate Valid Voucher - After Several Changes")]
        [Trait("Category", "Sales - Voucher")]
        public void Order_ApplyValidVoucherAfterChanges_ShouldReturnOrderTotalWithDiscount()
        {
            // Arrange
            var order = Order.OrderFactory.NewDraftOrder(Guid.NewGuid());
            var item = new Item(Guid.NewGuid(), "product x", 2, 100.00M);
            order.AddItem(item);
            var voucher = new Voucher("PROMO 15 DOLLARS", 150, null, 1, DateTime.Now.AddYears(1), true, false, VoucherType.Value);
            
            var result = order.ApplyVoucher(voucher);
            var item2 = new Item(Guid.NewGuid(), "product x", 3, 150.00M);
            
            // Act
            order.AddItem(item2);

            // Assert
            var totalOrderValue = order.Items.Sum(i => i.Value * i.Quantity) - voucher.DiscountValue;
            Assert.Equal(totalOrderValue, order.TotalValue);
        }
    }
}