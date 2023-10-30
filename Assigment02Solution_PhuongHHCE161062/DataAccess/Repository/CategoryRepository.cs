using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        public void DeleteCategory(int CategoryId)
        => CategoryDAO.Instance.Remove(CategoryId);

        public Category GetCategoryById(int carId)
        => CategoryDAO.Instance.GetCategoryByID(carId);

        public List<Category> GetCategorys()
        => CategoryDAO.Instance.GetCategoryList();

        public void InsertCategory(Category Category)
        => CategoryDAO.Instance.AddNew(Category);

        public void UpdateCategory(Category Category)
        => CategoryDAO.Instance.Update(Category);
    }
}
