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
    public class BLLProductionFile
    {
        #region constructor
        SanXuatCheckListEntities db;
        static object key = new object();
        private static volatile BLLProductionFile _Instance;
        public static BLLProductionFile Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLProductionFile();

                return _Instance;
            }
        }
        private BLLProductionFile() { }
        #endregion

        public ProductionFile GetById(int Id)
        {
            using (db = new SanXuatCheckListEntities())
            {
                return db.ProductionFile.FirstOrDefault(x => !x.IsDeleted && x.Id == Id);
            }
        }

        public ResponseBase CreateOrUpdate(ProductionFileModel model)
        {
            var result = new ResponseBase();
            result.IsSuccess = false;
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    result.IsSuccess = true;
                    if (result.IsSuccess)
                    {
                        var now = DateTime.Now;
                        ProductionFile obj;
                        ProFileControl control;
                        if (model.Id == 0)
                        {
                            obj = new ProductionFile();
                            Parse.CopyObject(model, ref obj);
                            obj.CreatedDate = now;
                            obj.CreatedUser = model.ActionUser;
                            obj.ApprovedUser = null;
                            obj.ApprovedDate = null;

                            if (model.IsApproved)
                            {
                                obj.ApprovedUser = model.ActionUser;
                                obj.ApprovedDate = now;
                            }

                            if (model.Controls != null && model.Controls.Count > 0)
                            {
                                obj.ProFileControl = new Collection<ProFileControl>();
                                foreach (var item in model.Controls)
                                {
                                    control = new ProFileControl();
                                    control.ControlType = item.ControlType;
                                    control.ControlName = item.ControlName;
                                    control.Checked = item.Checked;
                                    control.Value = item.Value;
                                    control.ProductionFile = obj;
                                    control.IsDeleted = false;
                                    obj.ProFileControl.Add(control);
                                }
                            }
                            db.ProductionFile.Add(obj);
                            result.IsSuccess = true;
                        }
                        else // cập nhật
                        {
                            obj = db.ProductionFile.FirstOrDefault(x => !x.IsDeleted && x.Id == model.Id);
                            if (obj != null)
                            {
                                if (obj.IsApproved)
                                {
                                    result.IsSuccess = false;
                                    result.Errors.Add(new Error() { MemberName = "Create", Message = "Biểu Mẫu này đã được duyệt bạn không thể thay đổi thông tin được nữa." });
                                }
                                else
                                {
                                    // obj.TemplateFileId = model.TemplateFileId;
                                    obj.Name = model.Name;
                                    //obj.RequireId = model.RequireId;
                                    obj.IsApproved = model.IsApproved;
                                    obj.ApprovedUser = null;
                                    obj.ApprovedDate = null;
                                    obj.UpdatedDate = now;
                                    obj.UpdatedUser = model.ActionUser;

                                    if (model.IsApproved)
                                    {
                                        obj.ApprovedUser = model.ActionUser;
                                        obj.ApprovedDate = now;
                                    }
                                    obj.Content = model.Content;

                                    var oldControls = db.ProFileControl.Where(x => !x.IsDeleted && x.ProductFileId == obj.Id);
                                    if (oldControls != null && oldControls.Count() > 0)
                                    {
                                        if (model.Controls == null || model.Controls.Count == 0)
                                        {
                                            #region xoa het neu list control moi ko co cai nao
                                            foreach (var item in oldControls)
                                            {
                                                item.IsDeleted = true;
                                                //db.ProFileControl.Update(item);
                                            }
                                            #endregion
                                        }
                                        else
                                        {
                                            foreach (var item in model.Controls)
                                            {
                                                var exist = oldControls.FirstOrDefault(x => x.ControlName == item.ControlName);
                                                if (exist != null)
                                                {
                                                    #region   neu co thi update lai value
                                                    // exist.Value = item.Checked; 
                                                    exist.Value = item.Value;
                                                    //db.ProFileControl.Update(exist);
                                                    #endregion
                                                }
                                                else
                                                {
                                                    #region   ko co thi tao moi
                                                    control = new ProFileControl();
                                                    control.ControlType = item.ControlType;
                                                    control.ControlName = item.ControlName;
                                                    control.Checked = item.Checked;
                                                    control.Value = item.Value;
                                                    control.ProductFileId = obj.Id;
                                                    control.IsDeleted = false;
                                                    db.ProFileControl.Add(control);
                                                    #endregion
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (model.Controls != null)
                                        {
                                            #region  truoc do ko co control nao thi tao moi
                                            foreach (var item in model.Controls)
                                            {
                                                control = new ProFileControl();
                                                control.ControlType = item.ControlType;
                                                control.ControlName = item.ControlName;
                                                control.Checked = item.Checked;
                                                control.Value = item.Value;
                                                control.ProductFileId = obj.Id;
                                                control.IsDeleted = false;
                                                db.ProFileControl.Add(control);
                                            }
                                            #endregion
                                        }
                                    }
                                    result.IsSuccess = true;
                                }
                            }
                            else
                            {
                                result.IsSuccess = false;
                                result.Errors.Add(new Error() { MemberName = "Create", Message = "Mẫu Kiểm Nghiệm không tồn tại hoặc đã bị xóa, Vui Lòng kiểm tra lại." });
                            }
                        }
                        if (result.IsSuccess)
                        {
                            db.SaveChanges();
                            if (string.IsNullOrEmpty(obj.CodeUrl))
                            {
                                string str = "/ProductionFile/Create/" + obj.Id;
                                obj.CodeUrl = EncryptString.Encrypt(str);
                                //db.ProductionFile.Update(obj);
                                db.SaveChanges();
                            }
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
                        result.Errors.Add(new Error() { MemberName = "Create", Message = "Tên Đã Tồn Tại, Vui Lòng Chọn Tên Khác" });
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Errors.Add(new Error() { MemberName = "Create", Message = "Lỗi Exception" });
            }
            return result;
        }

        public ResponseBase Delete(int Id, int actionUserId)
        {
            var rs = new ResponseBase();
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var obj = db.ProductionFile.FirstOrDefault(x => !x.IsDeleted && x.Id == Id);
                    if (obj != null)
                    {
                        var now = DateTime.Now;
                        obj.IsDeleted = true;
                        obj.DeletedDate = now;
                        obj.DeletedUser = actionUserId;
                        //db.ProductionFile.Update(obj);
                        db.SaveChanges();
                        rs.IsSuccess = true;
                    }
                    else
                    {
                        rs.IsSuccess = false;
                        rs.Errors.Add(new Error() { MemberName = "Delete", Message = "Biểu Mẫu này không tồn tại hoặc đã bị xóa, Vui Lòng kiểm tra lại." });
                    }
                }
            }
            catch (Exception)
            {
                rs.IsSuccess = false;
                rs.Errors.Add(new Error() { MemberName = "Delete", Message = "Lỗi Exception" });
            }
            return rs;
        }

        public PagedList<ProductionFileModel> GetList(string keyWord, int startIndexRecord, int pageSize, string sorting)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    if (string.IsNullOrEmpty(sorting))
                        sorting = "CreatedDate DESC";

                    IQueryable<ProductionFile> Ilist = db.ProductionFile
                        .Where(x => !x.IsDeleted && !x.TemplateFile.IsDeleted);

                    if (!string.IsNullOrEmpty(keyWord))
                    {
                        keyWord = keyWord.Trim().ToUpper();
                        Ilist = Ilist.Where(x => x.Name.Trim().ToUpper().Contains(keyWord));
                    }

                    var pageNumber = (startIndexRecord / pageSize) + 1;
                    var _returnObjs = new PagedList<ProductionFileModel>(Ilist.OrderBy(sorting)
                        .Select(x => new ProductionFileModel()
                        {
                            Id = x.Id,
                            TemplateFileId = x.TemplateFileId,
                            Name = x.Name,
                            Index = x.Index,
                            // RequireId = x.RequireId,
                            IsApproved = x.IsApproved,
                            ApprovedDate = x.ApprovedDate,
                            ApprovedUser = x.ApprovedUser,
                            TemplateFileIndex = x.TemplateFile.Index,
                            TemplateFiletName = x.TemplateFile.Name,
                            //ProductionBatchIndex = x.P_Requirement.Index,
                            //ProductionBatchName = x.P_Requirement.Name,
                            //CreatedDate = x.CreatedDate,
                            CodeUrl = x.CodeUrl,
                            // ProductId = x.P_Requirement.MaterialId,
                        }).ToList(), pageNumber, pageSize);

                    if (_returnObjs.Count > 0)
                    {
                        string pbvalue = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.REQUIRE);
                        string tfvalue = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.TemplateFile);
                        string code = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.PROFILE);
                        var users = BLLUser.Instance.GetSelectItem(null);
                        foreach (var item in _returnObjs)
                        {
                            item.ProductionBatchName = pbvalue + item.ProductionBatchIndex;
                            item.TemplateFiletName = item.TemplateFiletName + " (" + tfvalue + item.TemplateFileIndex + ")";
                            item.CodeUrl = code + item.Index;

                            if (item.ApprovedUser.HasValue)
                            {
                                var found = users.FirstOrDefault(x => x.Value == item.ApprovedUser.Value);
                                if (found != null)
                                    item.ApproverName = found.Name;
                            }
                        } 
                    }
                    return _returnObjs;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ModelSelectItem> GetSelectList()
        {
            using (db = new SanXuatCheckListEntities())
            {
                return db.ProductionFile.Where(x => !x.IsDeleted).Select(x => new ModelSelectItem() { Value = x.Id, Name = x.TemplateFile.Name }).ToList();
            }
        }

        public ProductionFileModel GetByCode(int index)
        {
            using (db = new SanXuatCheckListEntities())
            {
                var obj = db.ProductionFile.Where(x => !x.IsDeleted //&& !x.P_Requirement.IsDeleted
                    && !x.TemplateFile.IsDeleted && x.Index == index).Select(x => new ProductionFileModel()
                    {
                        Id = x.Id,
                        TemplateFileId = x.TemplateFileId,
                        Name = x.Name,
                        //RequireId = x.RequireId,
                        IsApproved = x.IsApproved,
                        ApprovedDate = x.ApprovedDate,
                        ApprovedUser = x.ApprovedUser,
                        CodeUrl = x.CodeUrl,
                        //ApproverName=x.P_ProductionBatch.P_Customer.Name+" ("+x.P_ProductionBatch.P_Customer.Code+")",
                        TemplateFileIndex = x.TemplateFile.Index,
                        TemplateFiletName = x.TemplateFile.Name,
                        //ProductionBatchIndex = x.P_Requirement.Index,
                        Index = x.Index,
                        Content = x.Content,
                    }).FirstOrDefault();
                if (obj != null)
                {
                    var users = BLLUser.Instance.GetSelectItem(null);
                    if (obj.ApprovedUser.HasValue)
                    {
                        var found = users.FirstOrDefault(x => x.Value == obj.ApprovedUser.Value);
                        if (found != null)
                            obj.ApproverName = found.Name;
                    }

                    //obj.ProductionBatchName = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.REQUIRE) + obj.ProductionBatchIndex;
                    obj.CodeUrl = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.PROFILE) + obj.Index;
                    obj.TemplateFiletName = obj.TemplateFiletName + " (" + BLLAppConfig.Instance.GetConfigByCode(eConfigCode.PROFILE) + obj.Index + ")";

                    obj.Controls.AddRange(db.ProFileControl.Where(x => !x.IsDeleted && x.ProductFileId == obj.Id).Select(x => new ControlModel()
                    {
                        Id = x.Id,
                        Value = x.Value,
                        Checked = x.Checked,
                        ControlType = x.ControlType,
                        ControlName = x.ControlName,
                        proFileId = x.ProductFileId,
                    }));
                }
                return obj;
            }
        }

        public ModelSelectItem GetNewCode()
        {
            using (db = new SanXuatCheckListEntities())
            {
                ModelSelectItem rs = new ModelSelectItem();
                var obj = db.ProductionFile.Where(x => !x.IsDeleted).OrderByDescending(x => x.Index).FirstOrDefault();
                rs.Name = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.PROFILE);
                rs.Value = (obj != null ? (obj.Index + 1) : 1);
                return rs;
            }
        }

    }
}
