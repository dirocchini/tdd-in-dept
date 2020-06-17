using System;
using Core.Messages;
using FluentValidation;
using MediatR;
using Sales.Domain;

namespace Sales.Application.Commands
{
    public class AddOrderItemCommand : Command
    {
        public Guid ClientId { get; private set; }
        public Guid ItemId { get; }
        public string ItemName { get; }
        public int ItemQuantity { get; }
        public decimal ItemValue { get; }


        public AddOrderItemCommand(Guid clientId, Guid itemId, string itemName, int itemQuantity, decimal itemValue)
        {
            ClientId = clientId;
            ItemId = itemId;
            ItemName = itemName;
            ItemQuantity = itemQuantity;
            ItemValue = itemValue;
        }

        
        public override bool IsValid()
        {
            ValidationResult = new AddOrderItemCommandValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }

    public class AddOrderItemCommandValidation : AbstractValidator<AddOrderItemCommand>
    {
        public static string IdClienteErroMsg => "Id do cliente inválido";
        public static string IdProdutoErroMsg => "Id do produto inválido";
        public static string NomeErroMsg => "O nome do produto não foi informado";
        public static string QtdMaxErroMsg => $"A quantidade máxima de um item é {Order.MAX_ITEM_QUANTITY_PER_ITEM}";
        public static string QtdMinErroMsg => "A quantidade miníma de um item é 1";
        public static string ValorErroMsg => "O valor do item precisa ser maior que 0";

        public AddOrderItemCommandValidation()
        {
            RuleFor(c => c.ClientId)
                .NotEqual(Guid.Empty)
                .WithMessage(IdClienteErroMsg);

            RuleFor(c => c.ItemId)
                .NotEqual(Guid.Empty)
                .WithMessage(IdProdutoErroMsg);

            RuleFor(c => c.ItemName)
                .NotEmpty()
                .WithMessage(NomeErroMsg);

            RuleFor(c => c.ItemQuantity)
                .GreaterThan(0)
                .WithMessage(QtdMinErroMsg)
                .LessThanOrEqualTo(Order.MAX_ITEM_QUANTITY_PER_ITEM)
                .WithMessage(QtdMaxErroMsg);

            RuleFor(c => c.ItemValue)
                .GreaterThan(0)
                .WithMessage(ValorErroMsg);
        }
    }
}