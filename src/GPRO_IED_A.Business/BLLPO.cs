using GPRO.Core.Mvc;
using GPRO.Ultilities;
using Hugate.Framework;
using PagedList;
using SanXuatCheckList.Business.Enum;
using SanXuatCheckList.Business.Model;
using SanXuatCheckList.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SanXuatCheckList.Business
{
    public class BLLPO_Sell
    {
        #region constructor
        SanXuatCheckListEntities db;
        static object key = new object();
        private static volatile BLLPO_Sell _Instance;
        public static BLLPO_Sell Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLPO_Sell();

                return _Instance;
            }
        }
        private BLLPO_Sell() { }
        #endregion

        bool checkPermis(PO_Sell obj, int actionUser, bool isOwner)
        {
            if (isOwner) return true;
            return obj.CreatedUser == actionUser;
        }

        public PagedList<PO_SellModel> GetList(string keyWord, int startIndexRecord, int pageSize, string sorting)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    if (string.IsNullOrEmpty(sorting))
                        sorting = "CreatedDate DESC";

                    IQueryable<PO_Sell> PO_Sells = db.PO_Sell.Where(x => !x.IsDeleted && !x.Customer.IsDeleted);
                    if (!string.IsNullOrEmpty(keyWord))
                        PO_Sells = PO_Sells.Where(x => x.Code.Trim().ToUpper().Contains(keyWord.Trim().ToUpper()));

                    var pageNumber = (startIndexRecord / pageSize) + 1;
                    var objs = new PagedList<PO_SellModel>(PO_Sells.OrderBy(sorting).Select(x => new PO_SellModel()
                    {
                        Id = x.Id,
                        Code = x.Code,
                         //nam = x.Name,
                        CustomerId = x.CustomerId,
                        CustomerName = x.Customer.Name,
                        //ProductId = x.ProductId,
                        //ProductName = x.Product.Name,
                        //Image = x.Image,
                        MoneyUnitId = x.MoneyUnitId,
                        MoneyTypeName = x.Unit.Name,
                        //ProductUnit = x.Product.Unit.Name,
                        //ProductSize = x.Product.Size.Name,  
                        // Price = x.Price,
                        Phone = x.Phone,
                        DeliveryDate = x.DeliveryDate,
                        Exchange = x.Exchange,
                        Note = x.Note,
                        StatusId = x.StatusId,
                        StatusName = x.Status.Name
                        //  Total = (x.Quantities * x.Price)
                    }).ToList(), pageNumber, pageSize);
                    if (objs.Count > 0)
                    {
                        var ids = objs.Select(x => x.Id).ToArray();
                        var details = db.PO_SellDetail
                            .Where(x => !x.IsDeleted && !x.Product.IsDeleted && ids.Contains(x.POId))
                            .Select(x => new { POId = x.POId, Quantity = x.Quantities, Price = x.Price })
                            .ToList();
                        for (int i = 0; i < objs.Count; i++)
                        {
                            objs[i].Total = details.Where(x => x.POId == objs[i].Id).Sum(x => x.Quantity * x.Price);
                        }
                    }

                    return objs;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PO_SellModel Get(int poId)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var obj = db.PO_Sell
                        .Where(x => !x.IsDeleted && !x.Customer.IsDeleted && x.Id == poId)
                     .Select(x => new PO_SellModel()
                     {
                         Id = x.Id,
                         Code = x.Code,
                         CustomerId = x.CustomerId,
                         CustomerName = x.Customer.Name,
                         MoneyUnitId = x.MoneyUnitId,
                         MoneyTypeName = x.Unit.Name,
                         Phone = x.Phone,
                         DeliveryDate = x.DeliveryDate,
                         Exchange = x.Exchange,
                         Note = x.Note,
                         StatusId = x.StatusId,
                         StatusName = x.Status.Name
                     }).FirstOrDefault();

                    var details = db.PO_SellDetail
                        .Where(x => !x.IsDeleted && !x.Product.IsDeleted && x.POId == poId)
                        .Select(x => new PO_SellDetailModel { Id = x.Id, POId = x.POId, Quantities = x.Quantities, Price = x.Price, ProductId = x.ProductId, ProductName = x.Product.Name, ProductUnit = x.Product.Unit.Name })
                        .ToList();
                    obj.Total = details.Sum(x => x.Quantities * x.Price);
                    obj.Details.AddRange(details);
                    return obj;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ResponseBase InsertOrUpdate(PO_SellModel model, bool isOwner)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var result = new ResponseBase();
                    if (CheckExists(model.Code.Trim().ToUpper(), model.Id))
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Insert ", Message = "Mã phiếu này đã tồn tại. Vui lòng chọn lại Mã khác !." });
                        return result;
                    }
                    else if (model.StatusId != (int)eStatus.Draft && model.Details == null || model.Details.Count == 1)
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Insert ", Message = "Vui lòng chọn ít nhất 1 sản phẩm cho phiếu đặt hàng" });
                        return result;
                    }
                    else
                    {
                        PO_Sell obj;
                        if (model.Id == 0)
                        {
                            obj = new PO_Sell();
                            Parse.CopyObject(model, ref obj);
                            obj.Code = model.Code;// DateTime.Now.ToString("ddMMyyyy-HHmmss");
                            obj.CreatedDate = DateTime.Now;
                            obj.CreatedUser = model.ActionUser;

                            if (model.Details != null && model.Details.Count > 1)
                            {
                                obj.PO_SellDetail = new List<PO_SellDetail>();
                                PO_SellDetail child = null;
                                foreach (var item in model.Details)
                                {
                                    if (item.ProductId != 0)
                                    {
                                        child = new PO_SellDetail();
                                        Parse.CopyObject(item, ref child);
                                        child.CreatedDate = obj.CreatedDate;
                                        child.CreatedUser = obj.CreatedUser;
                                        child.PO_Sell = obj;
                                        obj.PO_SellDetail.Add(child);
                                    }
                                }
                            }
                            db.PO_Sell.Add(obj);
                            db.SaveChanges();
                            result.IsSuccess = true;
                        }
                        else
                        {
                            obj = db.PO_Sell.FirstOrDefault(x => !x.IsDeleted && x.Id == model.Id);
                            if (obj == null)
                            {
                                result.IsSuccess = false;
                                result.Errors.Add(new Error() { MemberName = "Update ", Message = "Phiếu bạn đang thao tác đã bị xóa hoặc không tồn tại. Vui lòng kiểm tra lại !." });
                                return result;
                            }
                            else if (obj.StatusId != (int)eStatus.Draft && obj.StatusId != (int)eStatus.Submited)
                            {
                                result.IsSuccess = false;
                                result.Errors.Add(new Error() { MemberName = "Update ", Message = "Trạng thái phiếu đặt hàng này không còn cho phép bạn cập nhật thông tin. Vui lòng liện hệ Admin" });
                                return result;
                            }
                            else
                            {
                                if (!checkPermis(obj, model.ActionUser, isOwner))
                                {
                                    result.IsSuccess = false;
                                    result.Errors.Add(new Error() { MemberName = "update", Message = "Bạn không phải là người tạo phiếu này nên bạn không cập nhật được thông tin cho phiếu này." });
                                }
                                else
                                {
                                    obj.Phone = model.Phone;
                                    obj.CustomerId = model.CustomerId;
                                    obj.DeliveryDate = model.DeliveryDate;
                                    obj.StatusId = model.StatusId;
                                    obj.MoneyUnitId = model.MoneyUnitId;
                                    obj.Exchange = model.Exchange;
                                    obj.Note = model.Note;
                                    obj.UpdatedUser = model.ActionUser;
                                    obj.UpdatedDate = DateTime.Now;

                                    var _details = db.PO_SellDetail.Where(x => x.POId == obj.Id);
                                    if (_details != null && _details.Count() > 0)
                                    {
                                        var newChilds = model.Details.Where(x => x.ProductId != 0).ToList();
                                        foreach (var item in _details)
                                        {
                                            var found = newChilds.FirstOrDefault(x => x.ProductId == item.ProductId);
                                            if (found == null)
                                            {
                                                //ko còn xóa đi
                                                item.IsDeleted = true;
                                                item.DeletedUser = model.ActionUser;
                                                item.DeletedDate = obj.UpdatedDate;
                                            }
                                            else
                                            {
                                                // có update thong tin moi
                                                item.Quantities = found.Quantities;
                                                item.Price = found.Price;
                                                item.UpdatedUser = model.ActionUser;
                                                item.UpdatedDate = obj.UpdatedDate;
                                                newChilds.Remove(found);
                                            }
                                        }

                                        if (newChilds.Count > 0)
                                        {
                                            PO_SellDetail child = null;
                                            foreach (var item in newChilds)
                                            {
                                                if (item.ProductId != 0)
                                                {
                                                    child = new PO_SellDetail();
                                                    Parse.CopyObject(item, ref child);
                                                    child.CreatedDate = obj.CreatedDate;
                                                    child.CreatedUser = obj.CreatedUser;
                                                    child.POId = obj.Id;
                                                    obj.PO_SellDetail.Add(child);
                                                }
                                            }
                                        }
                                    }
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

        private bool CheckExists(string name, int? id)
        {
            try
            {
                PO_Sell obj = null;
                obj = db.PO_Sell.FirstOrDefault(x => !x.IsDeleted && x.Code.Trim().ToUpper().Equals(name) && x.Id != id);

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
                    var obj = db.PO_Sell.FirstOrDefault(x => !x.IsDeleted && x.Id == id);
                    if (obj == null)
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Delete ", Message = "Phiếu bạn đang thao tác đã bị xóa hoặc không tồn tại. Vui lòng kiểm tra lại !." });
                    }
                    else
                    {
                        if (!checkPermis(obj, acctionUserId, isOwner))
                        {
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "Delete", Message = "Bạn không phải là người tạo Phiếu này nên bạn không xóa được Phiếu này." });
                        }
                        else
                        {
                            obj.IsDeleted = true;
                            obj.DeletedUser = acctionUserId;
                            obj.DeletedDate = DateTime.Now;

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

        public List<ModelSelectItem> GetSelectItem()
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var listModelSelect = new List<ModelSelectItem>();
                    var productTypes = db.PO_Sell.Where(x => !x.IsDeleted && !x.Customer.IsDeleted && x.StatusId == (int)eStatus.Approved).Select(
                        x => new ModelSelectItem()
                        {
                            Value = x.Id,
                            Code = x.Code,
                            Name = x.Customer.Name
                        }).ToList();

                    if (productTypes != null && productTypes.Count() > 0)
                    {
                        listModelSelect.Add(new ModelSelectItem() { Value = 0, Name = " --  Chọn phiếu báo giá  -- " });
                        listModelSelect.AddRange(productTypes);
                    }
                    else
                        listModelSelect.Add(new ModelSelectItem() { Value = 0, Name = "  Không có phiếu báo giá  " });
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
