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
    public class BLLOrder
    {
        #region constructor
        SanXuatCheckListEntities db;
        static object key = new object();
        private static volatile BLLOrder _Instance;
        public static BLLOrder Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLOrder();

                return _Instance;
            }
        }
        private BLLOrder() { }
        #endregion

        bool checkPermis(Order obj, int actionUser, bool isOwner)
        {
            if (isOwner) return true;
            return obj.CreatedUser == actionUser;
        }

        public PagedList<OrderModel> GetList(string keyWord, int startIndexRecord, int pageSize, string sorting)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    if (string.IsNullOrEmpty(sorting))
                        sorting = "CreatedDate DESC";

                    IQueryable<Order> Orders = db.Order.Where(x => !x.IsDeleted && !x.Customer.IsDeleted);
                    if (!string.IsNullOrEmpty(keyWord))
                        Orders = Orders.Where(x => x.Code.Trim().ToUpper().Contains(keyWord.Trim().ToUpper()));

                    var pageNumber = (startIndexRecord / pageSize) + 1;
                    var objs = new PagedList<OrderModel>(Orders.OrderBy(sorting).Select(x => new OrderModel()
                    {
                        Id = x.Id,
                        FromPOId = x.FromPOId,
                        Code = x.Code,
                        CustomerId = x.CustomerId,
                        CustomerName = x.Customer.Name,
                        ContactName = x.ContactName,
                        Email = x.Email,
                        Phone = x.Phone,
                        Address = x.Address,
                        MoneyUnitId = x.MoneyUnitId,
                        MoneyTypeName = x.Unit.Name,
                        DeliveryDate = x.DeliveryDate,
                        Exchange = x.Exchange,
                        Note = x.Note,
                        StatusId = x.StatusId,
                        StatusName = x.Status.Name
                    }).ToList(), pageNumber, pageSize);
                    if (objs.Count > 0)
                    {
                        var ids = objs.Select(x => x.Id).ToArray();
                        var details = db.OrderDetail
                            .Where(x => !x.IsDeleted && !x.Product.IsDeleted && ids.Contains(x.OrderId))
                            .Select(x => new { OrderId = x.OrderId, Quantity = x.Quantities, Price = x.Price })
                            .ToList();
                        for (int i = 0; i < objs.Count; i++)
                        {
                            objs[i].Total = details.Where(x => x.OrderId == objs[i].Id).Sum(x => x.Quantity * x.Price);
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

        public OrderModel Get(int poId)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var obj = db.Order
                        .Where(x => !x.IsDeleted && !x.Customer.IsDeleted && x.Id == poId)
                     .Select(x => new OrderModel()
                     {
                         Id = x.Id,
                         Code = x.Code,
                         CustomerId = x.CustomerId,
                         CustomerName = x.Customer.Name,
                         ContactName = x.ContactName,
                         Email = x.Email,
                         Address = x.Address,
                         Phone = x.Phone,
                         MoneyUnitId = x.MoneyUnitId,
                         MoneyTypeName = x.Unit.Name,
                         DeliveryDate = x.DeliveryDate,
                         Exchange = x.Exchange,
                         Note = x.Note,
                         StatusId = x.StatusId,
                         StatusName = x.Status.Name
                     }).FirstOrDefault();

                    var details = db.OrderDetail
                        .Where(x => !x.IsDeleted && !x.Product.IsDeleted && x.OrderId == poId)
                        .Select(x => new OrderDetailModel { Id = x.Id, OrderId = x.OrderId, Quantities = x.Quantities, Price = x.Price, ProductId = x.ProductId, ProductName = x.Product.Name, ProductUnit = x.Product.Unit.Name })
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

        public ResponseBase InsertOrUpdate(OrderModel model, bool isOwner)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var result = new ResponseBase();
                    if (CheckExists(model))
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
                        Order obj = db.Order.FirstOrDefault(x => !x.IsDeleted && x.FromPOId == model.FromPOId && x.Id != model.Id);
                        if (obj != null)
                        {
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "Insert ", Message = "Đã có đơn hàng được tạo từ phiếu báo giá này. Vui lòng chọn phiếu báo giá khác." });
                            return result;
                        }
                        else
                        {
                            if (model.Id == 0)
                            {
                                obj = new Order();
                                Parse.CopyObject(model, ref obj);
                                obj.Code = model.Code;
                                obj.CreatedDate = DateTime.Now;
                                obj.CreatedUser = model.ActionUser;
                                if (obj.FromPOId == 0)
                                     obj.FromPOId = null;

                                if (model.Details != null && model.Details.Count > 1)
                                {
                                    obj.OrderDetail = new List<OrderDetail>();
                                    OrderDetail child = null;
                                    foreach (var item in model.Details)
                                    {
                                        if (item.ProductId != 0)
                                        {
                                            child = new OrderDetail();
                                            Parse.CopyObject(item, ref child);
                                            child.CreatedDate = obj.CreatedDate;
                                            child.CreatedUser = obj.CreatedUser;
                                            child.Order = obj;
                                            obj.OrderDetail.Add(child);
                                        }
                                    }
                                }
                                db.Order.Add(obj);
                                db.SaveChanges();
                                result.IsSuccess = true;
                            }
                            else
                            {
                                obj = db.Order.FirstOrDefault(x => !x.IsDeleted && x.Id == model.Id);
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
                                        obj.Email = model.Email;
                                        obj.Address = model.Address;
                                        obj.ContactName = model.ContactName;
                                        obj.CustomerId = model.CustomerId;
                                        obj.DeliveryDate = model.DeliveryDate;
                                        obj.StatusId = model.StatusId;
                                        obj.MoneyUnitId = model.MoneyUnitId;
                                        obj.Exchange = model.Exchange;
                                        obj.Note = model.Note;
                                        obj.UpdatedUser = model.ActionUser;
                                        obj.UpdatedDate = DateTime.Now;

                                        var _details = db.OrderDetail.Where(x => x.OrderId == obj.Id);
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
                                                OrderDetail child = null;
                                                foreach (var item in newChilds)
                                                {
                                                    if (item.ProductId != 0)
                                                    {
                                                        child = new OrderDetail();
                                                        Parse.CopyObject(item, ref child);
                                                        child.CreatedDate = obj.CreatedDate;
                                                        child.CreatedUser = obj.CreatedUser;
                                                        child.OrderId = obj.Id;
                                                        obj.OrderDetail.Add(child);
                                                    }
                                                }
                                            }
                                        }
                                        db.SaveChanges();
                                        result.IsSuccess = true;
                                    }
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

        private bool CheckExists(Order model)
        {
            try
            {
                Order obj = db.Order.FirstOrDefault(x => !x.IsDeleted && x.Code.Trim().ToUpper().Equals(model.Code.Trim().ToUpper()) && x.Id != model.Id);

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
                    var obj = db.Order.FirstOrDefault(x => !x.IsDeleted && x.Id == id);
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
                    var productTypes = db.Order.Where(x => !x.IsDeleted).Select(
                        x => new ModelSelectItem()
                        {
                            Value = x.Id,
                            Name = x.Code
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
