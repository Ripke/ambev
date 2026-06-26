using Ambev.DeveloperEvaluation.Application.SalesPromotions.CreateSalesPromotion;
using Ambev.DeveloperEvaluation.Application.SalesPromotions.DeleteSalesPromotion;
using Ambev.DeveloperEvaluation.Application.SalesPromotions.GetSalesPromotion;
using Ambev.DeveloperEvaluation.Application.SalesPromotions.ListSalesPromotions;
using Ambev.DeveloperEvaluation.Application.SalesPromotions.UpdateSalesPromotion;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.SalesPromotions;

public class SalesPromotionHandlersTests
{
    [Fact]
    public async Task Create_ValidRequest_ReturnsCreatedPromotion()
    {
        var repository = Substitute.For<ISalesPromotionRepository>();
        var mapper = Substitute.For<IMapper>();
        var handler = new CreateSalesPromotionHandler(repository, mapper);
        var command = CreateCommand();
        var promotion = CreatePromotion(command);
        var result = new CreateSalesPromotionResult { Id = Guid.NewGuid(), Name = command.Name };

        repository.CreateAsync(Arg.Any<SalesPromotion>(), Arg.Any<CancellationToken>()).Returns(promotion);
        mapper.Map<CreateSalesPromotionResult>(promotion).Returns(result);

        var response = await handler.Handle(command, CancellationToken.None);

        response.Should().BeSameAs(result);
        await repository.Received(1).CreateAsync(Arg.Any<SalesPromotion>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Get_WhenPromotionExists_ReturnsPromotion()
    {
        var repository = Substitute.For<ISalesPromotionRepository>();
        var mapper = Substitute.For<IMapper>();
        var handler = new GetSalesPromotionHandler(repository, mapper);
        var promotion = CreatePromotion(CreateCommand());
        var result = new GetSalesPromotionResult { Id = promotion.Id, Name = promotion.Name };

        repository.GetByIdAsync(promotion.Id, Arg.Any<CancellationToken>()).Returns(promotion);
        mapper.Map<GetSalesPromotionResult>(promotion).Returns(result);

        var response = await handler.Handle(new GetSalesPromotionCommand(promotion.Id), CancellationToken.None);

        response.Should().BeSameAs(result);
    }

    [Fact]
    public async Task List_ReturnsMappedPromotions()
    {
        var repository = Substitute.For<ISalesPromotionRepository>();
        var mapper = Substitute.For<IMapper>();
        var handler = new ListSalesPromotionsHandler(repository, mapper);
        var promotions = new List<SalesPromotion> { CreatePromotion(CreateCommand()) };
        var result = new List<ListSalesPromotionsResult> { new() { Id = promotions[0].Id, Name = promotions[0].Name } };

        repository.ListAsync(Arg.Any<CancellationToken>()).Returns(promotions);
        mapper.Map<IReadOnlyList<ListSalesPromotionsResult>>(promotions).Returns(result);

        var response = await handler.Handle(new ListSalesPromotionsCommand(), CancellationToken.None);

        response.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task Update_ValidRequest_UpdatesPromotion()
    {
        var repository = Substitute.For<ISalesPromotionRepository>();
        var mapper = Substitute.For<IMapper>();
        var handler = new UpdateSalesPromotionHandler(repository, mapper);
        var command = CreateUpdateCommand();
        var promotion = CreatePromotion(CreateCommand());
        var result = new UpdateSalesPromotionResult { Id = promotion.Id, Name = command.Name };

        repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(promotion);
        mapper.Map<UpdateSalesPromotionResult>(promotion).Returns(result);

        var response = await handler.Handle(command, CancellationToken.None);

        promotion.Name.Should().Be(command.Name);
        response.Should().BeSameAs(result);
        await repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Delete_DeactivatesPromotion()
    {
        var repository = Substitute.For<ISalesPromotionRepository>();
        var handler = new DeleteSalesPromotionHandler(repository);
        var promotion = CreatePromotion(CreateCommand());

        repository.GetByIdAsync(promotion.Id, Arg.Any<CancellationToken>()).Returns(promotion);

        var response = await handler.Handle(new DeleteSalesPromotionCommand(promotion.Id), CancellationToken.None);

        response.Success.Should().BeTrue();
        promotion.IsActive.Should().BeFalse();
        await repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    private static CreateSalesPromotionCommand CreateCommand()
    {
        return new CreateSalesPromotionCommand
        {
            Name = "Promo",
            Description = "Descricao",
            Priority = 1,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(1),
            ProductId = null,
            IsActive = true,
            Items = [new() { MinimumQuantity = 4, MaximumQuantity = 9, DiscountType = DiscountType.Percentage, DiscountValue = 10 }]
        };
    }

    private static UpdateSalesPromotionCommand CreateUpdateCommand()
    {
        return new UpdateSalesPromotionCommand
        {
            Id = Guid.NewGuid(),
            Name = "Promo Atualizada",
            Description = "Nova descricao",
            Priority = 2,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(2),
            ProductId = Guid.NewGuid(),
            IsActive = true,
            Items = [new() { MinimumQuantity = 10, MaximumQuantity = 20, DiscountType = DiscountType.FixedAmount, DiscountValue = 2 }]
        };
    }

    private static SalesPromotion CreatePromotion(CreateSalesPromotionCommand command)
    {
        return SalesPromotion.Create(
            command.Name,
            command.Description,
            command.Priority,
            command.StartDate,
            command.EndDate,
            command.ProductId,
            command.IsActive,
            command.Items.Select(item => SalesPromotionItem.Create(item.MinimumQuantity, item.MaximumQuantity, item.DiscountType, item.DiscountValue)));
    }
}
