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
    public class BLLLotSupplies
    {
        #region constructor
        SanXuatCheckListEntities db;
        static object key = new object();
        private static volatile BLLLotSupplies _Instance;
        public static BLLLotSupplies Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLLotSupplies();

                return _Instance;
            }
        }
        private BLLLotSupplies() { }
        #endregion

        public ResponseBase CreateOrUpdate(LotSuppliesModel model)
        {
            using (db = new SanXuatCheckListEntities())
            {
                var result = new ResponseBase();
                result.IsSuccess = false;
                try
                {
                    if (!CheckExists(model.Id, model.Name, model.Index, true)) // nếu ko bị trùng tên                
                    {
                        //switch (model.strStatus)
                        //{
                        //    case "Draft": model.StatusId = (int)eStatus.Draft; break;
                        //    case "Submited": model.StatusId = (int)eStatus.Submited; break;
                        //    case "Approved": model.StatusId = (int)eStatus.Approved; break;
                        //}
                        LotSupplies obj;
                        if (model.Id == 0)  // tạo một khách hàng mới
                        {
                            obj = new LotSupplies();
                            Parse.CopyObject(model, ref obj);

                            db.LotSupplies.Add(obj);
                            result.IsSuccess = true;
                        }
                        else // cập nhật
                        {
                            obj = GetById(model.Id);
                            if (obj != null)
                            {
                                var receipt = db.ReceiptionDetail.FirstOrDefault(x => !x.IsDeleted && x.Receiption.StatusId == (int)eStatus.Approved && x.LotSuppliesId == model.Id);
                                if (receipt != null)
                                {
                                    result.IsSuccess = false;
                                    result.Errors.Add(new Error() { MemberName = "Create", Message = "Lô vật tư này đã được duyệt bạn không được thay đổi thông tin." });
                                }
                                else if (model.Quantity < obj.QuantityUsed)
                                {
                                    result.IsSuccess = false;
                                    result.Errors.Add(new Error() { MemberName = "Create", Message = "Số lượng nhập không được nhỏ hơn số lượng tồn kho. Vui Lòng kiểm tra lại." });
                                }
                                else
                                {
                                    obj.Name = model.Name;
                                    obj.Index = model.Index;
                                    obj.MaterialId = model.MaterialId;
                                    // obj.WareHouseId = model.WareHouseId;
                                    obj.Quantity = model.Quantity;
                                    //obj.MaterialUnitId = model.MaterialUnitId;
                                    obj.Price = model.Price;
                                    //obj.MoneyTypeId = model.MoneyTypeId;
                                    //obj.ExchangeRate = model.ExchangeRate;
                                    //obj.InputDate = model.InputDate;
                                    obj.ManufactureDate = model.ManufactureDate;
                                    obj.ExpiryDate = model.ExpiryDate;
                                    obj.WarrantyDate = model.WarrantyDate;
                                    obj.Note = model.Note;
                                    obj.SpecificationsPaking = model.SpecificationsPaking;
                                    //obj.StatusId = model.StatusId;

                                    obj.UpdatedDate = model.UpdatedDate;
                                    obj.UpdatedUser = model.UpdatedUser;
                                    result.IsSuccess = true;
                                }
                            }
                            else
                            {
                                result.IsSuccess = false;
                                result.Errors.Add(new Error() { MemberName = "Create", Message = "Lô vật tư không tồn tại hoặc đã bị xóa, Vui Lòng kiểm tra lại." });
                            }
                        }
                        if (result.IsSuccess)
                        {
                            db.SaveChanges();
                            result.IsSuccess = true;
                        }
                        else
                        {
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "Create", Message = "Lỗi khi thực hiện SQL, Vui Lòng kiểm tra lại." });
                        }
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Create", Message = "Tên lô vật tư đã tồn tại, Xin chọn tên khác" });
                    }
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Errors.Add(new Error() { MemberName = "Create", Message = "Lỗi Exception" });
                }
                return result;
            }
        }

        private bool CheckExists(int Id, string value, int index, bool isCheckName)
        {
            LotSupplies obj;
            if (!isCheckName)
                obj = db.LotSupplies.FirstOrDefault(x => !x.IsDeleted && x.Id != Id && x.Index == index); // kiem tra index có trùng với 1 kh đã tồn tại ?
            else
                obj = db.LotSupplies.FirstOrDefault(x => !x.IsDeleted && x.Id != Id && x.Name.Trim().Equals(value)); //kiem tra name có giống với 1 kh đã tồn tại ?
            return obj != null ? true : false;
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
                        var now = DateTime.Now;
                        obj.IsDeleted = true;
                        obj.DeletedDate = now;
                        obj.DeletedUser = actionUserId;
                        db.SaveChanges();
                        rs.IsSuccess = true;
                    }
                    else
                    {
                        rs.IsSuccess = false;
                        rs.Errors.Add(new Error() { MemberName = "Create", Message = "Vật tư này không Tồn Tại hoặc đã bị xóa, Vui Lòng kiểm tra lại." });
                    }
                }
                catch (Exception)
                {
                    rs.IsSuccess = false;
                    rs.Errors.Add(new Error() { MemberName = "Create", Message = "Lỗi Exception" });
                }
                return rs;
            }
        }

        public List<ModelSelectItem> GetSelectList()
        {
            using (db = new SanXuatCheckListEntities())
            {
                var cf = "";
                cf = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.LotSupplies);
                return db.LotSupplies.Where(x => !x.IsDeleted).Select(x => new ModelSelectItem() { Value = x.Id, Name = x.Name, Code = cf, Data = x.Index }).ToList();
            }
        }

        public LotSupplies GetById(int Id)
        {
            if (db != null)
                return db.LotSupplies.FirstOrDefault(x => !x.IsDeleted && x.Id == Id);
            else
                using (db = new SanXuatCheckListEntities())
                    return db.LotSupplies.FirstOrDefault(x => !x.IsDeleted && x.Id == Id);
        }

        public List<LotSupplies> GetFullList()
        {
            using (db = new SanXuatCheckListEntities())
            {
                return db.LotSupplies.Where(x => !x.IsDeleted).ToList();
            }
        }

        public int GetLastIndex()
        {
            using (db = new SanXuatCheckListEntities())
            {
                var obj = db.LotSupplies.Where(x => !x.IsDeleted).OrderByDescending(x => x.Index).FirstOrDefault();
                return obj != null ? obj.Index : 0;
            }
        }



        public PagedList<LotSuppliesModel> GetList(bool isForMana, string keyword, int startIndexRecord, int pageSize, string sorting)
        {
            using (db = new SanXuatCheckListEntities())
            {
                try
                {
                    if (string.IsNullOrEmpty(sorting))
                        sorting = "CreatedDate DESC";

                    IQueryable<ReceiptionDetail> objs = db.ReceiptionDetail.Where(c => !c.IsDeleted && !c.LotSupplies.IsDeleted);
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        keyword = keyword.Trim().ToUpper();
                        objs = objs.Where(c => c.LotSupplies.Name.Trim().ToUpper().Contains(keyword));
                    }
                    var pageNumber = (startIndexRecord / pageSize) + 1;

                    var pagedList = new PagedList<LotSuppliesModel>(objs.OrderBy(sorting).Select(x => new LotSuppliesModel()
                    {
                        Id = x.LotSupplies.Id,

                        Name = x.LotSupplies.Name,
                        Index = x.LotSupplies.Index,

                        strCustomer = x.Receiption.Customer.Name,
                        WareHouseId = x.Receiption.WareHouse.Id,
                        strWarehouse = x.Receiption.WareHouse.Name,
                        WareHouseIndex = x.Receiption.WareHouse.Index,

                        MaterialId = x.LotSupplies.MaterialId,
                        strMaterial = x.LotSupplies.Material.NameTM,
                        MaterialIndex = x.LotSupplies.Material.Index,

                        Quantity = x.LotSupplies.Quantity,
                        QuantityUsed = x.LotSupplies.QuantityUsed,
                        MaterialUnitId = x.LotSupplies.Material.UnitId,
                        strMaterialUnit = x.LotSupplies.Material.Unit.Name,

                        Price = x.LotSupplies.Price,
                        MoneyTypeId = x.Receiption.MoneyTypeId,
                        strMoneyType = x.Receiption.Unit.Name,
                        ExchangeRate = x.Receiption.ExchangeRate,

                        ManufactureDate = x.LotSupplies.ManufactureDate,
                        WarrantyDate = x.LotSupplies.WarrantyDate,
                        ExpiryDate = x.LotSupplies.ExpiryDate,

                        // StatusId = x.LotSupplies.StatusId,
                        Note = x.LotSupplies.Note,
                        CreatedDate = x.LotSupplies.CreatedDate,
                    }).ToList(), pageNumber, pageSize);
                    if (pagedList.Count > 0)
                    {
                        var cf = db.P_Config.ToList();
                        string lotvalue = "", mvalue = "", whvalue = "";
                        var found = cf.FirstOrDefault(x => x.Code == eConfigCode.LotSupplies);
                        if (found != null)
                            lotvalue = found.Value;

                        found = cf.FirstOrDefault(x => x.Code == eConfigCode.Material);
                        if (found != null)
                            mvalue = found.Value;

                        found = cf.FirstOrDefault(x => x.Code == eConfigCode.WareHouse);
                        if (found != null)
                            whvalue = found.Value;


                        if (pagedList.Count > 0)
                        {
                            foreach (var item in pagedList)
                            {
                                item.Code = lotvalue + item.Index;
                                item.strMaterial = "<span >" + item.strMaterial + "</span> (<span class=\"red\">" + mvalue + item.MaterialIndex + "</span>)";
                                item.strWarehouse = "<span >" + item.strWarehouse + "</span> (<span class=\"red\">" + whvalue + item.WareHouseIndex + "</span>)";
                            }
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

        public PagedList<LotSuppliesModel> Gets(int warehouseId, bool quantityGreaterThan0, string keyword, int startIndexRecord, int pageSize, string sorting)
        {
            using (db = new SanXuatCheckListEntities())
            {
                try
                {
                    if (string.IsNullOrEmpty(sorting))
                        sorting = "CreatedDate DESC";

                    IQueryable<ReceiptionDetail> objs = db.ReceiptionDetail.Where(c => !c.IsDeleted && !c.LotSupplies.IsDeleted);
                    if (warehouseId != 0)
                    {
                        objs = objs.Where(c => c.Receiption.StoreWarehouseId == warehouseId);
                    }
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        keyword = keyword.Trim().ToUpper();
                        objs = objs.Where(c => c.LotSupplies.Name.Trim().ToUpper().Contains(keyword));
                    }
                    if (quantityGreaterThan0)
                    {
                        objs = objs.Where(x => (x.LotSupplies.Quantity - x.LotSupplies.QuantityUsed) > 0 && x.Receiption.StatusId == (int)eStatus.Approved);
                    }
                    var pageNumber = (startIndexRecord / pageSize) + 1;

                    var pagedList = new PagedList<LotSuppliesModel>(objs.Select(x => new LotSuppliesModel()
                    {
                        Id = x.LotSupplies.Id,

                        Name = x.LotSupplies.Name,
                        Index = x.LotSupplies.Index,

                        strCustomer = x.Receiption.Customer.Name,

                        WareHouseId = x.Receiption.StoreWarehouseId,
                        strWarehouse = x.Receiption.WareHouse1.Name,
                        WareHouseIndex = x.Receiption.WareHouse1.Index,

                        MaterialId = x.LotSupplies.MaterialId,
                        strMaterial = x.LotSupplies.Material.NameTM,
                        MaterialIndex = x.LotSupplies.Material.Index,

                        Quantity = x.LotSupplies.Quantity,
                        QuantityUsed = x.LotSupplies.QuantityUsed,
                        MaterialUnitId = x.LotSupplies.Material.UnitId,
                        strMaterialUnit = x.LotSupplies.Material.Unit.Name,

                        Price = x.LotSupplies.Price,
                        MoneyTypeId = x.Receiption.MoneyTypeId,
                        strMoneyType = x.Receiption.Unit.Name,
                        ExchangeRate = x.Receiption.ExchangeRate,

                        InputDate = x.Receiption.InputDate,
                        ManufactureDate = x.LotSupplies.ManufactureDate,
                        WarrantyDate = x.LotSupplies.WarrantyDate,
                        ExpiryDate = x.LotSupplies.ExpiryDate,

                        StatusId = x.Receiption.StatusId,
                        strStatus = x.Receiption.Status.Name,
                        Note = x.LotSupplies.Note,
                        CreatedDate = x.LotSupplies.CreatedDate,
                        SpecificationsPaking = x.LotSupplies.SpecificationsPaking
                    }).ToList(), pageNumber, pageSize);
                    if (pagedList.Count > 0)
                    {
                        var cf = db.P_Config.ToList();
                        string lotvalue = "", mvalue = "", whvalue = "";
                        var found = cf.FirstOrDefault(x => x.Code == eConfigCode.LotSupplies);
                        if (found != null)
                            lotvalue = found.Value;

                        found = cf.FirstOrDefault(x => x.Code == eConfigCode.Material);
                        if (found != null)
                            mvalue = found.Value;

                        found = cf.FirstOrDefault(x => x.Code == eConfigCode.WareHouse);
                        if (found != null)
                            whvalue = found.Value;


                        if (pagedList.Count > 0)
                        {
                            foreach (var item in pagedList)
                            {
                                item.Code = lotvalue + item.Index;
                                item.strMaterial = "<span >" + item.strMaterial + "</span> (<span class=\"red\">" + mvalue + item.MaterialIndex + "</span>)";
                                item.strWarehouse = "<span >" + item.strWarehouse + "</span> (<span class=\"red\">" + whvalue + item.WareHouseIndex + "</span>)";
                            }
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


        public ReportInventoryModel GetReportInventory(string vattuId, string khoId)  // Báo cáo sl tồn kho vật tư theo vật tư
        {
            using (db = new SanXuatCheckListEntities())
            {
                var report = new ReportInventoryModel();
                List<ReportInventoryModel> listObjs = new List<ReportInventoryModel>();
                int mId = 0, whId = 0;
                if (!string.IsNullOrEmpty(vattuId))
                {
                    int.TryParse(vattuId, out mId);
                    var found = db.Material.FirstOrDefault(x => !x.IsDeleted && x.Id == mId);
                    if (found != null)
                        report.MaterialName = found.NameTM;
                }

                if (!string.IsNullOrEmpty(khoId))
                {
                    int.TryParse(khoId, out whId);
                    var found = db.WareHouse.FirstOrDefault(x => !x.IsDeleted && x.Id == mId);
                    if (found != null)
                        report.WarehouseName = found.Name;
                }

                var objs = db.ReceiptionDetail
                       .Where(x => !x.IsDeleted && !x.LotSupplies.Material.IsDeleted && (x.LotSupplies.Quantity - x.LotSupplies.QuantityUsed) > 0 && x.Receiption.StatusId == (int)eStatus.Approved);

                if (mId > 0)
                    objs = objs.Where(x => x.LotSupplies.MaterialId == mId);
                if (whId > 0)
                    objs = objs.Where(x => x.Receiption.StoreWarehouseId == whId);

                report.Details.AddRange( objs.Select(x => new ReportInventoryDetailModel()
                {
                    Id = x.Id,
                    Index = x.LotSupplies.Index,
                    Name = x.LotSupplies.Name,

                    MaterialId = x.LotSupplies.Material.Id,
                    MaterialIndex = x.LotSupplies.Material.Index,
                    MaterialName = x.LotSupplies.Material.NameTM,

                    Quantity = x.LotSupplies.Quantity,
                    QuantityUsed = x.LotSupplies.QuantityUsed,
                    UnitId = x.LotSupplies.Material.UnitId,
                    UnitName = x.LotSupplies.Material.Unit.Name,

                    StoreWarehouseId = x.Receiption.StoreWarehouseId,
                    StoreWareHouseIndex = x.Receiption.WareHouse1.Index,
                    StoreWareHouseName = x.Receiption.WareHouse1.Name,

                    Price = x.LotSupplies.Price,
                    MoneyTypeId = x.Receiption.MoneyTypeId,
                    ExchangeRate = x.Receiption.ExchangeRate,
                    TotalMoney = (x.LotSupplies.Quantity - x.LotSupplies.QuantityUsed) * x.LotSupplies.Price * x.Receiption.ExchangeRate,
                    MoneyTypeName = x.Receiption.Unit.Name,

                    InputDate = x.Receiption.InputDate,
                    ExpiryDate = x.LotSupplies.ExpiryDate,
                    ManufactureDate = x.LotSupplies.ManufactureDate,
                    WarrantyDate = x.LotSupplies.WarrantyDate,
                    SpecificationsPaking = x.LotSupplies.SpecificationsPaking
                    //  StatusId = x.LotSupplies.StatusId
                })
                .OrderBy(x => x.StoreWarehouseId)
                .ThenByDescending(x => x.MaterialId)
                .ToList());

                string lotvalue = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.LotSupplies);
                string mvalue = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.Material);
                string whvalue = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.WareHouse);
                for (int i = 0; i < report.Details.Count; i++)
                {
                    report.Details[i].Code = lotvalue + report.Details[i].Index;
                    report.Details[i].MaterialCode = mvalue + report.Details[i].MaterialIndex;
                    report.Details[i].StoreWareHouseCode = whvalue + report.Details[i].StoreWareHouseIndex;
                } 
                return report;
            }
        }

        public List<List<ReportInventoryModel>> GetReportInventoryByWH(int wId) // Báo cáo sl tồn kho vật tư theo kho
        {
            using (db = new SanXuatCheckListEntities())
            {
                List<List<ReportInventoryModel>> listResult = new List<List<ReportInventoryModel>>();
                List<ReportInventoryModel> listObjs = new List<ReportInventoryModel>();

                var listWId = new List<int>();

                if (wId == 0)
                    listWId = db.ReceiptionDetail
                        .Where(x => !x.IsDeleted && (x.LotSupplies.Quantity - x.LotSupplies.QuantityUsed) > 0)
                        .OrderBy(x => x.Receiption.StoreWarehouseId)
                        .Select(x => x.Receiption.StoreWarehouseId).Distinct().ToList();
                else
                    listWId.Add(wId);

                //foreach (var w in listWId)
                //{
                //    listObjs = db.ReceiptionDetail
                //    .Where(x => !x.IsDeleted && !x.LotSupplies.IsDeleted && (x.LotSupplies.Quantity - x.LotSupplies.QuantityUsed) > 0 && x.Receiption.StoreWarehouseId == w)
                //    .Select(x => new ReportInventoryDetailModel()
                //    {
                //        Id = x.Id,
                //        Index = x.LotSupplies.Index,
                //        Name = x.LotSupplies.Name,
                //        MaterialId = x.LotSupplies.Material.Id,
                //        MaterialIndex = x.LotSupplies.Material.Index,
                //        MaterialName = x.LotSupplies.Material.NameTM,
                //        Quantity = x.LotSupplies.Quantity,
                //        QuantityUsed = x.LotSupplies.QuantityUsed,
                //        UnitId = x.LotSupplies.Material.UnitId,
                //        // WareHouseId = x.LotSupplies.WareHouse.Id,
                //        StoreWareHouseIndex = x.Receiption.WareHouse.Index,
                //        StoreWareHouseName = x.Receiption.WareHouse.Name,
                //        Price = x.LotSupplies.Price,
                //        MoneyTypeId = x.Receiption.MoneyTypeId,
                //        ExchangeRate = x.Receiption.ExchangeRate,
                //        TotalMoney = (x.LotSupplies.Quantity - x.LotSupplies.QuantityUsed) * x.LotSupplies.Price * x.Receiption.ExchangeRate,
                //        InputDate = x.Receiption.InputDate,
                //        ExpiryDate = x.LotSupplies.ExpiryDate,
                //        //StatusId = x.Receiption.StatusId,
                //    }).ToList();

                //    if (listObjs != null && listObjs.Count > 0)
                //    {
                //        string lotvalue = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.LotSupplies);
                //        string mvalue = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.Material);
                //        string whvalue = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.WareHouse);
                //        foreach (var item in listObjs)
                //        {
                //            item.Code = lotvalue + item.Index;
                //            item.MaterialCode = mvalue + item.MaterialIndex;
                //            item.StoreWareHouseCode = whvalue + item.StoreWareHouseIndex;
                //        }
                //    }
                //    if (listObjs != null && listObjs.Count > 0)
                //        listResult.Add(listObjs);
                //}

                return listResult;
            }
        }

        public List<SuppliesModel> Gets(int materialId)
        {
            using (db = new SanXuatCheckListEntities())
            {
                try
                {
                    var objs = db.ReceiptionDetail
                        .Where(c => !c.IsDeleted && !c.LotSupplies.Material.IsDeleted && !c.LotSupplies.Material.MaterialType.IsDeleted && c.LotSupplies.MaterialId == materialId && (c.LotSupplies.Quantity - c.LotSupplies.QuantityUsed > 0))
                        .Select(x => new SuppliesModel()
                        {
                            Index = x.LotSupplies.Index,
                            Name = x.LotSupplies.Name,
                            WareHouseName = x.Receiption.WareHouse.Name,
                            StatusId = x.Receiption.StatusId,
                            UnitId = x.LotSupplies.Material.UnitId,
                            Price = x.LotSupplies.Price,
                            Quantity = x.LotSupplies.Quantity,
                            QuantityUsed = x.LotSupplies.QuantityUsed,
                            InputDate = x.Receiption.InputDate,
                            ExpireDate = x.LotSupplies.ExpiryDate,
                            MoneyTypeId = x.Receiption.MoneyTypeId
                        }).OrderBy(x => x.InputDate).ToList();
                    return objs;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return null;
            }
        }

        public LotSuppliesModel Get(int Id)
        {
            using (db = new SanXuatCheckListEntities())
            {
                var obj = db.ReceiptionDetail
                    .Where(c => !c.IsDeleted && !c.LotSupplies.IsDeleted && c.LotSuppliesId == Id)
                    .Select(x => new LotSuppliesModel()
                    {
                        Id = x.LotSupplies.Id,

                        Name = x.LotSupplies.Name,
                        Index = x.LotSupplies.Index,
                        strCustomer = x.Receiption.Customer.Name,
                        // Sửa
                        //WareHouseName = x.Receiption.WareHouse.Name,
                        WareHouseId = x.Receiption.WareHouse.Id,
                        strWarehouse = x.Receiption.WareHouse.Name,
                        WareHouseIndex = x.Receiption.WareHouse.Index,

                        //MaterialId = x.LotSupplies.MaterialId,
                        MaterialId = x.LotSupplies.Material.Id,
                        strMaterial = x.LotSupplies.Material.NameTM,
                        //  MateriaNameKH = x.LotSupplies.Material.NameKH,
                        MaterialIndex = x.LotSupplies.Material.Index,
                        // MTypeName = x.LotSupplies.Material.MaterialType.Name,

                        Quantity = x.LotSupplies.Quantity,
                        QuantityUsed = x.LotSupplies.QuantityUsed,
                        MaterialUnitId = x.LotSupplies.Material.UnitId,
                        strMaterialUnit = x.LotSupplies.Material.Unit.Name,

                        Price = x.LotSupplies.Price,
                        MoneyTypeId = x.Receiption.MoneyTypeId,
                        strMoneyType = x.Receiption.Unit.Name,
                        ExchangeRate = x.Receiption.ExchangeRate,

                        InputDate = x.Receiption.InputDate,
                        ManufactureDate = x.LotSupplies.ManufactureDate,
                        WarrantyDate = x.LotSupplies.WarrantyDate,
                        ExpiryDate = x.LotSupplies.ExpiryDate,

                        // StatusId = x.Receiption.StatusId,
                        Note = x.LotSupplies.Note,
                        CreatedDate = x.LotSupplies.CreatedDate,
                    })
                .FirstOrDefault();
                if (obj != null)
                {
                    var cf = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.LotSupplies);
                    if (!string.IsNullOrEmpty(cf))
                        obj.Code = cf + "_" + obj.Index;
                    else
                        obj.Code = obj.Index.ToString();
                }
                return obj;
            }
        }
    }
}
