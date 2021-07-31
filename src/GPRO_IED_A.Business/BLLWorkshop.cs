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
    public class BLLWorkshop
    {
        #region constructor
        SanXuatCheckListEntities db;
        static object key = new object();
        private static volatile BLLWorkshop _Instance;
        public static BLLWorkshop Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLWorkshop();

                return _Instance;
            }
        }
        private BLLWorkshop() { }
        #endregion

        bool checkPermis(WorkShop obj, int actionUser, bool isOwner)
        {
            if (isOwner)
                return true;
            return obj.CreatedUser == actionUser;
        }
        private bool CheckExists(string name, string code, int Id, int CompanyId, SanXuatCheckListEntities db)
        {
            try
            {
                WorkShop objectExists = null;
                if (!string.IsNullOrEmpty(name))
                    objectExists = db.WorkShop.FirstOrDefault(c => !c.IsDeleted && c.Id != Id && c.CompanyId == CompanyId && c.Name.Trim().ToUpper().Equals(name.Trim().ToUpper()));
                else
                    objectExists = db.WorkShop.FirstOrDefault(c => !c.IsDeleted && c.Id != Id && c.CompanyId == CompanyId && c.Code.Trim().ToUpper().Equals(code.Trim().ToUpper()));

                if (objectExists == null)
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResponseBase InsertOrUpdate(WorkShopModel model, bool isOwner)
        {
            ResponseBase result = new ResponseBase();
            result.IsSuccess = false; var flag = false;
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    if (CheckExists(model.Name.Trim().ToUpper(), null, model.Id, model.CompanyId, db))
                    {
                        flag = true;
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Create  ", Message = "Tên Phân Xưởng này Đã Tồn Tại,Vui Lòng Chọn Tên Khác" });
                    }
                    else if (!string.IsNullOrEmpty(model.Code))
                    {
                        if (CheckExists(null, model.Code.Trim().ToUpper(), model.Id, model.CompanyId, db))
                        {
                            flag = true;
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "Create  ", Message = "Mã Phân Xưởng này Đã Tồn Tại,Vui Lòng Chọn Mã Khác" });
                        }
                    }

                    if (!flag)
                    {
                        WorkShop obj;
                        if (model.Id == 0)
                        {
                            obj = new WorkShop();
                            Parse.CopyObject(model, ref obj);
                            obj.CreatedDate = DateTime.Now;
                            db.WorkShop.Add(obj);
                            db.SaveChanges();
                            result.IsSuccess = true;
                        }
                        else
                        {
                            obj = db.WorkShop.FirstOrDefault(x => x.Id == model.Id && !x.IsDeleted);
                            if (obj != null)
                            {
                                if (!checkPermis(obj, model.ActionUser, isOwner))
                                {
                                    result.IsSuccess = false;
                                    result.Errors.Add(new Error() { MemberName = "update", Message = "Bạn không phải là người tạo phân xưởng này nên bạn không cập nhật được thông tin cho phân xưởng này." });
                                }
                                else
                                {
                                    obj.Code = model.Code;
                                    obj.Name = model.Name;
                                    obj.Description = model.Description;
                                    obj.UpdatedDate = DateTime.Now;
                                    obj.UpdatedUser = model.ActionUser;

                                    // cap nhat ben phan tich mat hang
                                    //var commoAna = db.T_CommodityAnalysis.Where(x => !x.IsDeleted && x.ObjectId == obj.Id && x.ObjectType == (int)eObjectType.isWorkShop);
                                    //if (commoAna != null && commoAna.Count() > 0)
                                    //{
                                    //    foreach (var item in commoAna)
                                    //    {
                                    //        item.Name = obj.Name;
                                    //        item.UpdatedUser = model.ActionUser;
                                    //        item.UpdatedDate = DateTime.Now;
                                    //    }
                                    //}
                                    db.SaveChanges();
                                    result.IsSuccess = true;
                                }
                            }
                            else
                            {
                                result.IsSuccess = false;
                                result.Errors.Add(new Error() { MemberName = "UpdateWorkShop", Message = "Thông tin nhập không đúng Vui lòng kiểm tra lại!" });
                            }
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public ResponseBase Delete(int id, int userId, bool isOwner)
        {
            ResponseBase rs;

            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    rs = new ResponseBase();
                    var WorkShop = db.WorkShop.Where(c => !c.IsDeleted && c.Id == id).FirstOrDefault();
                    if (WorkShop != null)
                    {
                        if (!checkPermis(WorkShop, userId, isOwner))
                        {
                            rs.IsSuccess = false;
                            rs.Errors.Add(new Error() { MemberName = "Delete", Message = "Bạn không phải là người tạo phân xưởng này nên bạn không xóa được phân xưởng này." });
                        }
                        else
                        {
                            WorkShop.IsDeleted = true;
                            WorkShop.DeletedUser = userId;
                            WorkShop.DeletedDate = DateTime.Now;
                            db.SaveChanges(); ;
                            rs.IsSuccess = true;
                        }
                    }
                    else
                    {
                        rs.IsSuccess = false;
                        rs.Errors.Add(new Error() { MemberName = "Delete", Message = "Đối Tượng Đã Bị Xóa,Vui Lòng Kiểm Tra Lại" });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return rs;
        }

        public List<ModelSelectItem> GetListWorkShop()
        {

            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    List<ModelSelectItem> objs = new List<ModelSelectItem>();
                    var workshops = db.WorkShop.Where(x => !x.IsDeleted).Select(x => new ModelSelectItem() { Value = x.Id, Name = x.Name });
                    if (workshops != null && workshops.Count() > 0)
                        objs.AddRange(workshops);
                    else
                        objs.Add(new ModelSelectItem() { Value = 0, Name = " Không có Dữ Liệu " });
                    return objs;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public PagedList<WorkShopModel> GetList(string keyWord,  int startIndexRecord, int pageSize, string sorting, int companyId)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    if (string.IsNullOrEmpty(sorting))
                        sorting = "Id DESC";

                    IQueryable<WorkShop> workshops = workshops = db.WorkShop.Where(c => !c.IsDeleted && c.CompanyId == companyId);
                    var pageNumber = (startIndexRecord / pageSize) + 1;

                    if (!string.IsNullOrEmpty(keyWord))
                    {
                        keyWord = keyWord.Trim().ToUpper();
                        workshops = workshops.Where(c => (c.Code.Trim().ToUpper().Contains(keyWord) || c.Name.Trim().ToUpper().Contains(keyWord)));
                    }

                    return new PagedList<WorkShopModel>(workshops.OrderBy(sorting).Select(c => new WorkShopModel()
                    {
                        Id = c.Id,
                        Code = c.Code,
                        Name = c.Name,
                        Description = c.Description,
                    }).ToList(), pageNumber, pageSize);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
