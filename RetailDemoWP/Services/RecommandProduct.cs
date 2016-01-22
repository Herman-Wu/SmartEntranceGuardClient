using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RetailDemoWP.Models;
using Microsoft.ProjectOxford.Face.Contract;

namespace RetailDemoWP.Services
{
    public class RecommandProduct
    {
        public List<Product> ProductData;
        public async Task<List<Product>> RecommandProductbyImage(Face singleFace)
        {
            List<Product> recommanedProduct = new List<Product>();
            await LoadData();
            string url = "";

            // A single person.
            //List<Product> genderProduct = ProductData.Where(p => p.Gender.ToLower().Equals(singleFace.Attributes.Gender.ToLower())).ToList<Product>();
            foreach (Product p in ProductData)
            {
                int pAge = 100;
                try
                {
                    pAge = int.Parse(p.Age);
                }
                catch(Exception e)
                {
                    continue;
                }
                try
                {
                    if (singleFace.Attributes.Gender.ToLower() == p.Gender.ToLower() && singleFace.Attributes.Age > pAge - 5 && singleFace.Attributes.Age < pAge + 5)
                    {
                        if ((recommanedProduct.Where(c => c.name == p.name).Count<Product>()) ==0)
                        {
                            recommanedProduct.Add(p);
                        }
                    }
                }
                catch (Exception e)
                {
                    continue;
                }
            }
            //recommanedProduct = ProductData.Where(p => int.Parse(p.Age)-5 < singleFace.Attributes.Age && int.Parse(p.Age) +5 > singleFace.Attributes.Age && p.Gender.ToLower().Equals(singleFace.Attributes.Gender.ToLower())).ToList<Product>();

            // recommanedProduct = ProductData.Where(p => p.PGender.ToLower().Equals(singleFace.Attributes.Gender.ToLower())).ToList<Product>();



            return recommanedProduct;
        }

        public async Task LoadData()
        {
            ProductDataService psv = new ProductDataService();
            ProductData = await psv.GetProductData();
        }


    }
}
