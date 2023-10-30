using BusinessObject;
using BusinessObject.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class CategoryDAO
    {
        private static CategoryDAO instance = null;
        private static readonly object instanceLock = new object();
        public static CategoryDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new CategoryDAO();
                    }
                    return instance;
                }
            }
        }

        public List<Category> GetCategoryList()
        {
            var cates = new List<Category>();
            try
            {
                using var context = new ApplicationDbContext();
                cates = context.Categories.ToList();
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
            return cates;
        }

        public Category GetCategoryByID(int cateId)
        {
            Category category = null;
            try
            {
                using var context = new ApplicationDbContext();
                category = context.Categories.SingleOrDefault(c => c.CategoryId == cateId);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return category;
        }

        public void AddNew(Category category)
        {
            try
            {
                Category _category = GetCategoryByID(category.CategoryId);
                if (_category == null)
                {
                    using var context = new ApplicationDbContext();
                    context.Categories.Add(category);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("The cagegory is already exist.");
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public void Update(Category category)
        {
            try
            {
                Category _category = GetCategoryByID(category.CategoryId);
                if (_category != null)
                {
                    using var context = new ApplicationDbContext();
                    context.Entry<Category>(category).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("The category does not already exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Remove(int cateId)
        {
            try
            {
                Category category = GetCategoryByID(cateId);
                if (category != null)
                {
                    using var context = new ApplicationDbContext();
                    context.Categories.Remove(category);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("The category does not already exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
