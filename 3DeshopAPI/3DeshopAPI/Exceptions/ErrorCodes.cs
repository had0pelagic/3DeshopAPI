using System.ComponentModel;

namespace _3DeshopAPI.Exceptions
{
    public enum ErrorCodes
    {
        [Description("User already exists")]
        InvalidUsername,
        [Description("User does not exist")]
        UserNotFound,
        [Description("Passwords do not match")]
        BadPassword,
        [Description("Password cannot be the same as previous")]
        SamePassword,
        [Description("Product was not found")]
        ProductNotFound,
        [Description("Category was not found")]
        CategoryNotFound,
        [Description("Format was not found")]
        FormatNotFound,
        [Description("Value is empty")]
        Empty
    }
}
