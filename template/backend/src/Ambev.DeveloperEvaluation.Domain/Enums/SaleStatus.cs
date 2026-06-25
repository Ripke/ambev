namespace Ambev.DeveloperEvaluation.Domain.Enums;

public enum SaleStatus
{
    Unknown = 0,
    Open = 1,
    Subtotalized = 2,
    PaymentCompleted = 3,
    EmittingNfce = 4,
    PrintingFiscalReceipt = 5,
    IntegratedWithErp = 6,
    Canceled = 99
}
