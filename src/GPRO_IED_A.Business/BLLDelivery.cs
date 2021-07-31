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
    public class BLLDelivery
    {
        #region constructor
        SanXuatCheckListEntities db;
        static object key = new object();
        private static volatile BLLDelivery _Instance;
        public static BLLDelivery Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLDelivery();

                return _Instance;
            }
        }
        private BLLDelivery() { }
        #endregion

        private bool CheckExists(int Id, string value, int index, bool isCheckName)
        {
            Delivery obj;
            if (!isCheckName)
                obj = db.Delivery.FirstOrDefault(x => !x.IsDeleted && x.Id != Id && x.Index == index);
            else
                obj = db.Delivery.FirstOrDefault(x => !x.IsDeleted && x.Id != Id && x.Name.Trim().Equals(value));
            return obj != null ? true : false;
        }

        public Delivery GetById(int Id)
        {
            if (db != null)
                return db.Delivery.FirstOrDefault(x => !x.IsDeleted && x.Id == Id);
            else
                using (db = new SanXuatCheckListEntities())
                    return db.Delivery.FirstOrDefault(x => !x.IsDeleted && x.Id == Id);
        }

        public ResponseBase CreateOrUpdate( DeliveryModel model)
        {
            using (db = new SanXuatCheckListEntities())
            {
                var result = new ResponseBase();
                result.IsSuccess = false;
                try
                {
                    if (!CheckExists(model.Id, model.Name, model.Index, true)) // nếu ko bị trùng tên
                    {
                        if (!CheckExists(model.Id, model.Name, model.Index, false)) // nếu ko bị trùng index
                            result.IsSuccess = true;
                        else
                        {
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "Create", Message = "Mã phiếu xuất kho đã bị trùng, Xin chọn mã khác" });
                        }
                        if (result.IsSuccess)
                        {
                            Delivery obj;
                            if (model.Id == 0)  // tao moi
                            {
                                obj = new Delivery();
                                Parse.CopyObject(model, ref obj);
                                obj.CreatedUser = model.ActionUser;
                                obj.CreatedDate = DateTime.Now;
                                db.Delivery.Add(obj);
                                result.IsSuccess = true;
                            }
                            else // cập nhật
                            {
                                obj = GetById(model.Id);
                                if (obj != null)
                                {

                                    obj.Name = model.Name;
                                    obj.Index = model.Index;
                                    obj.WarehouseId = model.WarehouseId;
                                    obj.Deliverier = model.Deliverier;
                                    obj.DeliveryDate = model.DeliveryDate;
                                    obj.CustomerId = model.CustomerId;
                                    obj.Reciever = model.Reciever;
                                    obj.UnitId = model.UnitId;
                                    obj.ExchangeRate = model.ExchangeRate;
                                    obj.TransactionType = model.TransactionType;
                                    obj.DateOfAccounting = model.DateOfAccounting;
                                    obj.IsApproved = model.IsApproved;
                                    if (model.IsApproved)
                                    {
                                        obj.ApprovedUser = model.ActionUser;
                                        obj.ApprovedDate = DateTime.Now;
                                    }
                                    else
                                    {
                                        obj.ApprovedUser = null;
                                        obj.ApprovedDate = null;
                                    }
                                    obj.Note = model.Note;
                                    obj.UpdatedDate = DateTime.Now;
                                    obj.UpdatedUser = model.ActionUser;
                                    result.IsSuccess = true;
                                }
                                else
                                {
                                    result.IsSuccess = false;
                                    result.Errors.Add(new Error() { MemberName = "Create", Message = "Phiếu Xuất Kho không tồn tại hoặc đã bị xóa, Vui Lòng kiểm tra lại." });
                                }
                            }
                            if (result.IsSuccess)
                            {
                                db.SaveChanges();
                                result.IsSuccess = true;
                            }
                        }
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Create", Message = "Tên Phiếu Đã Tồn Tại, Vui Lòng Chọn Tên Khác" });
                    }
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Errors.Add(new Error() { MemberName = "Create", Message = "Lỗi khi thực hiện SQL, Vui Lòng kiểm tra lại." });

                }
                return result;
            }

        }

        public ResponseBase Delete(int Id, int actionUserId)
        {
            using (db = new SanXuatCheckListEntities())
            {
                var rs = new ResponseBase();
                try
                {
                    var obj = GetById(Id);
                    if (obj != null)
                    {
                        obj.IsDeleted = true;
                        obj.DeletedDate = DateTime.Now;
                        obj.DeletedUser = actionUserId;
                        db.SaveChanges();
                        rs.IsSuccess = true;
                    }
                    else
                    {
                        rs.IsSuccess = false;
                        rs.Errors.Add(new Error() { MemberName = "Delete", Message = "Phiếu Xuất Kho này không tồn tại hoặc đã bị xóa, Vui Lòng kiểm tra lại." });
                    }
                }
                catch (Exception ex)
                {
                    rs.IsSuccess = false;
                    rs.Errors.Add(new Error() { MemberName = "Delete", Message = "Lỗi Exception" });
                }
                return rs;
            }
        }

        public PagedList<DeliveryModel> GetList(string keyword, int startIndexRecord, int pageSize, string sorting)
        {
            using (db = new SanXuatCheckListEntities())
            {
                try
                {
                    if (string.IsNullOrEmpty(sorting))
                        sorting = "CreatedDate DESC";

                    IQueryable<Delivery> objs = db.Delivery.Where(c => !c.IsDeleted);
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        keyword = keyword.Trim().ToUpper();
                        objs = objs.Where(c => c.Name.Trim().ToUpper().Contains(keyword));
                    }
                    var pageNumber = (startIndexRecord / pageSize) + 1;
                    var pagelist = new PagedList<DeliveryModel>(objs.OrderBy(sorting).Select(x => new DeliveryModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Index = x.Index,
                        CustomerId = x.CustomerId,
                        strCustomer = x.Customer.Name,
                        Reciever = x.Reciever,
                        UnitId = x.UnitId,
                        TienTe = x.Unit.Name,
                        WarehouseId = x.WarehouseId,
                        strWarehouse = x.WareHouse.Name,
                        ExchangeRate = x.ExchangeRate,
                        TransactionType = x.TransactionType,
                        DateOfAccounting = x.DateOfAccounting,
                        ApprovedUser = x.ApprovedUser,
                        IsApproved = x.IsApproved,
                        ApprovedDate = x.ApprovedDate,
                        Note = x.Note,
                        DeliveryDate = x.DeliveryDate,
                        Deliverier = x.Deliverier
                    }), pageNumber, pageSize);
                    if (pagelist.Count > 0)
                    {
                        var ids = pagelist.Select(x => x.Id);
                        var details = db.DeliveryDetail.Where(x => !x.IsDeleted && ids.Contains(x.DeliveryId));

                        string dvalue = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.Delivery);
                        var employees = BLLEmployee.Instance.GetSelectItem();
                        var users = BLLUser.Instance.GetSelectItem(null);
                        ModelSelectItem found;
                        foreach (var item in pagelist)
                        {
                            var dts = details.Where(x => x.DeliveryId == item.Id);
                            item.Total = (dts != null && dts.Count() > 0 ? Math.Round(dts.Sum(x => x.Price * x.Quantity), 2) : 0);
                            item.Code = dvalue + item.Index;

                            found = employees.FirstOrDefault(x => x.Value == item.Deliverier);
                            if (found != null)
                                item.strDeliverier = found.Name;
                            if (item.ApprovedUser.HasValue)
                            {
                                found = users.FirstOrDefault(x => x.Value == item.ApprovedUser.Value);
                                if (found != null)
                                    item.strApprover = found.Name;
                            }
                        }
                    }
                    return pagelist;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public PagedList<DeliveryModel> GetList(int custId, int startIndexRecord, int pageSize, string sorting)
        {
            using (db = new SanXuatCheckListEntities())
            {
                try
                {
                    if (string.IsNullOrEmpty(sorting))
                        sorting = "CreatedDate DESC";

                    IQueryable<Delivery> objs = db.Delivery.Where(c => !c.IsDeleted );
                    if (custId != 0)
                        objs = objs.Where(c => c.CustomerId == custId);

                    var pageNumber = (startIndexRecord / pageSize) + 1;
                    var pagelist = new PagedList<DeliveryModel>(objs.OrderBy(sorting).Select(x => new DeliveryModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Index = x.Index,
                        CustomerId = x.CustomerId,
                        strCustomer = x.Customer.Name,
                        Reciever = x.Reciever,
                        UnitId = x.UnitId,
                        TienTe = x.Unit.Name,
                        WarehouseId = x.WarehouseId,
                        strWarehouse = x.WareHouse.Name,
                        ExchangeRate = x.ExchangeRate,
                        TransactionType = x.TransactionType,
                        DateOfAccounting = x.DateOfAccounting,
                        ApprovedUser = x.ApprovedUser,
                        IsApproved = x.IsApproved,
                        ApprovedDate = x.ApprovedDate,
                        Note = x.Note,
                        DeliveryDate = x.DeliveryDate,
                        Deliverier = x.Deliverier
                    }), pageNumber, pageSize);
                    if (pagelist.Count > 0)
                    {
                        var ids = pagelist.Select(x => x.Id);
                        var details = db.DeliveryDetail.Where(x => !x.IsDeleted && ids.Contains(x.DeliveryId));

                        string dvalue = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.Delivery);
                        var employees = BLLEmployee.Instance.GetSelectItem();
                        var users = BLLUser.Instance.GetSelectItem(null);
                        ModelSelectItem found;
                        foreach (var item in pagelist)
                        {
                            var dts = details.Where(x => x.DeliveryId == item.Id);
                            item.Total = (dts != null && dts.Count() > 0 ? Math.Round(dts.Sum(x => x.Price * x.Quantity), 2) : 0);
                            item.Code = dvalue + item.Index;

                            found = employees.FirstOrDefault(x => x.Value == item.Deliverier);
                            if (found != null)
                                item.strDeliverier = found.Name;
                            if (item.ApprovedUser.HasValue)
                            {
                                found = users.FirstOrDefault(x => x.Value == item.ApprovedUser.Value);
                                if (found != null)
                                    item.strApprover = found.Name;
                            }
                        }
                    }
                    return pagelist;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        public List<ModelSelectItem> GetSelectList()
        {
            using (db = new SanXuatCheckListEntities())
            {
                return db.Delivery.Where(x => !x.IsDeleted).Select(x => new ModelSelectItem() { Value = x.Id, Name = x.Name }).ToList();
            }
        }

        public int GetLastIndex()
        {
            using (db = new SanXuatCheckListEntities())
            {
                var obj = db.Delivery.OrderByDescending(x => x.Index).FirstOrDefault(x => !x.IsDeleted);
                return obj != null ? obj.Index : 0;
            }
        }
    }
}
