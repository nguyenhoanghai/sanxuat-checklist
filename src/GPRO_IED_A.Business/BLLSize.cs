using SanXuatCheckList.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hugate.Framework;
using GPRO.Core.Mvc;
using SanXuatCheckList.Business.Model;
using PagedList;
using GPRO.Ultilities;

namespace SanXuatCheckList.Business
{
   public class BLLSize
    {
        #region constructor
        SanXuatCheckListEntities db;
        static object key = new object();
        private static volatile BLLSize _Instance;
        public static BLLSize Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLSize();

                return _Instance;
            }
        }
        private BLLSize() { }
        #endregion

        bool checkPermis(Size obj, int actionUser, bool isOwner)
        {
            if (isOwner) return true;
            return obj.CreatedUser == actionUser;
        }

        public PagedList<SizeModel> GetList(string keyWord, int startIndexRecord, int pageSize, string sorting)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    if (string.IsNullOrEmpty(sorting))
                        sorting = "Id DESC";

                    IQueryable<Size> objs = null;
                    if (string.IsNullOrEmpty(keyWord))
                        objs = db.Size.Where(x => !x.IsDeleted).OrderByDescending(x => x.CreatedDate);
                    else
                        objs = db.Size.Where(x => !x.IsDeleted && x.Name.Trim().ToUpper().Contains(keyWord.Trim().ToUpper())).OrderByDescending(x => x.CreatedDate);

                    var pageNumber = (startIndexRecord / pageSize) + 1;
                    return new PagedList<SizeModel>(objs.Select(x => new SizeModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Note = x.Note,
                    }).OrderBy(sorting).ToList(), pageNumber, pageSize);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResponseBase InsertOrUpdate(SizeModel model, bool isOwner)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var result = new ResponseBase();
                    if (CheckExists(model.Name.Trim().ToUpper(), model.Id))
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Insert ", Message = "Tên kích cỡ này đã tồn tại. Vui lòng chọn lại Tên khác !." });
                        return result;
                    }
                    else
                    {
                        Size obj;
                        if (model.Id == 0)
                        {
                            obj = new Size();
                            Parse.CopyObject(model, ref obj);
                            obj.CreatedDate = DateTime.Now;
                            obj.CreatedUser = model.ActionUser;
                            db.Size.Add(obj);
                            db.SaveChanges();
                            result.IsSuccess = true;
                        }
                        else
                        {
                            obj = db.Size.FirstOrDefault(x => !x.IsDeleted && x.Id == model.Id);
                            if (obj == null)
                            {
                                result.IsSuccess = false;
                                result.Errors.Add(new Error() { MemberName = "Update ", Message = "Kích cỡ bạn đang thao tác đã bị xóa hoặc không tồn tại. Vui lòng kiểm tra lại !." });
                                return result;
                            }
                            else
                            {
                                if (!checkPermis(obj, model.ActionUser, isOwner))
                                {
                                    result.IsSuccess = false;
                                    result.Errors.Add(new Error() { MemberName = "update", Message = "Bạn không phải là người tạo kích cỡ này nên bạn không cập nhật được thông tin cho kích cỡ này." });
                                }
                                else
                                {
                                    obj.Name = model.Name;
                                    obj.Note = model.Note;
                                    obj.UpdatedUser = model.ActionUser;
                                    obj.UpdatedDate = DateTime.Now;
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

        private bool CheckExists(string code, int? id)
        {
            try
            {
                Size obj = null;
                obj = db.Size.FirstOrDefault(x => !x.IsDeleted && x.Name.Trim().ToUpper().Equals(code) && x.Id != id);

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
                    var objs = db.Size.FirstOrDefault(x => !x.IsDeleted && x.Id == id);
                    if (objs == null)
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Delete ", Message = "Kích cỡ bạn đang thao tác đã bị xóa hoặc không tồn tại. Vui lòng kiểm tra lại !." });
                    }
                    else
                    {
                        if (!checkPermis(objs, acctionUserId, isOwner))
                        {
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "Delete", Message = "Bạn không phải là người tạo kích cỡ này nên bạn không xóa được kích cỡ này." });
                        }
                        else
                        {
                            objs.IsDeleted = true;
                            objs.DeletedUser = acctionUserId;
                            objs.DeletedDate = DateTime.Now;

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
                    var selectItems = new List<ModelSelectItem>();
                    var objs = db.Size.Where(x => !x.IsDeleted).Select(
                        x => new ModelSelectItem()
                        {
                            Value = x.Id,
                            Name = x.Name
                        }).ToList();

                    if (objs != null && objs.Count() > 0)
                    {
                       // selectItems.Add(new ModelSelectItem() { Value = 0, Name = " - -  Chọn kích cỡ  - - " });
                        selectItems.AddRange(objs);
                    }
                    else
                        selectItems.Add(new ModelSelectItem() { Value = 0, Name = "  Không có kích cỡ  " });
                    return selectItems;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
