using System;
using FluentValidation.Results;

namespace Core.Messages
{
    public abstract class Command : Message
    {
        public DateTime Timestamp { get; set; }
        public ValidationResult ValidationResult { get; set; }

        public Command()
        {
            Timestamp = DateTime.Now;
        }

        public abstract bool IsValid();
    }
}