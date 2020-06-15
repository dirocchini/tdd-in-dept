using FluentValidation;
using System;

namespace Sales.Domain.VoucherEntities
{
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

    
}
