using System;
using System.Runtime.Serialization;

namespace DevBasics.CarManagement.Dependencies
{
    internal class ForceRegistermentException : Exception
    {
        public ForceRegistermentException()
        {
        }

        public ForceRegistermentException(string message) : base(message)
        {
        }

        public ForceRegistermentException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ForceRegistermentException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}