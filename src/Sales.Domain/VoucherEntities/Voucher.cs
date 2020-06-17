using FluentValidation.Results;
using System;

namespace Sales.Domain.VoucherEntities
{
    public class Voucher
    {
        public string Code { get; }
        public decimal? DiscountValue { get; }
        public decimal? DiscountPercentage { get; }
        public int Quantity { get; }
        public DateTime ValidUntil { get; }
        public bool Active { get; }
        public bool Used { get; }
        public VoucherType VoucherType { get; }

        public Voucher(VoucherSettings voucherSettings)
        {
            Code = voucherSettings.Code;
            DiscountValue = voucherSettings.DiscountValue;
            DiscountPercentage = voucherSettings.DiscountPercentage;
            Quantity = voucherSettings.Quantity;
            ValidUntil = voucherSettings.ValidUntil;
            Active = voucherSettings.Active;
            Used = voucherSettings.Used;
            VoucherType = voucherSettings.VoucherType;
        }

        public ValidationResult Validate()
        {
            return new AplicableVoucherValidation().Validate(this);
        }
    }

    public class VoucherSettings
    {
        public string Code { get; }
        public decimal? DiscountValue { get; }
        public decimal? DiscountPercentage { get; }
        public int Quantity { get; }
        public DateTime ValidUntil { get; }
        public bool Active { get; }
        public bool Used { get; }
        public VoucherType VoucherType { get; }

        public VoucherSettings(string code, decimal? discountValue, decimal? discountPercentage, int quantity, DateTime validUntil, bool active, bool used, VoucherType voucherType)
        {
            Code = code;
            DiscountValue = discountValue;
            DiscountPercentage = discountPercentage;
            Quantity = quantity;
            ValidUntil = validUntil;
            Active = active;
            Used = used;
            VoucherType = voucherType;
        }
    }
}
