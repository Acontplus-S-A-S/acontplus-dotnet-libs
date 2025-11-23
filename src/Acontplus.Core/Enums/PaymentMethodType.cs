namespace Acontplus.Core.Enums;

/// <summary>
/// Payment method type enum for e-commerce and financial applications.
/// Represents generic payment method categories rather than specific providers.
/// </summary>
public enum PaymentMethodType
{
    /// <summary>
    /// Cash payment
    /// </summary>
    Cash = 1,

    /// <summary>
    /// Credit card payment
    /// </summary>
    CreditCard = 2,

    /// <summary>
    /// Debit card payment
    /// </summary>
    DebitCard = 3,

    /// <summary>
    /// Bank transfer
    /// </summary>
    BankTransfer = 4,

    /// <summary>
    /// Digital wallet payment (e.g., PayPal, Apple Pay, Google Pay)
    /// </summary>
    DigitalWallet = 5,

    /// <summary>
    /// Cryptocurrency payment
    /// </summary>
    Cryptocurrency = 6,

    /// <summary>
    /// Check payment
    /// </summary>
    Check = 7,

    /// <summary>
    /// Money order
    /// </summary>
    MoneyOrder = 8,

    /// <summary>
    /// Gift card or voucher
    /// </summary>
    GiftCard = 9,

    /// <summary>
    /// Store credit
    /// </summary>
    StoreCredit = 10,

    /// <summary>
    /// Buy now, pay later (BNPL)
    /// </summary>
    BuyNowPayLater = 11,

    /// <summary>
    /// Mobile payment (generic)
    /// </summary>
    MobilePayment = 12
}
