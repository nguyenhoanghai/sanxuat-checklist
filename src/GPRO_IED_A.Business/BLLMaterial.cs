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
    public class BLLMaterial
    {
        #region constructor
        SanXuatCheckListEntities db;
        static object key = new object();
        private static volatile BLLMaterial _Instance;
        public static BLLMaterial Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLMaterial();

                return _Instance;
            }
        }
        private BLLMaterial() { }
        #endregion

        bool checkPermis(Material obj, int actionUser, bool isOwner)
        {
            if (isOwner) return true;
            return obj.CreatedUser == actionUser;
        }

        public ResponseBase CreateOrUpdate(MaterialModel model, bool isOwner)
        {
            using (db = new SanXuatCheckListEntities())
            {
                var result = new ResponseBase();
                result.IsSuccess = false;
                try
                {
                    if (!CheckExists(model.Id, model.NameTM, model.Index, true)) // nếu ko bị trùng tên
                    {
                        Material material;
                        MaterialNorms norms;
                        var now = DateTime.Now;
                        if (model.Id == 0)
                        {
                            material = new Material();
                            Parse.CopyObject(model, ref material);
                            material.MTypeId = model.MTypeId;
                            material.CreatedUser = model.ActionUser;
                            material.CreatedDate = now;
                            if (model.Norms != null && model.Norms.Count > 1)
                            {
                                for (int i = 0; i < model.Norms.Count; i++)
                                {
                                    norms = new MaterialNorms();
                                    norms.Material = material;
                                    norms.ObjectId = model.Norms[i].MaterialId;
                                    norms.Quantities = model.Norms[i].Quantities;
                                    norms.CreatedDate = now;
                                    norms.CreatedUser = model.ActionUser;

                                    material.MaterialNorms.Add(norms);
                                }
                            }
                            db.Material.Add(material);
                            result.IsSuccess = true;
                        }
                        else
                        {
                            material = GetById(model.Id);
                            if (material != null)
                            {
                                if (!checkPermis(material, model.ActionUser, isOwner))
                                {
                                    result.IsSuccess = false;
                                    result.Errors.Add(new Error() { MemberName = "update", Message = "Bạn không phải là người tạo vật tư này nên bạn không cập nhật được thông tin cho vật tư này." });
                                }
                                else
                                {
                                    material.Index = model.Index;
                                    material.NameTM = model.NameTM;
                                    material.NameKH = model.NameKH;
                                    material.UnitId = model.UnitId;
                                    material.MTypeId = model.MTypeId;
                                    material.Note = model.Note;
                                    if (!string.IsNullOrEmpty(model.Picture))
                                        material.Picture = model.Picture;

                                    var oldNorms = db.MaterialNorms.Where(x => !x.IsDeleted && x.MaterialId == material.Id).ToList();
                                    if (oldNorms != null && oldNorms.Count() > 0 && model.Norms.Count == 1)
                                    {
                                        for (int i = 0; i < oldNorms.Count; i++)
                                        {
                                            oldNorms[i].IsDeleted = true;
                                            oldNorms[i].DeletedDate = now;
                                            oldNorms[i].DeletedUser = model.ActionUser;
                                        }
                                    }
                                    else if ((oldNorms == null || oldNorms.Count() == 0) && model.Norms.Count > 1)
                                    {
                                        for (int i = 0; i < model.Norms.Count; i++)
                                        {
                                            if (i < model.Norms.Count - 1)
                                            {
                                                norms = new MaterialNorms();
                                                norms.MaterialId = model.Id;
                                                norms.ObjectId = model.Norms[i].MaterialId;
                                                norms.Quantities = model.Norms[i].Quantities;
                                                norms.CreatedDate = now;
                                                norms.CreatedUser = model.ActionUser;
                                                material.MaterialNorms.Add(norms);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        for (int i = 0; i < model.Norms.Count; i++)
                                        {
                                            if (i < model.Norms.Count - 1)
                                            {
                                                norms = oldNorms.FirstOrDefault(x => x.ObjectId == model.Norms[i].MaterialId);
                                                if (norms != null)
                                                {
                                                    norms.Quantities = model.Norms[i].Quantities;
                                                    norms.UpdatedDate = now;
                                                    norms.UpdatedUser = model.ActionUser;
                                                    oldNorms.Remove(norms);
                                                }
                                                else
                                                {
                                                    norms = new MaterialNorms();
                                                    norms.MaterialId = model.Id;
                                                    norms.ObjectId = model.Norms[i].MaterialId;
                                                    norms.Quantities = model.Norms[i].Quantities;
                                                    norms.CreatedDate = now;
                                                    norms.CreatedUser = model.ActionUser;
                                                    material.MaterialNorms.Add(norms);
                                                }
                                            }
                                        }
                                        if (oldNorms.Count > 0)
                                        {
                                            for (int i = 0; i < oldNorms.Count; i++)
                                            {
                                                oldNorms[i].IsDeleted = true;
                                                oldNorms[i].DeletedDate = now;
                                                oldNorms[i].DeletedUser = model.ActionUser;
                                            }
                                        }
                                    }
                                    result.IsSuccess = true;
                                }
                            }
                            else
                            {
                                result.IsSuccess = false;
                                result.Errors.Add(new Error() { MemberName = "Create", Message = "Vật tư này không Tồn Tại hoặc đã bị xóa, Vui Lòng kiểm tra lại." });
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
                        result.Errors.Add(new Error() { MemberName = "Create", Message = "Tên Đã Tồn Tại, Vui Lòng Chọn Tên Khác" });
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
            Material obj = db.Material.FirstOrDefault(x => !x.IsDeleted && x.Id != Id && x.NameTM.Trim().Equals(value));
            return obj != null ? true : false;
        }
        public ResponseBase Delete(int Id, int actionUserId, bool isOwner)
        {
            using (db = new SanXuatCheckListEntities())
            {
                var rs = new ResponseBase();
                try
                {
                    var material = GetById(Id);
                    if (material != null)
                    {
                        if (!checkPermis(material,  actionUserId, isOwner))
                        {
                            rs.IsSuccess = false;
                            rs.Errors.Add(new Error() { MemberName = "update", Message = "Bạn không phải là người tạo vật tư này nên bạn không cập nhật được thông tin cho vật tư này." });
                        }
                        else
                        {
                            var now = DateTime.Now;
                        material.IsDeleted = true;
                        material.DeletedDate = now;
                        material.DeletedUser = actionUserId;
                        db.SaveChanges();
                        rs.IsSuccess = true;
                    }
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

        public PagedList<MaterialModel> GetList(int mTypeId, int startIndexRecord, int pageSize, string sorting)
        {
            using (db = new SanXuatCheckListEntities())
            {
                try
                {
                    if (string.IsNullOrEmpty(sorting))
                        sorting = "Index DESC";

                    var materials = db.Material.Where(c => !c.IsDeleted && c.MTypeId == mTypeId).Select(x => new MaterialModel()
                    {
                        Id = x.Id,
                        Index = x.Index,
                        NameKH = x.NameKH,
                        NameTM = x.NameTM,
                        Note = x.Note,
                        UnitId = x.UnitId,
                        UnitName = x.Unit.Name,
                        MTypeId = x.MTypeId,
                        TypeName = x.MaterialType.Name,
                        Picture = x.Picture
                    }).OrderBy(sorting).ToList();
                    var pageNumber = (startIndexRecord / pageSize) + 1;
                    var pagedList = new PagedList<MaterialModel>(materials, pageNumber, pageSize);
                    if (pagedList.Count > 0)
                    {
                        string value = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.Material);
                        if (pagedList.Count > 0)
                        {
                            foreach (var item in pagedList)
                            {
                                item.Code = value + item.Index;
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

        public List<ModelSelectItem> GetSelectList(int mTypeId)
        {
            using (db = new SanXuatCheckListEntities())
            {
                if (mTypeId == 0)
                    return db.Material.Where(x => !x.IsDeleted && !x.MaterialType.IsDeleted).Select(x => new ModelSelectItem() { Value = x.Id, Name = (x.NameTM + " (" + x.MaterialType.Name + ")"), Code = x.Unit.Name, Data = x.UnitId }).ToList();
                else
                    return db.Material.Where(x => !x.IsDeleted && !x.MaterialType.IsDeleted && x.MTypeId == mTypeId).Select(x => new ModelSelectItem() { Value = x.Id, Name = (x.NameTM + " (" + x.MaterialType.Name + ")"), Code = x.Unit.Name, Data = x.UnitId }).ToList();
            }
        }

        public Material GetById(int Id)
        {
            if (db != null)
                return db.Material.FirstOrDefault(x => !x.IsDeleted && !x.MaterialType.IsDeleted && x.Id == Id);
            else
                using (db = new SanXuatCheckListEntities())
                    return db.Material.FirstOrDefault(x => !x.IsDeleted && !x.MaterialType.IsDeleted && x.Id == Id);
        }

        public List<Material> GetFullList()
        {
            using (db = new SanXuatCheckListEntities())
            {
                return db.Material.Where(x => !x.IsDeleted && !x.MaterialType.IsDeleted).ToList();
            }
        }
        public int GetMaterialLastIndex()
        {
            using (db = new SanXuatCheckListEntities())
            {
                var obj = db.Material.Where(x => !x.IsDeleted).OrderByDescending(x => x.Index).FirstOrDefault();
                return obj != null ? obj.Index : 0;
            }
        }

        public List<ModelSelectItem> GetMaterialSelect_FilterByTextInput(string text) // Lọc tên vật tư khi nhập vào kendo combobox
        {
            using (db = new SanXuatCheckListEntities())
            {
                IEnumerable<Material> materials = null;
                var returnList = new List<ModelSelectItem>();

                materials = db.Material.Where(x => !x.IsDeleted);
                if (materials != null && materials.Count() > 0)
                {
                    returnList.Add(new ModelSelectItem() { Value = 0, Code = "Chọn Tất Cả" });
                    var mvalue = BLLAppConfig.Instance.GetConfigByCode(eConfigCode.Material);
                    foreach (var item in materials)
                    {
                        returnList.Add(new ModelSelectItem() { Value = item.Id, Code = mvalue + item.Index });
                        returnList.Add(new ModelSelectItem() { Value = item.Id, Code = item.NameTM });
                    }
                }
                return returnList;
            }
        }


        public List<MaterialNormsModel> GetNormsApi(int materialId)
        {
            var returnlist = new List<MaterialNormsModel>();
            try
            {
                var IdCheck = new List<int>();
                first:
                returnlist.AddRange(db.MaterialNorms.Where(x => !x.IsDeleted && !x.Material.IsDeleted && !x.Material.MaterialType.IsDeleted && x.MaterialId == materialId).Select(x => new MaterialNormsModel() { Id = x.Id, MaterialId = x.ObjectId, Name = x.Material.NameTM, Quantities = x.Quantities, UnitId = x.Material.UnitId }).ToList());
                for (int i = 0; i < returnlist.Count; i++)
                {
                    if (!IdCheck.Contains(returnlist[i].MaterialId))
                    {
                        materialId = returnlist[i].MaterialId;
                        IdCheck.Add(materialId);
                        goto first;
                    }
                }
            }
            catch (Exception)
            {
            }
            return returnlist;
        }
        public List<MaterialNormsModel> GetNorms(int materialId)
        {
            using (db = new SanXuatCheckListEntities())
                return db.MaterialNorms
                    .Where(x => !x.IsDeleted && !x.Material.IsDeleted && !x.Material.MaterialType.IsDeleted && x.MaterialId == materialId)
                    .Select(x => new MaterialNormsModel()
                    {
                        Id = x.Id,
                        MaterialId = x.ObjectId,
                        Name = x.Material.NameTM,
                        Quantities = x.Quantities,
                        UnitId = x.Material.UnitId,
                        UnitName = x.Material.Unit.Name
                    }).ToList();
        }
    }
}
