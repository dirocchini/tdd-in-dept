using Sales.Domain.VoucherEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Text;
using Xunit;

namespace Domain.Tests
{
    public class VoucherTests
    {
        [Fact(DisplayName = "Validate Valid Voucher - Value Type")]
        [Trait("Category", "Sales - Voucher")]
        public void Voucher_ValidatedVoucherValueType_ShouldBeValid()
        {
            // Arrange
            var voucher = new Voucher("PROMO 15 DAYS", 150, null, 1, DateTime.Now.AddYears(1), true, false, VoucherType.Value);

            // Act
            var result = voucher.Validate();

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact(DisplayName = "Validate Valid Voucher - Value Type Invalid")]
        [Trait("Category", "Sales - Voucher")]
        public void Voucher_ValidatedVoucherValueType_ShouldBeInvalid()
        {
            // Arrange
            var voucher = new Voucher("", null, null, 0, DateTime.Now.AddYears(-1), false, true, VoucherType.Value);

            // Act
            var result = voucher.Validate();

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(6, result.Errors.Count);
            Assert.Contains(AplicableVoucherValidation.CodeErrorMsg, result.Errors.Select(c => c.ErrorMessage));
            Assert.Contains(AplicableVoucherValidation.ValidUntilErrorMsg, result.Errors.Select(c => c.ErrorMessage));
            Assert.Contains(AplicableVoucherValidation.ActiveErrorMsg, result.Errors.Select(c => c.ErrorMessage));
            Assert.Contains(AplicableVoucherValidation.UsedErrorMsg, result.Errors.Select(c => c.ErrorMessage));
            Assert.Contains(AplicableVoucherValidation.QuantityErrorMsg, result.Errors.Select(c => c.ErrorMessage));
            Assert.Contains(AplicableVoucherValidation.DiscountTypeValueErrorMsg, result.Errors.Select(c => c.ErrorMessage));
        }

        [Fact(DisplayName = "Validate Valid Voucher - Percentage Type")]
        [Trait("Category", "Sales - Voucher")]
        public void Voucher_ValidatedVoucherPercentageType_ShouldBeValid()
        {
            // Arrange
            var voucher = new Voucher("PROMO 15 DAYS", null, 15, 1, DateTime.Now.AddYears(1), true, false, VoucherType.Percentage);

            // Act
            var result = voucher.Validate();

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact(DisplayName = "Validate Valid Voucher - Percentage Type Invalid")]
        [Trait("Category", "Sales - Voucher")]
        public void Voucher_ValidatedVoucherPercentageType_ShouldBeInvalid()
        {
            // Arrange
            var voucher = new Voucher("", null, null, 0, DateTime.Now.AddYears(-1), false, true, VoucherType.Percentage);

            // Act
            var result = voucher.Validate();

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(6, result.Errors.Count);
            Assert.Contains(AplicableVoucherValidation.CodeErrorMsg, result.Errors.Select(c => c.ErrorMessage));
            Assert.Contains(AplicableVoucherValidation.ValidUntilErrorMsg, result.Errors.Select(c => c.ErrorMessage));
            Assert.Contains(AplicableVoucherValidation.ActiveErrorMsg, result.Errors.Select(c => c.ErrorMessage));
            Assert.Contains(AplicableVoucherValidation.UsedErrorMsg, result.Errors.Select(c => c.ErrorMessage));
            Assert.Contains(AplicableVoucherValidation.QuantityErrorMsg, result.Errors.Select(c => c.ErrorMessage));
            Assert.Contains(AplicableVoucherValidation.DiscountTypePercentualErrorMsg, result.Errors.Select(c => c.ErrorMessage));
        }
    }
}
