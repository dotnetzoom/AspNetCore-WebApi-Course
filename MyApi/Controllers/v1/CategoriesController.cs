using AutoMapper;
using Data.Repositories;
using Entities;
using MyApi.Models;
using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    public class CategoriesController : CrudController<CategoryDto, Category>
    {
        public CategoriesController(IRepository<Category> repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }
}
