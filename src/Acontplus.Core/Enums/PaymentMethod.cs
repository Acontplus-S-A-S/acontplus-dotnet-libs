namespace Acontplus.Core.Enums;

/// <summary>
/// Payment method enum for e-commerce and financial applications
/// </summary>
public enum PaymentMethod
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
    /// PayPal payment
    /// </summary>
    PayPal = 5,

    /// <summary>
    /// Apple Pay
    /// </summary>
    ApplePay = 6,

    /// <summary>
    /// Google Pay
    /// </summary>
    GooglePay = 7,

    /// <summary>
    /// Stripe payment
    /// </summary>
    Stripe = 8,

    /// <summary>
    /// Cryptocurrency payment
    /// </summary>
    Cryptocurrency = 9,

    /// <summary>
    /// Check payment
    /// </summary>
    Check = 10,

    /// <summary>
    /// Money order
    /// </summary>
    MoneyOrder = 11,

    /// <summary>
    /// Gift card or voucher
    /// </summary>
    GiftCard = 12,

    /// <summary>
    /// Store credit
    /// </summary>
    StoreCredit = 13,

    /// <summary>
    /// Buy now, pay later (BNPL)
    /// </summary>
    BuyNowPayLater = 14,

    /// <summary>
    /// Mobile payment (generic)
    /// </summary>
    MobilePayment = 15
}
