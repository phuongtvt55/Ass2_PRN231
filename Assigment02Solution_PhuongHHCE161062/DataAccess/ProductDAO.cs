using BusinessObject;
using BusinessObject.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class ProductDAO
    {
        private static ProductDAO instance = null;
        private static readonly object instanceLock = new object();
        public static ProductDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new ProductDAO();
                    }
                    return instance;
                }
            }
        }

        public List<Product> GetProductList()
        {
            var product = new List<Product>();
            try
            {
                using var context = new ApplicationDbContext();
                product = context.Products.Include(p => p.Category).ToList();
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
            return product;
        }

        public Product GetProductByID(int productId)
        {
            Product product = null;
            try
            {
                using var context = new ApplicationDbContext();
                product = context.Products.Include(c => c.Category).SingleOrDefault(c => c.ProductId == productId);
                //decimal price = product.UnitPrice;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return product;
        }

        public void AddNew(Product product)
        {
            try
            {
                Product _car = GetProductByID(product.ProductId);
                if (_car == null)
                {
                    using var context = new ApplicationDbContext();
                    product.Category = null;
                    context.Products.Add(product);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("The product is already exist.");
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public void Update(Product product)
        {
            try
            {
                Product _product = GetProductByID(product.ProductId);
                if (_product != null)
                {
                    using var context = new ApplicationDbContext();
                    context.Entry<Product>(product).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("The product does not already exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Remove(int productId)
        {
            try
            {
                Product product = GetProductByID(productId);
                if (product != null)
                {
                    using var context = new ApplicationDbContext();
                    context.Products.Remove(product);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("The product does not already exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Product> SearchProductName(string name)
        {
            try
            {
                var product = new List<Product>();
                using var context = new ApplicationDbContext();
                product = context.Products.Where(p => p.ProductName.Contains(name)).ToList();
                return product;
            }catch(Exception ex) { 
                throw new Exception(ex.Message);
            }
        }

    }
}
