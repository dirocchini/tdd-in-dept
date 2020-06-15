using FluentValidation.Results;
using System;

namespace Sales.Domain.VoucherEntities
{
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

    
}
