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
        Empty,
        [Description("Error while sending an email")]
        Email,
        [Description("Please upload file")]
        File,
        [Description("User has not bought the product")]
        NotPaid,
        [Description("Order has not been found")]
        OrderNotFound,
        [Description("Offer has not been found")]
        OfferNotFound
    }
}
