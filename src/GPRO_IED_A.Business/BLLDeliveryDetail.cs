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
    public class BLLDeliveryDetail
    {
        #region constructor
        SanXuatCheckListEntities db;
        static object key = new object();
        private static volatile BLLDeliveryDetail _Instance;
        public static BLLDeliveryDetail Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLDeliveryDetail();

                return _Instance;
            }
        }
        private BLLDeliveryDetail() { }
        #endregion

        public ResponseBase CreateOrUpdate(DeliveryDetail model)
        {
            using (db = new SanXuatCheckListEntities())
            {
                var result = new ResponseBase();
                result.IsSuccess = false;
                try
                {
                    DeliveryDetail obj;
                    //ktra xem có lô trước đó chưa
                    obj = db.DeliveryDetail.FirstOrDefault(x => !x.IsDeleted && x.LotSupliesId == model.LotSupliesId);
                    if (model.Id == 0 && obj != null)
                    {
                        model.Id = obj.Id;
                        model.Quantity += obj.Quantity;
                    }

                    var lotObj = db.LotSupplies.FirstOrDefault(x => !x.IsDeleted && x.Id == model.LotSupliesId);
                    if (model.Id == 0)
                    {
                        #region insert
                        if ((lotObj.QuantityUsed + model.Quantity) > lotObj.Quantity)
                        {
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "Create", Message = "Số lượng bạn chọn vượt quá số lượng tồn kho. Vui lòng kiểm tra lại" });
                        }
                        else
                        {
                            obj = new DeliveryDetail();
                            Parse.CopyObject(model, ref obj);
                            db.DeliveryDetail.Add(obj);

                            if (lotObj != null)
                            {
                                lotObj.QuantityUsed += model.Quantity;
                            }
                            result.IsSuccess = true;
                        }
                        #endregion
                    }
                    else
                    {
                        #region cập nhật
                        obj = GetById(model.Id);
                        if (obj != null)
                        {
                            var isApproved = db.DeliveryDetail.Where(x => x.Id == obj.Id).Select(x => x.Delivery.IsApproved).FirstOrDefault();
                            if (isApproved)
                            {
                                result.IsSuccess = false;
                                result.Errors.Add(new Error() { MemberName = "Create", Message = "Phiếu Xuất Kho này đã được duyệt Bạn sẽ không thể thay đổi thông tin được nữa." });
                            }
                            else
                            {
                                int oldSL = obj.Quantity;
                                if (((lotObj.QuantityUsed - oldSL) + model.Quantity) > lotObj.Quantity)
                                {
                                    result.IsSuccess = false;
                                    result.Errors.Add(new Error() { MemberName = "Create", Message = "Số lượng bạn chọn vượt quá số lượng tồn kho. Vui lòng kiểm tra lại" });
                                }
                                else
                                {
                                    obj.DeliveryId = model.DeliveryId;
                                    obj.LotSupliesId = model.LotSupliesId;
                                    obj.Quantity = model.Quantity;
                                    obj.Price = model.Price;
                                    obj.UpdatedDate = model.UpdatedDate;
                                    obj.UpdatedUser = model.UpdatedUser;

                                    if (lotObj != null)
                                    {
                                        lotObj.QuantityUsed -= oldSL;
                                        lotObj.QuantityUsed += model.Quantity;
                                    }
                                    result.IsSuccess = true;
                                }
                            }
                        }
                        else
                        {
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "Create", Message = "Phiếu Xuất Kho này không tồn tại hoặc đã bị xóa, Vui Lòng kiểm tra lại." });
                        }
                        #endregion
                    }

                    if (result.IsSuccess)
                    {
                        db.SaveChanges();
                        result.IsSuccess = true;
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
                        var isApproved = db.DeliveryDetail.Where(x => x.Id == obj.Id).Select(x => x.Delivery.IsApproved).FirstOrDefault();
                        if (isApproved)
                        {
                            rs.IsSuccess = false;
                            rs.Errors.Add(new Error() { MemberName = "Delete", Message = "Phiếu Nhập Kho này đã được duyệt. Bạn không thể thay đổi hoặc xóa thông tin của phiếu." });
                        }
                        else
                        {
                            obj.IsDeleted = true;
                            obj.DeletedDate = DateTime.Now;
                            obj.DeletedUser = actionUserId;

                            var lotsupliesObj = db.LotSupplies.FirstOrDefault(x => !x.IsDeleted && x.Id == obj.LotSupliesId);
                            if (lotsupliesObj != null)
                            {
                                lotsupliesObj.QuantityUsed -= obj.Quantity;
                                lotsupliesObj.DeletedDate = DateTime.Now;
                                lotsupliesObj.DeletedUser = actionUserId;
                            }
                            db.SaveChanges();
                            rs.IsSuccess = true;
                        }
                    }
                    else
                    {
                        rs.IsSuccess = false;
                        rs.Errors.Add(new Error() { MemberName = "Delete", Message = "Phiếu Nhập Kho Chi Tiết này không tồn tại hoặc đã bị xóa, Vui Lòng kiểm tra lại." });
                    }
                }
                catch (Exception)
                {
                    rs.IsSuccess = false;
                    rs.Errors.Add(new Error() { MemberName = "Delete", Message = "Lỗi Exception" });
                }
                return rs;
            }
        }

        public PagedList<DeliveryDetailModel> GetList(int recordId, int startIndexRecord, int pageSize, string sorting)
        {
            using (db = new SanXuatCheckListEntities())
            {
                try
                {
                    if (string.IsNullOrEmpty(sorting))
                        sorting = "CreatedDate DESC";
                    var objs = db.DeliveryDetail
                        .Where(c => !c.IsDeleted && c.DeliveryId == recordId)
                        .Select(x => new DeliveryDetailModel()
                        {
                            Id = x.Id,
                            DeliveryId = x.DeliveryId,
                            Quantity = x.Quantity,
                            QuantityLo = x.LotSupplies.Quantity,
                            QuantityUsed = x.LotSupplies.QuantityUsed,
                            LotSupliesId = x.LotSupliesId,

                            LotName = x.LotSupplies.Name,
                            LotIndex = x.LotSupplies.Index,

                            MaterialName = x.LotSupplies.Material.NameTM,
                            MaterialIndex = x.LotSupplies.Material.Index,
                            CreatedDate = x.CreatedDate,
                            UnitId = x.LotSupplies.Material.UnitId,
                            UnitName = x.LotSupplies.Material.Unit.Name,
                            // InputDate = x.LotSupplies...InputDate,
                            ExpiryDate = x.LotSupplies.ExpiryDate,
                            Price = x.Price 
                        }).OrderBy(sorting).ToList();

                    var pageNumber = (startIndexRecord / pageSize) + 1;
                    var pagedlist = new PagedList<DeliveryDetailModel>(objs, pageNumber, pageSize);
                    if (pagedlist.Count > 0)
                    {
                        string whvalue = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.WareHouse);
                        string mvalue = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.Material);
                        string lotvalue = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.LotSupplies);

                        foreach (var item in pagedlist)
                        {
                            item.LotName = "<span >" + item.LotName + "</span> (<span class=\"red\">" + lotvalue + item.LotIndex + "</span>)";
                            item.MaterialName = "<span >" + item.MaterialName + "</span> (<span class=\"red\">" + mvalue + item.MaterialIndex + "</span>)";
                        }

                        var lotids = pagedlist.Select(x => x.LotSupliesId).ToList();
                        var lotObjs = db.ReceiptionDetail.Where(x => !x.IsDeleted && lotids.Contains(x.LotSuppliesId)).Select(x => new ReceiptionDetailModel()
                        {
                            LotSuppliesId = x.LotSuppliesId,
                            WarehouseName = x.Receiption.WareHouse1.Name,
                            WarehouseIndex = x.Receiption.WareHouse1.Index,
                            InputDate = x.Receiption.InputDate,
                        }).ToList();

                        if (lotObjs.Count > 0)
                        {
                            foreach (var item in lotObjs)
                            {
                                item.WarehouseName = "<span>" + item.WarehouseName + "</span> (<span class=\"red\">" + whvalue + item.WarehouseIndex + "</span>)";
                            }
                        }
                        ReceiptionDetailModel obj;
                        foreach (var item in pagedlist)
                        {
                            obj = lotObjs.FirstOrDefault(x => x.LotSuppliesId == item.LotSupliesId);
                            item.WareHouseName = obj != null ? obj.WarehouseName : "";
                        }
                    }
                    return pagedlist;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private bool CheckExists(int Id, int dId)
        {
            DeliveryDetail obj;
            obj = db.DeliveryDetail.FirstOrDefault(x => !x.IsDeleted && x.Id != Id && x.DeliveryId == dId);
            return obj != null ? true : false;
        }

        public DeliveryDetail GetById(int Id)
        {
            if (db != null)
                return db.DeliveryDetail.FirstOrDefault(x => !x.IsDeleted && x.Id == Id);
            else
                using (db = new SanXuatCheckListEntities())
                    return db.DeliveryDetail.FirstOrDefault(x => !x.IsDeleted && x.Id == Id);
        }

        public List<DeliveryDetailModel> GetDeliveryDetails(int deliveryId)
        {
            using (db = new SanXuatCheckListEntities())
            {
                var listObjs = db.DeliveryDetail
                    .Where(x => !x.IsDeleted && !x.Delivery.IsDeleted && x.DeliveryId == deliveryId)
                    .Select(x => new DeliveryDetailModel()
                    {
                        Id = x.Id,
                        DeliveryId = x.DeliveryId,
                        Quantity = x.Quantity,
                        QuantityLo = x.LotSupplies.Quantity,
                        QuantityUsed = x.LotSupplies.QuantityUsed,
                        LotSupliesId = x.LotSupliesId,

                        LotName = x.LotSupplies.Name,
                        LotIndex = x.LotSupplies.Index,
                        MaterialName = x.LotSupplies.Material.NameTM,
                        MaterialIndex = x.LotSupplies.Material.Index,

                        CreatedDate = x.CreatedDate,
                        UnitId = x.LotSupplies.Material.UnitId,
                        // InputDate = x.LotSupplies.InputDate,
                        ExpiryDate = x.LotSupplies.ExpiryDate,
                        Price = x.Price,
                        DeliveryName = x.Delivery.Name,

                        DeliveryIndex = x.Delivery.Index,
                        CustomerName = x.Delivery.Customer.Name,
                        ApprovedUser = x.Delivery.ApprovedUser,
                        ApprovedDate = x.Delivery.ApprovedDate,
                        ReceiverName = x.Delivery.Reciever,
                        MoneyTypeId = x.Delivery.UnitId,
                        MoneyTypeName = x.Delivery.Unit.Name,
                        Total = x.Quantity * x.Price
                    }).ToList();

                if (listObjs != null && listObjs.Count > 0)
                {
                    string whvalue = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.WareHouse);
                    string mvalue = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.Material);
                    string lotvalue = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.LotSupplies);
                    string dvalue = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.Delivery);

                    foreach (var item in listObjs)
                    {
                        item.LotName = item.LotName + "(" + lotvalue + item.LotIndex + ")";
                        item.MaterialCode = mvalue + item.MaterialIndex;
                        item.DeliveryCode = dvalue + item.DeliveryIndex;
                    }

                    var lotids = listObjs.Select(x => x.LotSupliesId).ToList();
                    var lotObjs = db.ReceiptionDetail.Where(x => !x.IsDeleted && lotids.Contains(x.LotSuppliesId)).Select(x => new ReceiptionDetailModel()
                    {
                        LotSuppliesId = x.LotSuppliesId,
                        WarehouseName = x.Receiption.WareHouse1.Name,
                        WarehouseIndex = x.Receiption.WareHouse1.Index,
                        InputDate = x.Receiption.InputDate,
                    }).ToList();

                    if (lotObjs.Count > 0)
                    {
                        foreach (var item in lotObjs)
                        {
                            item.WarehouseName = item.WarehouseName + " (" + whvalue + item.WarehouseIndex + ")";
                        }
                    }
                    ReceiptionDetailModel obj;
                    foreach (var item in listObjs)
                    {
                        obj = lotObjs.FirstOrDefault(x => x.LotSuppliesId == item.LotSupliesId);
                        item.WareHouseName = obj != null ? obj.WarehouseName : "";
                    }
                }
                return listObjs;
            }
        }

    }
}
