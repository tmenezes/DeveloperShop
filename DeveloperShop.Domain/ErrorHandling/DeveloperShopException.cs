using System;
using System.Runtime.Serialization;

namespace DeveloperShop.Domain.ErrorHandling
{
    [Serializable]
    public class DeveloperShopException : Exception
    {
        public ErrorType ErrorType { get; set; }

        // constructors
        public DeveloperShopException()
        {
        }

        public DeveloperShopException(string message, ErrorType errorType) : base(message)
        {
            ErrorType = errorType;
        }

        public DeveloperShopException(string message, ErrorType errorType, Exception inner) : base(message, inner)
        {
            ErrorType = errorType;
        }

        protected DeveloperShopException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}