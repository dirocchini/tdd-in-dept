using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sales.Application.Commands;
using Xunit;

namespace Application.Tests.Orders
{
    public class AddOrderItemTests
    {
        [Fact(DisplayName = "Add Order Item Valid Command")]
        [Trait("Category", "Sales - Order Commands")]
        public void AddOrderItemCommand_ValidCommand_ShouldBeValid()
        {
            // Arrange
            var command = new AddOrderItemCommand(Guid.NewGuid(), Guid.NewGuid(), "order item x", 2, 100);

            // Act
            var result = command.IsValid();

            // Assert
            Assert.True(result);
        }

        [Fact(DisplayName = "Add Order Item Invalid Command")]
        [Trait("Category", "Sales - Order Commands")]
        public void AddOrderItemCommand_InvalidCommand_ShouldBeValid()
        {
            // Arrange
            var command = new AddOrderItemCommand(Guid.Empty, Guid.Empty, "", 0, 0);

            // Act
            var result = command.IsValid();

            // Assert
            Assert.False(result);
            Assert.Contains(AddOrderItemCommandValidation.IdClienteErroMsg, command.ValidationResult.Errors.Select(c => c.ErrorMessage));
            Assert.Contains(AddOrderItemCommandValidation.IdProdutoErroMsg, command.ValidationResult.Errors.Select(c => c.ErrorMessage));
            Assert.Contains(AddOrderItemCommandValidation.NomeErroMsg, command.ValidationResult.Errors.Select(c => c.ErrorMessage));
            Assert.Contains(AddOrderItemCommandValidation.QtdMinErroMsg, command.ValidationResult.Errors.Select(c => c.ErrorMessage));
            Assert.Contains(AddOrderItemCommandValidation.ValorErroMsg, command.ValidationResult.Errors.Select(c => c.ErrorMessage));

        }
    }
}
