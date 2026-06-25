using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Sale : BaseEntity
{
    public long SaleNumber { get; private set; }
    public Guid Version { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime MovementDate { get; private set; }
    public DateTime? FinishedAt { get; private set; }
    public decimal Subtotal { get; private set; }
    public decimal Total { get; private set; }
    public decimal AdditionalAmountTotal { get; private set; }
    public decimal DiscountAmountTotal { get; private set; }
    public decimal PaymentAmountTotal { get; private set; }
    public decimal ChangeAmountTotal { get; private set; }
    public SaleStatus Status { get; private set; }
    public DateTime StatusChangedDate { get; private set; }
    public bool IsCanceled { get; private set; }
    public Guid? CancellationAuthorizerId { get; private set; }
    public string? CancellationAuthorizerName { get; private set; }
    public string? CancellationReason { get; private set; }
    public Guid CompanyId { get; private set; }
    public string CompanyName { get; private set; } = string.Empty;
    public Guid CustomerId { get; private set; }
    public string CustomerName { get; private set; } = string.Empty;
    public List<SaleItem> Items { get; private set; } = [];

    public static readonly SaleStatus[] CurrentStatuses =
    {
        SaleStatus.Open,
        SaleStatus.Subtotalized,
        SaleStatus.PaymentCompleted,
        SaleStatus.EmittingNfce,
        SaleStatus.PrintingFiscalReceipt
    };

    public Sale()
    {
    }

    public static Sale Create(
        Guid companyId,
        string companyName,
        Guid customerId,
        string customerName)
    {
        var now = DateTime.UtcNow;
        var sale = new Sale
        {
            StartedAt = now,
            MovementDate = now,
            StatusChangedDate = now,
            Status = SaleStatus.Open,
            Version = Guid.NewGuid(),
            CompanyId = companyId,
            CompanyName = companyName.Trim(),
            CustomerId = customerId,
            CustomerName = customerName.Trim(),
            IsCanceled = false,
            Subtotal = 0,
            Total = 0,
            AdditionalAmountTotal = 0,
            DiscountAmountTotal = 0,
            PaymentAmountTotal = 0,
            ChangeAmountTotal = 0
        };

        sale.ValidateAndThrow();
        sale.AddEvent(new SaleCreatedEvent(sale.Id));
        return sale;
    }

    public void Subtotalize()
    {
        EnsureStatus(SaleStatus.Open, "Only open sales can be subtotalized.");
        if (!Items.Any(item => !item.IsCanceled))
            throw new InvalidOperationException("Sale must have at least one active item before subtotalization.");

        RecalculateTotals();
        ChangeStatus(SaleStatus.Subtotalized);
        AddEvent(new SaleSubtotalizedEvent(Id, StatusChangedDate));
    }

    public void Reopen()
    {
        EnsureStatus(SaleStatus.Subtotalized, "Only subtotalized sales can be reopened.");
        ChangeStatus(SaleStatus.Open);
        AddEvent(new SaleReopenedEvent(Id, StatusChangedDate));
    }

    public void MarkPaymentCompleted()
    {
        EnsureStatus(SaleStatus.Subtotalized, "Only subtotalized sales can complete payment.");
        ChangeStatus(SaleStatus.PaymentCompleted, setFinishedAt: true);
    }

    public void StartNfceEmission()
    {
        EnsureStatus(SaleStatus.PaymentCompleted, "Only paid sales can start NFC-e emission.");
        ChangeStatus(SaleStatus.EmittingNfce);
    }

    public void StartReceiptPrinting()
    {
        EnsureStatus(SaleStatus.EmittingNfce, "Only NFC-e emission sales can start receipt printing.");
        ChangeStatus(SaleStatus.PrintingFiscalReceipt);
    }

    public void MarkIntegratedWithErp()
    {
        EnsureStatus(SaleStatus.PrintingFiscalReceipt, "Only receipt printing sales can be integrated with ERP.");
        ChangeStatus(SaleStatus.IntegratedWithErp);
    }

    public void Cancel(Guid authorizerId, string authorizerName, string? reason)
    {
        if (authorizerId == Guid.Empty)
            throw new ArgumentException("Cancellation authorizer is required.", nameof(authorizerId));

        if (string.IsNullOrWhiteSpace(authorizerName))
            throw new ArgumentException("Cancellation authorizer name is required.", nameof(authorizerName));

        if (!CanCancel(Status))
            throw new InvalidOperationException($"Sale in status {Status} cannot be canceled.");

        CancellationAuthorizerId = authorizerId;
        CancellationAuthorizerName = authorizerName.Trim();
        CancellationReason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
        IsCanceled = true;
        ChangeStatus(SaleStatus.Canceled);
        AddEvent(new SaleCanceledEvent(Id, StatusChangedDate));
    }

    public void RecalculateTotals()
    {
        var activeItems = Items.Where(item => !item.IsCanceled).ToList();
        Subtotal = activeItems.Sum(item => item.Subtotal);
        AdditionalAmountTotal = activeItems.Sum(item => item.AdditionalAmountTotal);
        DiscountAmountTotal = activeItems.Sum(item => item.DiscountAmountTotal);
        Total = Subtotal + AdditionalAmountTotal - DiscountAmountTotal;
    }

    public SaleItem AddItem(
        string productEan,
        Guid productId,
        string productName,
        decimal quantity,
        decimal unitPrice,
        decimal additionalAmountTotal = 0,
        decimal discountAmountTotal = 0)
    {
        EnsureItemChangesAllowed();

        var sequentialNumber = Items.Count == 0 ? 1 : Items.Max(item => item.SequentialNumber) + 1;
        var item = SaleItem.Create(
            Id,
            sequentialNumber,
            productEan,
            productId,
            productName,
            quantity,
            unitPrice,
            additionalAmountTotal,
            discountAmountTotal);

        Items.Add(item);
        RecalculateTotals();
        return item;
    }

    public void UpdateItemQuantity(Guid itemId, decimal quantity)
    {
        EnsureItemChangesAllowed();
        var item = GetItemOrThrow(itemId);
        item.UpdateQuantity(quantity);
        RecalculateTotals();
    }

    public void CancelItem(Guid itemId, Guid authorizerId, string authorizerName, string? reason)
    {
        EnsureItemChangesAllowed();
        var item = GetItemOrThrow(itemId);
        item.Cancel(authorizerId, authorizerName, reason);
        RecalculateTotals();
    }

    public ValidationResultDetail Validate()
    {
        var validator = new SaleValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(error => (ValidationErrorDetail)error)
        };
    }

    public bool MatchesVersion(Guid version)
    {
        return Version == version;
    }

    private static bool CanCancel(SaleStatus status)
    {
        return status == SaleStatus.Open
            || status == SaleStatus.Subtotalized
            || status == SaleStatus.PaymentCompleted;
    }

    private void ChangeStatus(SaleStatus newStatus, bool setFinishedAt = false)
    {
        Status = newStatus;
        var now = DateTime.UtcNow;
        StatusChangedDate = now;
        MovementDate = now;
        Version = Guid.NewGuid();

        if (setFinishedAt)
            FinishedAt = now;

        ValidateAndThrow();
    }

    private void EnsureStatus(SaleStatus expectedStatus, string message)
    {
        if (Status != expectedStatus)
            throw new InvalidOperationException(message);
    }

    private void EnsureItemChangesAllowed()
    {
        if (Status != SaleStatus.Open)
            throw new InvalidOperationException("Items can only be changed while the sale is open.");
    }

    public SaleItem GetItemOrThrow(Guid itemId)
    {
        return Items.FirstOrDefault(item => item.Id == itemId)
            ?? throw new KeyNotFoundException($"Item with ID {itemId} not found in sale.");
    }

    private void ValidateAndThrow()
    {
        var validation = Validate();
        if (!validation.IsValid)
            throw new ArgumentException(validation.Errors.First().Detail);
    }
}
