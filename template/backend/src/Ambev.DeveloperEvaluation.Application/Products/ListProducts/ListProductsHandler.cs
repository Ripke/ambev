using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.ListProducts;

public class ListProductsHandler : IRequestHandler<ListProductsCommand, IReadOnlyList<ListProductsResult>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public ListProductsHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ListProductsResult>> Handle(ListProductsCommand request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.ListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<ListProductsResult>>(products);
    }
}
