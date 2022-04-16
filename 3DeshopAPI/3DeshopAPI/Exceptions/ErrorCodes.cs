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
        OfferNotFound,
        [Description("Job has not been found")]
        JobNotFound,
        [Description("User is unauthorized for this action")]
        UnauthorizedForAction,
        [Description("Order cannot be removed, because it is active")]
        CantRemoveOrderIsActive,
        [Description("Not enough balance")]
        NotEnoughBalance,
        [Description("Product can't be bought by the owner")]
        OwnerUnableToBuyProduct,
        [Description("Can't buy the same product again")]
        DuplicateBuy,
        [Description("Payment was not found")]
        BalanceHistoryNotFound,
        [Description("Image was not found")]
        ImageNotFound
    }
}
