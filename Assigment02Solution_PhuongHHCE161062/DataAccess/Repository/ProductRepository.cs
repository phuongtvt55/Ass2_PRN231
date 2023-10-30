using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class ProductRepository : IProductRepository
    {
        public void DeleteProduct(int productId)
        => ProductDAO.Instance.Remove(productId);

        public Product GetProductById(int carId)
        => ProductDAO.Instance.GetProductByID(carId);

        public List<Product> GetProducts()
        => ProductDAO.Instance.GetProductList();

        public void InsertProduct(Product product)
        => ProductDAO.Instance.AddNew(product);

        public void UpdateProduct(Product product)
        => ProductDAO.Instance.Update(product);

        public List<Product> SearchProductName(string name)
        => ProductDAO.Instance.SearchProductName(name);
    }
}
