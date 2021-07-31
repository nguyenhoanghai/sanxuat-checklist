using GPRO.Core.Mvc;
using GPRO.Ultilities;
using Hugate.Framework;
using PagedList;
using SanXuatCheckList.Business.Model;
using SanXuatCheckList.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SanXuatCheckList.Business
{
    public class BLLProduct
    {
        #region constructor
        SanXuatCheckListEntities db;
        static object key = new object();
        private static volatile BLLProduct _Instance;
        public static BLLProduct Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLProduct();

                return _Instance;
            }
        }
        private BLLProduct() { }
        #endregion
        bool checkPermis(Product obj, int actionUser, bool isOwner)
        {
            if (isOwner) return true;
            return obj.CreatedUser == actionUser;
        }

        public PagedList<ProductModel> GetList(string keyWord, int companyId, int[] relationCompanyId, int startIndexRecord, int pageSize, string sorting)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    if (string.IsNullOrEmpty(sorting))
                        sorting = "CreatedDate DESC";

                    IQueryable<Product> products = db.Product
                                 .Where(x => !x.IsDeleted && !x.Customer.IsDeleted && !x.Size.IsDeleted &&
                                 (x.CompanyId == null || x.CompanyId == companyId || relationCompanyId.Contains(x.CompanyId ?? 0)));

                    if (!string.IsNullOrEmpty(keyWord))
                    {
                        keyWord = keyWord.Trim().ToUpper();
                        products = products.Where(x => (x.Name.Trim().ToUpper().Contains(keyWord.Trim().ToUpper()) || x.Code.Trim().ToUpper().Contains(keyWord.Trim().ToUpper())));

                    }
                    var pageNumber = (startIndexRecord / pageSize) + 1;
                    return new PagedList<ProductModel>(products.Select(
                            x => new ProductModel()
                            {
                                Id = x.Id,
                                Name = x.Name,
                                Code = x.Code,
                                CustomerId = x.CustomerId,
                                CustomerName = x.Customer.Name,
                                SizeId = x.SizeId,
                                SizeName = x.Size.Name,
                                UnitId = x.UnitId,
                                UnitName = x.Unit.Name,
                                Note = x.Note,
                                IsPrivate = (x.CompanyId == null ? true : false),
                                CompanyId = x.CompanyId,
                                Image = x.Image,
                                CreatedDate = x.CreatedDate
                            }).OrderBy(sorting).ToList(), pageNumber, pageSize);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
         
        public ResponseBase InsertOrUpdate(ProductModel model, bool isOwner)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var result = new ResponseBase();
                    if (CheckExists(model.Name.Trim().ToUpper(), model.Id, model.CompanyId, true))
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Insert Product Type", Message = "Tên Sản Phẩm này đã tồn tại. Vui lòng chọn lại Tên khác !." });
                        return result;
                    }
                    else
                    {
                        //if (!string.IsNullOrEmpty(model.Code))
                        //{
                        //    if (CheckExists(model.Code.Trim().ToUpper(), model.Id, model.CompanyId , false))
                        //    {
                        //        result.IsSuccess = false;
                        //        result.Errors.Add(new Error() { MemberName = "Insert Product Type", Message = "Mã  Sản Phẩm này đã tồn tại. Vui lòng chọn lại Tên khác !." });
                        //        return result;
                        //    }
                        //}
                        Product obj;
                        if (model.Id == 0)
                        {
                            obj = new Product();
                            Parse.CopyObject(model, ref obj);
                            obj.CreatedDate = DateTime.Now;
                            obj.CreatedUser = model.ActionUser;
                            db.Product.Add(obj);
                            db.SaveChanges();
                            result.IsSuccess = true;
                        }
                        else
                        {
                            obj = db.Product.FirstOrDefault(x => !x.IsDeleted && x.Id == model.Id);
                            if (obj == null)
                            {
                                result.IsSuccess = false;
                                result.Errors.Add(new Error() { MemberName = "Update Product Type", Message = "Sản Phẩm bạn đang thao tác đã bị xóa hoặc không tồn tại. Vui lòng kiểm tra lại !." });
                                return result;
                            }
                            else
                            {
                                if (!checkPermis(obj, model.ActionUser, isOwner))
                                {
                                    result.IsSuccess = false;
                                    result.Errors.Add(new Error() { MemberName = "update", Message = "Bạn không phải là người tạo sản phẩm này nên bạn không cập nhật được thông tin cho sản phẩm này." });
                                }
                                else
                                {
                                    obj.CompanyId = model.CompanyId;
                                    obj.Name = model.Name;
                                    obj.Code = model.Code;
                                    obj.CustomerId = model.CustomerId;
                                    obj.SizeId = model.SizeId;
                                    obj.UnitId = model.UnitId;
                                    obj.Note = model.Note;
                                    if (!string.IsNullOrEmpty(model.Image))
                                    {
                                        obj.Image = model.Image;
                                    }

                                    obj.UpdatedUser = model.ActionUser;
                                    obj.UpdatedDate = DateTime.Now;

                                    //  cap nhat ben phan tich mat hang
                                    //var commoAna = db.T_CommodityAnalysis.Where(x => !x.IsDeleted && x.ObjectId == obj.Id && x.ObjectType == (int)eObjectType.isCommodity);
                                    //if (commoAna != null && commoAna.Count() > 0)
                                    //{
                                    //    foreach (var item in commoAna)
                                    //    {
                                    //        item.Name = model.Name;
                                    //        item.UpdatedUser = model.ActionUser;
                                    //        item.UpdatedDate = DateTime.Now;
                                    //    }
                                    //}
                                    db.SaveChanges();
                                    result.IsSuccess = true;
                                }
                            }
                        }

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool CheckExists(string code, int? id, int? companyId, bool isName)
        {
            try
            {
                Product obj = null;
                if (!isName)
                    obj = db.Product.FirstOrDefault(x => !x.IsDeleted && x.CompanyId == companyId && x.Code.Trim().ToUpper().Equals(code) && x.Id != id);
                else
                    obj = db.Product.FirstOrDefault(x => !x.IsDeleted && x.CompanyId == companyId && x.Name.Trim().ToUpper().Equals(code) && x.Id != id);

                if (obj == null)
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResponseBase Delete(int id, int acctionUserId, bool isOwner)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var result = new ResponseBase();
                    var obj = db.Product.FirstOrDefault(x => !x.IsDeleted && x.Id == id);
                    if (obj == null)
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Delete Product Type", Message = "Sản Phẩm bạn đang thao tác đã bị xóa hoặc không tồn tại. Vui lòng kiểm tra lại !." });
                    }
                    else
                    {
                        if (!checkPermis(obj, acctionUserId, isOwner))
                        {
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "Delete", Message = "Bạn không phải là người tạo sản phẩm này nên bạn không xóa được sản phẩm này." });
                        }
                        else
                        {
                            obj.IsDeleted = true;
                            obj.DeletedUser = acctionUserId;
                            obj.DeletedDate = DateTime.Now;

                            //var proanas = (from x in db.T_CommodityAnalysis where !x.IsDeleted && x.ObjectType == 1 && x.ObjectId == productType.Id select x) ;
                            //if(proanas != null && proanas.Count() > 0)
                            //    foreach (var item in proanas)
                            //    {
                            //        item.IsDeleted = true;
                            //        item.DeletedUser = acctionUserId;
                            //        item.DeletedDate = DateTime.Now;
                            //    }

                            db.SaveChanges();
                            result.IsSuccess = true;
                        }
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ModelSelectItem> GetSelectItem(int companyId, int[] relationCompanyId)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var listModelSelect = new List<ModelSelectItem>();
                    var productTypes = db.Product.Where(x => !x.IsDeleted
                    && (x.CompanyId == null || x.CompanyId == companyId || relationCompanyId.Contains(x.CompanyId ?? 0))).Select(
                        x => new ModelSelectItem()
                        {
                            Value = x.Id,
                            Name = x.Name,
                            Code = "Khách hàng: " + x.Customer.Name + " - kích cỡ: " + x.Size.Name,
                            Data = x.CustomerId
                        }).ToList();

                    if (productTypes != null && productTypes.Count() > 0)
                    {
                        //listModelSelect.Add(new ModelSelectItem() { Value = 0, Name = " - -  Chọn Sản Phẩm  - - " });
                        listModelSelect.AddRange(productTypes);
                    }
                    else
                        listModelSelect.Add(new ModelSelectItem() { Value = 0, Name = "  Không có Sản Phẩm  " });
                    return listModelSelect;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
