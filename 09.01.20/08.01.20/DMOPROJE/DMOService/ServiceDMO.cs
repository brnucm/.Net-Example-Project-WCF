using DTOs;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace DMOService
{

    public class ServiceDMO : IServiceDMO
    {

        NorthwindEntities db = new NorthwindEntities();

        public string AddProduct(string role, ProductsDTO p)
        {
            if (role == "TED")
            {
                Products x = new Products();
                x.ProductName = p.ProductName;
                x.UnitPrice = p.UnitPrice;
                x.Discontinued = false;
                x.SupplierID = p.SupplierID;
                db.Products.Add(x);
                db.SaveChanges();
                return x.ProductName + " Eklendi..";
            }
            else
            {
                return " Kamu personeli ürün ekleyemez.";
            }
        }

        public string DeleteProduct(int id,string role,int sid)
        {
            // Bu ürünü databaseden silersek hata verebilir. Bu yüzden silme işleminde discontinuedini açıyoruz. Böylece satılazları göstermediği için ürün database den silinmesede satılamaz listesine gircek.
            Products p = db.Products.Find(id);
            if (role == "TED")
            {

                if (p.SupplierID == sid)
                {

                    p.Discontinued = true;
                    db.SaveChanges();
                    return p.ProductName + " Silindi";
                }
                else
                {
                    return "Bu Ürün Size Ait Değil";
                }
            }
            else
            {
                return "Kamu Ürün Silemez.";
            }
        }

        public ICollection<ProductsDTO> GetAllProducts(int? supplierId, string role)
        {
            if (supplierId == null)
            {
                supplierId = 0;

            }
            if (role == "KAMU")
            {
                return db.Products.Select(x => new ProductsDTO
                {
                    ProductsID = x.ProductID,
                    ProductName = x.ProductName,
                    SupplierID = (int)x.SupplierID,
                    Discontinued = x.Discontinued,
                    UnitPrice = (decimal)x.UnitPrice

                }).Where(x => x.Discontinued == false).ToList();


            }

            else
            {
                return db.Products.Select(x => new ProductsDTO
                {
                    ProductsID = x.ProductID,
                    ProductName = x.ProductName,
                    SupplierID = (int)x.SupplierID,
                    Discontinued = x.Discontinued,
                    UnitPrice = (decimal)x.UnitPrice

                }).Where(x => x.SupplierID == supplierId && x.Discontinued == false).ToList();
            }

        }

        public UserDTO Login(string userID, string paswword)
        {
            UserDTO userDTO = new UserDTO();

            Users u = db.Users.Find(userID);
            if (u == null)
            {
                return null;

            }

            else  
            {
                if(u.UserID ==userID && u.Password == paswword)
                {
                    userDTO.UserID = u.UserID;
                    userDTO.Role = u.Role;
                    try
                    {
                        userDTO.SupplierID = (int)u.SupplierID;
                    }
                    catch (Exception)
                    {

                        userDTO.SupplierID = 0;
                    }
                   

                    return userDTO;
                }
                else
                {
                    return null;
                }
                
            }
        }
    }
}
