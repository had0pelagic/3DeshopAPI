using _3DeshopAPI.Extensions;

namespace _3DeshopAPI.Exceptions
{
    public class InvalidClientOperationException : InvalidOperationException
    {
        public InvalidClientOperationException(ErrorCodes code) : base(code.GetEnumDescription())
        {
        }
    }
}
