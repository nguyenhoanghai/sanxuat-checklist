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
    public class BLLCustomer
    {
        #region constructor
        SanXuatCheckListEntities db;
        static object key = new object();
        private static volatile BLLCustomer _Instance;
        public static BLLCustomer Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLCustomer();

                return _Instance;
            }
        }
        private BLLCustomer() { }
        #endregion

        public PagedList<CustomerModel> GetList(string keyWord, int companyId, int[] relationCompanyId, int startIndexRecord, int pageSize, string sorting)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    if (string.IsNullOrEmpty(sorting))
                        sorting = "CreatedDate DESC";

                    IQueryable<Customer> customers = db.Customer.Where(x => !x.IsDeleted && (x.CompanyId == null || x.CompanyId == companyId || relationCompanyId.Contains(x.CompanyId ?? 0)));
                    if (!string.IsNullOrEmpty(keyWord))
                        customers = customers.Where(x => x.Name.Trim().ToUpper().Contains(keyWord.Trim().ToUpper()));

                    var pageNumber = (startIndexRecord / pageSize) + 1;
                    var pagedList = new PagedList<CustomerModel>(customers.Select(x => new CustomerModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Note = x.Note,
                        IsPrivate = (x.CompanyId == null ? true : false),
                        CompanyId = x.CompanyId,
                        Phone = x.Phone,
                        Address = x.Address,
                        CreatedDate = x.CreatedDate,

                        Index = x.Index,
                        Type = x.Type,
                        Fax = x.Fax,
                        Telephone = x.Telephone,
                        Email = x.Email,
                        Representative = x.Representative,
                        TaxCode = x.TaxCode,
                        BankAccount = x.BankAccount,
                        CardNumber = x.CardNumber,
                        DefaultAccount = x.DefaultAccount,
                        DefaultAccountCode = x.DefaultAccountCode,
                        CreditAccount = x.CreditAccount,
                        DebitLimitStandard = x.DebitLimitStandard,
                        DebitLimitAllow = x.DebitLimitAllow,
                        UpAdjust = x.UpAdjust,

                    }).OrderBy(sorting).ToList(), pageNumber, pageSize);

                    if (pagedList.Count > 0)
                    {
                        string value = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.Customer);
                        if (pagedList.Count > 0)
                            foreach (var item in pagedList)
                                item.Code = value + item.Index;
                    }
                    return pagedList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResponseBase InsertOrUpdate(CustomerModel model, bool isOwner)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var result = new ResponseBase();
                    if (CheckExists(model.Name.Trim().ToUpper(), model.Id, model.CompanyId))
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Insert Customer Type", Message = "Khách hàng này đã tồn tại. Vui lòng chọn lại Tên khác !." });
                        return result;
                    }
                    else
                    {
                        Customer obj;
                        if (model.Id == 0)
                        {
                            obj = new Customer();
                            Parse.CopyObject(model, ref obj);
                            obj.CreatedDate = DateTime.Now;
                            obj.CreatedUser = model.ActionUser;
                            db.Customer.Add(obj);
                            db.SaveChanges();
                            result.IsSuccess = true;
                        }
                        else
                        {
                            obj = db.Customer.FirstOrDefault(x => !x.IsDeleted && x.Id == model.Id);
                            if (obj == null)
                            {
                                result.IsSuccess = false;
                                result.Errors.Add(new Error() { MemberName = "Update Customer Type", Message = "Khách hàng bạn đang thao tác đã bị xóa hoặc không tồn tại. Vui lòng kiểm tra lại !." });
                                return result;
                            }
                            else
                            {
                                if (!checkPermis(obj, model.ActionUser, isOwner))
                                {
                                    result.IsSuccess = false;
                                    result.Errors.Add(new Error() { MemberName = "update", Message = "Bạn không phải là người tạo khách hàng này nên bạn không cập nhật được thông tin cho khách hàng này." });
                                }
                                else
                                {
                                    obj.CompanyId = model.CompanyId;
                                    obj.Name = model.Name;
                                    obj.Phone = model.Phone;
                                    obj.Address = model.Address;
                                    obj.Note = model.Note;
                                    obj.Address = model.Address;
                                    obj.Representative = model.Representative;
                                    obj.Type = model.Type;
                                    obj.Email = model.Email;
                                    obj.TaxCode = model.TaxCode;
                                    obj.Note = model.Note;
                                    obj.Fax = model.Fax;
                                    obj.Telephone = model.Telephone;
                                    obj.BankAccount = model.BankAccount;
                                    obj.CardNumber = model.CardNumber;
                                    obj.CreditAccount = model.CreditAccount;
                                    obj.DebitLimitAllow = model.DebitLimitAllow;
                                    obj.DebitLimitStandard = model.DebitLimitStandard;
                                    obj.DefaultAccountCode = model.DefaultAccountCode;
                                    obj.UpAdjust = model.UpAdjust;
                                    obj.DefaultAccount = model.DefaultAccount;
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

        private bool CheckExists(string code, int? id, int? companyId)
        {
            try
            {
                Customer obj = null;
                obj = db.Customer.FirstOrDefault(x => !x.IsDeleted && x.CompanyId == companyId && x.Name.Trim().ToUpper().Equals(code) && x.Id != id);

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
                    var obj = db.Customer.FirstOrDefault(x => !x.IsDeleted && x.Id == id);
                    if (obj == null)
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Delete Customer Type", Message = "Khách hàng bạn đang thao tác đã bị xóa hoặc không tồn tại. Vui lòng kiểm tra lại !." });
                    }
                    else
                    {
                        if (!checkPermis(obj, acctionUserId, isOwner))
                        {
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "Delete", Message = "Bạn không phải là người tạo khách hàng này nên bạn không xóa được khách hàng này." });
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

        public List<ModelSelectItem> GetSelectItem(int companyId, int[] relationCompanyId)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var objs = new List<ModelSelectItem>();
                    var CustomerTypes = db.Customer.Where(x => !x.IsDeleted && (x.CompanyId == null || x.CompanyId == companyId || relationCompanyId.Contains(x.CompanyId ?? 0))).Select(
                        x => new ModelSelectItem()
                        {
                            Value = x.Id,
                            Name = x.Name
                        }).ToList();

                    if (CustomerTypes != null && CustomerTypes.Count() > 0)
                    {
                        //objs.Add(new ModelSelectItem() { Value = 0, Name = " - -  Chọn khách hàng  - - " });
                        objs.AddRange(CustomerTypes);
                    }
                    else
                        objs.Add(new ModelSelectItem() { Value = 0, Name = "  Không có khách hàng  " });
                    return objs;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        bool checkPermis(Customer obj, int actionUser, bool isOwner)
        {
            if (isOwner) return true;
            return obj.CreatedUser == actionUser;
        }

        public int GetLastIndex()
        {
            using (db = new SanXuatCheckListEntities())
            {
                var obj = db.Customer.Where(x => !x.IsDeleted).OrderByDescending(x => x.Index).FirstOrDefault();
                return obj != null ? obj.Index : 0;
            }

        }

        public List<ModelSelectItem> GetSelectItem(string text) // Lọc tên vật tư khi nhập vào kendo combobox
        {
            using (db = new SanXuatCheckListEntities())
            {
                IEnumerable<Customer> objs = db.Customer.Where(x => !x.IsDeleted);
                var returnList = new List<ModelSelectItem>();
                if (!string.IsNullOrEmpty(text))
                {
                    objs = objs.Where(x => !x.IsDeleted && x.Name.Trim().ToUpper().Contains(text.Trim().ToUpper()));
                }

                if (objs != null && objs.Count() > 0)
                {
                    returnList.Add(new ModelSelectItem() { Value = 0, Code = "Chọn Tất Cả" });
                    var mvalue = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.Customer);
                    foreach (var item in objs)
                    {
                        returnList.Add(new ModelSelectItem() { Value = item.Id, Code = mvalue + item.Index });
                        returnList.Add(new ModelSelectItem() { Value = item.Id, Code = item.Name });
                    }
                }
                return returnList;
            }
        }
    }
}
