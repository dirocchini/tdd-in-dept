using FluentValidation;
using FluentValidation.Results;
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

    public class Voucher
    {
        public string Code { get; }
        public decimal? DiscountValue { get; }
        public decimal? DiscountPercentual { get; }
        public int Quantity { get; }
        public DateTime ValidUntil { get; }
        public bool Active { get; }
        public bool Used { get; }
        public VoucherType VoucherType { get; }

        public Voucher(string code, decimal? discountValue, decimal? discountPercentual, int quantity, DateTime validUntil, bool active, bool used, VoucherType voucherType)
        {
            Code = code;
            DiscountValue = discountValue;
            DiscountPercentual = discountPercentual;
            Quantity = quantity;
            ValidUntil = validUntil;
            Active = active;
            Used = used;
            VoucherType = voucherType;
        }

        public ValidationResult Validate()
        {
            return new AplicableVoucherValidation().Validate(this);
        }
    }
    public class AplicableVoucherValidation : AbstractValidator<Voucher>
    {
        public static string CodeErrorMsg => "Voucher code must be valid.";
        public static string ValidUntilErrorMsg => "This voucher is expired.";
        public static string ActiveErrorMsg => "Invalid voucher.";
        public static string UsedErrorMsg => "Used Voucher.";
        public static string QuantityErrorMsg => "Not available voucher";
        public static string DiscountTypeValueErrorMsg => "Discount value must be grater than 0";
        public static string DiscountTypePercentualErrorMsg => "Discount percentage must be grater than 0";

        public AplicableVoucherValidation()
        {
            RuleFor(c => c.Code)
                .NotEmpty()
                .WithMessage(CodeErrorMsg);

            RuleFor(c => c.ValidUntil)
                .Must(IsDateValid)
                .WithMessage(ValidUntilErrorMsg);

            RuleFor(c => c.Active)
                .Equal(true)
                .WithMessage(ActiveErrorMsg);

            RuleFor(c => c.Used)
                .Equal(false)
                .WithMessage(UsedErrorMsg);

            RuleFor(c => c.Quantity)
                .GreaterThan(0)
                .WithMessage(QuantityErrorMsg);

            When(f => f.VoucherType == VoucherType.Value, () =>
            {
                RuleFor(f => f.DiscountValue)
                    .NotNull()
                    .WithMessage(DiscountTypeValueErrorMsg)
                    .GreaterThan(0)
                    .WithMessage(DiscountTypeValueErrorMsg);
            });

            When(f => f.VoucherType == VoucherType.Percentage, () =>
            {
                RuleFor(f => f.DiscountPercentual)
                    .NotNull()
                    .WithMessage(DiscountTypePercentualErrorMsg)
                    .GreaterThan(0)
                    .WithMessage(DiscountTypePercentualErrorMsg);
            });
        }

        protected static bool IsDateValid(DateTime validUntil)
        {
            return validUntil >= DateTime.Now;
        }
    }


    public enum VoucherType
    {
        Percentage = 0,
        Value = 1
    }

    
}
