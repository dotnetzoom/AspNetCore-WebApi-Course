using System.Linq;
using Data.Contracts;
using Entities.Post;

namespace Services.DataInitializer
{
    public class CategoryDataInitializer : IDataInitializer
    {
        private readonly IRepository<Category> _repository;

        public CategoryDataInitializer(IRepository<Category> repository)
        {
            this._repository = repository;
        }

        public void InitializeData()
        {
            if (!_repository.TableNoTracking.Any(p => p.Name == "دسته بندی اولیه 1"))
            {
                _repository.Add(new Category
                {
                    Name = "دسته بندی اولیه 1"
                });
            }
            if (!_repository.TableNoTracking.Any(p => p.Name == "دسته بندی اولیه 2"))
            {
                _repository.Add(new Category
                {
                    Name = "دسته بندی اولیه 2"
                });
            }
            if (!_repository.TableNoTracking.Any(p => p.Name == "دسته بندی اولیه 3"))
            {
                _repository.Add(new Category
                {
                    Name = "دسته بندی اولیه 3"
                });
            }
        }
    }
}
