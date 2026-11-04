using UnitTests.MoqPractice.Repositories;
namespace UnitTests.MoqPractice.Services;
public class OrderService
{
    private readonly IProductRepository _repository;
    public OrderService(IProductRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }
    public decimal CalculateTotal(IEnumerable<int> productIds)
    {

        throw new NotImplementedException();
    }
}
