namespace Acerto.Shared.Contracts.Messages
{
    public static class ResultMessages
    {
        // orders
        public const string OrderPlacedSuccessfully = "Order placed successfully.";

        // products
        public const string PageAndPageSizeErrorMessage = "page and pageSize must be greather than 0.";
        public const string PriceMustBeGreaterThanZero = "'Price' must be greater than '0'.";

        // auth
        public const string UserIsLocked = "User is locked.";
        public const string InvalidUserOrPassword = "Invalid user or password.";
        public const string RefreshTokenIsExpired = "RefreshToken is expired.";
    }
}
