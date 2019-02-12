using Data.Repositories;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services.DataInitializer
{
    public class CategoryDataInitializer : IDataInitializer
    {
        private readonly IRepository<Category> repository;

        public CategoryDataInitializer(IRepository<Category> repository)
        {
            this.repository = repository;
        }

        public void InitializeData()
        {
            if (!repository.TableNoTracking.Any(p => p.Name == "دسته بندی اولیه 1"))
            {
                repository.Add(new Category
                {
                    Name = "دسته بندی اولیه 1"
                });
            }
            if (!repository.TableNoTracking.Any(p => p.Name == "دسته بندی اولیه 2"))
            {
                repository.Add(new Category
                {
                    Name = "دسته بندی اولیه 2"
                });
            }
            if (!repository.TableNoTracking.Any(p => p.Name == "دسته بندی اولیه 3"))
            {
                repository.Add(new Category
                {
                    Name = "دسته بندی اولیه 3"
                });
            }
        }
    }
}
