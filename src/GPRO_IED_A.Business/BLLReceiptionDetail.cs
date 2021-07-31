using GPRO.Core.Mvc;
using GPRO.Ultilities;
using Hugate.Framework;
using PagedList;
using SanXuatCheckList.Business.Enum;
using SanXuatCheckList.Business.Model;
using SanXuatCheckList.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SanXuatCheckList.Business
{
    public class BLLReceiptionDetail
    {
        #region constructor
        SanXuatCheckListEntities db;
        static object key = new object();
        private static volatile BLLReceiptionDetail _Instance;
        public static BLLReceiptionDetail Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLReceiptionDetail();

                return _Instance;
            }
        }
        private BLLReceiptionDetail() { }
        #endregion

        bool checkPermis(ReceiptionDetail obj, int actionUser, bool isOwner)
        {
            if (isOwner) return true;
            return obj.CreatedUser == actionUser;
        }

        public ResponseBase CreateOrUpdate(ReceiptionDetailModel model)
        {
            using (db = new SanXuatCheckListEntities())
            {
                var rs = new ResponseBase();
                rs.IsSuccess = false;
                try
                {
                    ReceiptionDetail detailObj;
                    LotSupplies lotSuplies;
                    if (model.Id == 0)
                    {
                        lotSuplies = new LotSupplies();
                        Parse.CopyObject(model, ref lotSuplies);
                        lotSuplies.ReceiptionDetail = new Collection<ReceiptionDetail>();

                        detailObj = new ReceiptionDetail();
                        detailObj.ReceiptionId = model.ReceiptionId;
                        detailObj.LotSupplies = lotSuplies;
                        detailObj.CreatedDate = model.CreatedDate;
                        detailObj.CreatedUser = model.CreatedUser;
                        lotSuplies.ReceiptionDetail.Add(detailObj);
                        db.LotSupplies.Add(lotSuplies);
                        db.SaveChanges();
                        rs.IsSuccess = true;
                    }
                    else // cập nhật
                    {
                        detailObj = GetById(model.Id);
                        if (detailObj != null)
                        {
                            var isApprove = db.ReceiptionDetail.Where(x => x.Id == detailObj.Id).Select(x => x.Receiption.StatusId == (int)eStatus.Approved).FirstOrDefault();
                            if (isApprove)
                            {
                                rs.IsSuccess = false;
                                rs.Errors.Add(new Error() { MemberName = "Create", Message = "Phiếu Nhập Kho này đã được duyệt Bạn sẽ không thể thay đổi thông tin được nữa." });
                            }
                            else
                            {
                                var lotObj = db.LotSupplies.FirstOrDefault(x=> !x.IsDeleted && x.Id == detailObj.LotSuppliesId) ;
                                if (lotObj != null)
                                {
                                    lotObj.Name = model.Name;
                                    lotObj.Index = model.Index;
                                    lotObj.MaterialId = model.MaterialId;
                                    //lotObj.WareHouseId = model.WareHouseId;
                                    lotObj.Quantity = model.Quantity;
                                    lotObj.Price = model.Price;
                                    //lotObj.InputDate = model.InputDate;
                                    lotObj.ManufactureDate = model.ManufactureDate;
                                    lotObj.ExpiryDate = model.ExpiryDate;
                                    lotObj.WarrantyDate = model.WarrantyDate;
                                    //lotObj.StatusId = model.StatusId;
                                    lotObj.Note = model.Note;
                                    lotObj.UpdatedDate = model.UpdatedDate;
                                    lotObj.UpdatedUser = model.UpdatedUser;

                                    rs.IsSuccess = true;
                                }
                            }
                        }
                        else
                        {
                            rs.IsSuccess = false;
                            rs.Errors.Add(new Error() { MemberName = "Create", Message = "Phiếu Nhập Kho Chi Tiết này không tồn tại hoặc đã bị xóa, Vui Lòng kiểm tra lại." });
                        }
                    }
                    if (rs.IsSuccess)
                    {
                        db.SaveChanges();
                        rs.IsSuccess = true;
                    }
                    else
                    {
                        rs.IsSuccess = false;
                        rs.Errors.Add(new Error() { MemberName = "Create", Message = "Lỗi khi thực hiện SQL, Vui Lòng kiểm tra lại." });
                    }
                }
                catch (Exception ex)
                {
                    rs.IsSuccess = false;
                    rs.Errors.Add(new Error() { MemberName = "Create", Message = "Lỗi Exception" });
                }
                return rs;
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
                        var isApproved = db.ReceiptionDetail.Where(x => x.Id == obj.Id).Select(x => x.Receiption.StatusId == (int)eStatus.Approved).FirstOrDefault();
                        if (isApproved)
                        {
                            rs.IsSuccess = false;
                            rs.Errors.Add(new Error() { MemberName = "Delete", Message = "Phiếu Nhập Kho này đã được duyệt. Bạn không thể xóa nó" });
                        }
                        else
                        {
                            obj.IsDeleted = true;
                            obj.DeletedDate = DateTime.Now;
                            obj.DeletedUser = actionUserId;
                            if (obj.LotSupplies!= null)
                            {
                                obj.LotSupplies.IsDeleted = true;
                                obj.LotSupplies.DeletedDate = obj.DeletedDate;
                                obj.LotSupplies.DeletedUser = actionUserId;
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

        public PagedList<ReceiptionDetailModel> GetList(int recordId, int startIndexRecord, int pageSize, string sorting)
        {
            using (db = new SanXuatCheckListEntities())
            {
                try
                {
                    if (string.IsNullOrEmpty(sorting))
                        sorting = "CreatedDate DESC";
                    var objs = db.ReceiptionDetail.Where(c => !c.IsDeleted && c.ReceiptionId == recordId);
                    var pageNumber = (startIndexRecord / pageSize) + 1;

                    var pagedList = new PagedList<ReceiptionDetailModel>(objs
                        .Select(x => new ReceiptionDetailModel()
                        {
                            Id = x.Id,
                            ReceiptionId = x.ReceiptionId,
                            LotSuppliesId = x.LotSuppliesId,

                            Name = x.LotSupplies.Name,
                            Index = x.LotSupplies.Index,

                            MaterialId = x.LotSupplies.MaterialId,
                            MaterialName = x.LotSupplies.Material.NameTM,
                            MaterialIndex = x.LotSupplies.Material.Index,

                            CustomerId = x.Receiption.FromCustomerId.Value,
                            CustomerName = x.Receiption.Customer.Name,
                            CustomerIndex = x.Receiption.Customer.Index,

                            WareHouseId = x.Receiption.StoreWarehouseId,
                            WarehouseName = x.Receiption.WareHouse.Name,
                            //WarehouseIndex = x.Receiption.WareHouse.Index,

                            Quantity = x.LotSupplies.Quantity,
                            QuantityUsed = x.LotSupplies.QuantityUsed,
                            UnitId = x.LotSupplies.Material.UnitId,
                            UnitName = x.LotSupplies.Material.Unit.Name,

                            Price = x.LotSupplies.Price,
                            ExchangeRate = x.Receiption.ExchangeRate,
                            MoneyTypeId = x.Receiption.MoneyTypeId,
                            MoneyTypeName = x.Receiption.Unit.Name,

                            //InputDate = x.Receiption.InputDate,
                            ManufactureDate = x.LotSupplies.ManufactureDate,
                            ExpiryDate = x.LotSupplies.ExpiryDate,
                            WarrantyDate = x.LotSupplies.WarrantyDate,

                            StatusId = x.Receiption.StatusId,
                            Note = x.LotSupplies.Note,
                            CreatedDate = x.CreatedDate
                        }).OrderBy(sorting) .ToList(), pageNumber, pageSize);
                    if (pagedList.Count > 0)
                    {
                        string whvalue = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.WareHouse);
                        string cusvalue = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.Customer);
                        string mvalue = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.Material);
                        string lotvalue = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.LotSupplies);

                        foreach (var item in pagedList)
                        {
                            item.Code = lotvalue + item.Index;
                            item.MaterialName = "<span >" + item.MaterialName + "</span> (<span class=\"red\">" + mvalue + item.MaterialIndex + "</span>)";
                            item.CustomerName = "<span >" + item.CustomerName + "</span> (<span class=\"red\">" + cusvalue + item.CustomerIndex + "</span>)";
                            item.WarehouseName = "<span >" + item.WarehouseName + "</span> (<span class=\"red\">" + whvalue + item.WarehouseIndex + "</span>)";
                        }
                    }
                    return pagedList;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private bool CheckExists(int Id, int rId)
        {
            ReceiptionDetail obj;
            obj = db.ReceiptionDetail.FirstOrDefault(x => !x.IsDeleted && x.Id != Id && x.ReceiptionId == rId);
            return obj != null ? true : false;
        }

        public ReceiptionDetail GetById(int Id)
        {
            if (db != null)
                return db.ReceiptionDetail.FirstOrDefault(x => !x.IsDeleted && x.Id == Id);
            else
                using (db = new SanXuatCheckListEntities())
                    return db.ReceiptionDetail.FirstOrDefault(x => !x.IsDeleted && x.Id == Id);
        }

        public List<ReceiptionDetailModel> GetReceiptionDetails(int receiptionId)
        {
            using (db = new SanXuatCheckListEntities())
            {
                try
                {
                    var listObjs = db.ReceiptionDetail.Where(x => !x.IsDeleted && !x.Receiption.IsDeleted && x.ReceiptionId == receiptionId).Select(x => new ReceiptionDetailModel()
                    {
                        Id = x.Id,
                        ReceiptionId = x.ReceiptionId,
                        ReceiptionName = x.Receiption.Name,
                        ReceiptionIndex = x.Receiption.Index,

                        LotSuppliesId = x.LotSuppliesId,

                        Name = x.LotSupplies.Name,
                        Index = x.LotSupplies.Index,
                        MaterialId = x.LotSupplies.MaterialId,
                        MaterialName = x.LotSupplies.Material.NameTM,
                        MaterialIndex = x.LotSupplies.Material.Index,

                        CustomerId = x.Receiption.FromCustomerId.Value,
                        CustomerName = x.Receiption.Customer.Name,
                        CustomerIndex = x.Receiption.Customer.Index,

                        WareHouseId = x.Receiption.WareHouse1.Id,
                        WarehouseName = x.Receiption.WareHouse1.Name,
                        WarehouseIndex = x.Receiption.WareHouse1.Index,

                        Quantity = x.LotSupplies.Quantity,
                        QuantityUsed = x.LotSupplies.QuantityUsed,
                        Price = x.LotSupplies.Price,
                        InputDate = x.Receiption.InputDate,
                        ManufactureDate = x.LotSupplies.ManufactureDate,
                        ExpiryDate = x.LotSupplies.ExpiryDate,
                        WarrantyDate = x.LotSupplies.WarrantyDate,
                        MoneyTypeId = x.Receiption.MoneyTypeId,
                        MoneyTypeName = x.Receiption.Unit.Name,
                        ExchangeRate = x.Receiption.ExchangeRate,
                        UnitId = x.LotSupplies.Material.UnitId,
                        StatusId = x.Receiption.StatusId,
                        Receiver = x.Receiption.RecieverId,
                        ApprovedUser = x.Receiption.ApprovedUser,
                        ApprovedDate = x.Receiption.ApprovedDate,
                        Note = x.LotSupplies.Note,
                        CreatedDate = x.CreatedDate,
                        Total = x.LotSupplies.Quantity * x.LotSupplies.Price,
                        UnitInStock = x.LotSupplies.Quantity - x.LotSupplies.QuantityUsed
                    }).ToList();

                    if (listObjs.Count > 0)
                    {
                        string whvalue = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.WareHouse);
                        string cusvalue = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.Customer);
                        string mvalue = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.Material);
                        string lotvalue = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.LotSupplies);
                        string rvalue = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.Receiption);

                        foreach (var item in listObjs)
                        {
                            item.ReceiptionCode = rvalue + item.ReceiptionIndex;
                            item.Code = lotvalue + item.Index;
                            item.Name = item.Name + "(" + item.Code + ")";
                            item.MaterialCode = mvalue + item.MaterialIndex;
                            item.WarehouseName = item.WarehouseName + "(" + whvalue + item.WarehouseIndex + ")";
                        }
                    }

                    return listObjs;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

    }
}
