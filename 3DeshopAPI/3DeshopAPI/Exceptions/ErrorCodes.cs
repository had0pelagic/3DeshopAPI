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
        SamePassword
    }
}
