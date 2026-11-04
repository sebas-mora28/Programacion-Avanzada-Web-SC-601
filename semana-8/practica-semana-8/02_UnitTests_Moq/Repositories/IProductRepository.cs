using UnitTests.MoqPractice.Models;
namespace UnitTests.MoqPractice.Repositories;
public interface IProductRepository
{
    Product? GetById(int id);
}
