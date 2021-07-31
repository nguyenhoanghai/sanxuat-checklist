using GPRO.Core.Mvc;
using GPRO.Ultilities;
using SanXuatCheckList.Business.Model;
using SanXuatCheckList.Data;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using SanXuatCheckList.Business.Enum;
using Hugate.Framework;

namespace SanXuatCheckList.Business
{
    public class BLLEquipment
    {
        #region constructor
        SanXuatCheckListEntities db;
        static object key = new object();
        private static volatile BLLEquipment _Instance;
        public static BLLEquipment Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLEquipment();

                return _Instance;
            }
        }
        private BLLEquipment() { }
        #endregion

        bool checkPermis(T_Equipment obj, int actionUser, bool isOwner)
        {
            if (isOwner) return true;
            return obj.CreatedUser == actionUser;
        }

        public PagedList<ModelEquipment> GetList(string keyWord, int startIndexRecord, int pageSize, string sorting, int companyId)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    if (string.IsNullOrEmpty(sorting))
                        sorting = "CurrentDate DESC";

                    IQueryable<T_Equipment> equips = null;
                    if (!string.IsNullOrEmpty(keyWord))
                    {
                        keyWord = keyWord.Trim().ToUpper();
                        equips = db.T_Equipment.Where(x => !x.IsDeleted && (x.Name.Trim().ToUpper().Contains(keyWord) || x.Code.Trim().ToUpper().Contains(keyWord)));
                    }
                    else
                        equips = db.T_Equipment.Where(c => !c.IsDeleted);

                    var Equipments = equips.Select(c => new ModelEquipment()
                    {
                        Id = c.Id,
                        Code = c.Code,
                        Name = c.Name,
                        Expend = c.Expend,
                        EquipmentTypeId = c.EquipmentTypeId,
                        Description = c.Description,
                        EquipmentTypeName = c.T_EquipmentType.Name,
                        EquipTypeDefaultId = c.T_EquipmentType.EquipTypeDefaultId,
                        EquipmentGroupId = c.EquipmentGroupId,
                        EGroupName = c.T_EquipmentGroup.GroupName,
                        CurrentDate = c.CreatedDate ?? DateTime.Now
                    }).OrderBy(sorting).ToList();

                    var pageNumber = (startIndexRecord / pageSize) + 1;

                    PagedList<ModelEquipment> returnList = new PagedList<ModelEquipment>(Equipments, pageNumber, pageSize);
                    foreach (var item in returnList)
                    {
                        item.EquipAtts = GetAtribuleByEquipmentId(item.Id, item.EquipmentTypeId);
                    }
                    return returnList;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ModelAtribute> GetAtribuleByEquipmentId(int equipmentId, int equipTypeId)
        {
            List<ModelAtribute> atribute = new List<ModelAtribute>();
            try
            {
                var modelEquipmentTypeAtt = db.T_EquipmentTypeAttribute.Where(c => !c.IsDeleted && c.EquipmentTypeId == equipTypeId).Select(c => new ModelEquipmentTypeAttribute()
                {
                    Id = c.Id,
                    Name = c.Name,
                    EquipmentTypeId = c.EquipmentTypeId,
                    EquipmentTypeName = c.T_EquipmentType.Name,
                    OrderIndex = c.OrderIndex,
                    IsDefault = c.IsDefault,
                    IsUseForTime = c.IsUseForTime,
                }).OrderBy(x => x.OrderIndex).ToList();


                var modelEquipmentAtt = db.T_EquipmentAttribute.FirstOrDefault(x => !x.IsDeleted && x.EquipmentId == equipmentId);
                int lengh = modelEquipmentTypeAtt.Count();

                if (modelEquipmentAtt != null)
                {
                    if (1 <= lengh)
                    {
                        atribute.Add(new ModelAtribute() { Name = modelEquipmentTypeAtt[0].Name, Value = modelEquipmentAtt.Column1 ?? "" });
                    }
                    if (2 <= lengh)
                    {
                        atribute.Add(new ModelAtribute() { Name = modelEquipmentTypeAtt[1].Name, Value = modelEquipmentAtt.Column2 ?? "" });
                    }
                    if (3 <= lengh)
                    {
                        atribute.Add(new ModelAtribute() { Name = modelEquipmentTypeAtt[2].Name, Value = modelEquipmentAtt.Column3 ?? "" });
                    }
                    if (4 <= lengh)
                    {
                        atribute.Add(new ModelAtribute() { Name = modelEquipmentTypeAtt[3].Name, Value = modelEquipmentAtt.Column4 ?? "" });
                    }
                    if (5 <= lengh)
                    {
                        atribute.Add(new ModelAtribute() { Name = modelEquipmentTypeAtt[4].Name, Value = modelEquipmentAtt.Column5 ?? "" });
                    }
                    if (6 <= lengh)
                    {
                        atribute.Add(new ModelAtribute() { Name = modelEquipmentTypeAtt[5].Name, Value = modelEquipmentAtt.Column6 ?? "" });
                    }
                    if (7 <= lengh)
                    {
                        atribute.Add(new ModelAtribute() { Name = modelEquipmentTypeAtt[6].Name, Value = modelEquipmentAtt.Column7 ?? "" });
                    }
                    if (8 <= lengh)
                    {
                        atribute.Add(new ModelAtribute() { Name = modelEquipmentTypeAtt[7].Name, Value = modelEquipmentAtt.Column8 ?? "" });
                    }
                    if (9 <= lengh)
                    {
                        atribute.Add(new ModelAtribute() { Name = modelEquipmentTypeAtt[8].Name, Value = modelEquipmentAtt.Column9 ?? "" }); ;
                    }
                    if (10 <= lengh)
                    {
                        atribute.Add(new ModelAtribute() { Name = modelEquipmentTypeAtt[9].Name, Value = modelEquipmentAtt.Column10 ?? "" });
                    }
                    if (11 <= lengh)
                    {
                        atribute.Add(new ModelAtribute() { Name = modelEquipmentTypeAtt[10].Name, Value = modelEquipmentAtt.Column11 ?? "" });
                    }
                    if (12 <= lengh)
                    {
                        atribute.Add(new ModelAtribute() { Name = modelEquipmentTypeAtt[11].Name, Value = modelEquipmentAtt.Column12 ?? "" });
                    }
                    if (13 <= lengh)
                    {
                        atribute.Add(new ModelAtribute() { Name = modelEquipmentTypeAtt[12].Name, Value = modelEquipmentAtt.Column13 ?? "" });
                    }
                    if (14 <= lengh)
                    {
                        atribute.Add(new ModelAtribute() { Name = modelEquipmentTypeAtt[13].Name, Value = modelEquipmentAtt.Column14 ?? "" });
                    }
                    if (15 <= lengh)
                    {
                        atribute.Add(new ModelAtribute() { Name = modelEquipmentTypeAtt[14].Name, Value = modelEquipmentAtt.Column15 ?? "" });
                    }
                    if (16 <= lengh)
                    {
                        atribute.Add(new ModelAtribute() { Name = modelEquipmentTypeAtt[15].Name, Value = modelEquipmentAtt.Column16 ?? "" });
                    }
                    if (17 <= lengh)
                    {
                        atribute.Add(new ModelAtribute() { Name = modelEquipmentTypeAtt[16].Name, Value = modelEquipmentAtt.Column17 ?? "" });
                    }
                    if (18 <= lengh)
                    {
                        atribute.Add(new ModelAtribute() { Name = modelEquipmentTypeAtt[17].Name, Value = modelEquipmentAtt.Column18 ?? "" });
                    }
                    if (19 <= lengh)
                    {
                        atribute.Add(new ModelAtribute() { Name = modelEquipmentTypeAtt[18].Name, Value = modelEquipmentAtt.Column19 ?? "" });
                    }
                    if (20 <= lengh)
                    {
                        atribute.Add(new ModelAtribute() { Name = modelEquipmentTypeAtt[19].Name, Value = modelEquipmentAtt.Column20 ?? "" });
                    }
                }
            }
            catch (Exception ex)
            { 
            }
            return atribute;
        }

        private bool CheckEquipmentName(string EquipmentName, int CompanyId, int Id)
        {
            var checkResult = false;
            try
            {
                var checkName = db.T_Equipment.FirstOrDefault(c => !c.IsDeleted && c.Id != Id && c.CompanyId == CompanyId && c.Name.Trim().ToUpper().Equals(EquipmentName.Trim().ToUpper()));
                if (checkName == null)
                    checkResult = true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return checkResult;
        }

        public ResponseBase Create(ModelEquipment model, int companyId, List<string> a)
        {
            ResponseBase result = new ResponseBase();
            result.IsSuccess = false;
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    if (model != null)
                    {
                        if (CheckEquipmentName(model.Name, model.Id, model.CompanyId))
                        {

                            var obj = new T_Equipment();
                            Parse.CopyObject(model, ref obj);
                            obj.CreatedUser = model.ActionUser;
                            obj.CreatedDate = DateTime.Now;
                            obj.T_EquipmentAttribute = new Collection<T_EquipmentAttribute>();

                            var eAtt = new T_EquipmentAttribute();
                            eAtt.EquipmentTypeId = obj.EquipmentTypeId;
                            eAtt.CreatedDate = DateTime.Now;
                            eAtt.CreatedUser = obj.CreatedUser;
                            eAtt.T_Equipment = obj;
                            if (!string.IsNullOrEmpty(a[0]))
                                eAtt.Column1 = a[0];
                            if (!string.IsNullOrEmpty(a[1]))
                                eAtt.Column2 = a[1];
                            if (!string.IsNullOrEmpty(a[2]))
                                eAtt.Column3 = a[2];
                            if (!string.IsNullOrEmpty(a[3]))
                                eAtt.Column4 = a[3];
                            if (!string.IsNullOrEmpty(a[4]))
                                eAtt.Column5 = a[4];
                            if (!string.IsNullOrEmpty(a[5]))
                                eAtt.Column6 = a[5];
                            if (!string.IsNullOrEmpty(a[6]))
                                eAtt.Column7 = a[6];
                            if (!string.IsNullOrEmpty(a[7]))
                                eAtt.Column8 = a[7];
                            if (!string.IsNullOrEmpty(a[8]))
                                eAtt.Column9 = a[8];
                            if (!string.IsNullOrEmpty(a[9]))
                                eAtt.Column10 = a[9];
                            if (!string.IsNullOrEmpty(a[10]))
                                eAtt.Column11 = a[10];
                            if (!string.IsNullOrEmpty(a[11]))
                                eAtt.Column12 = a[11];
                            if (!string.IsNullOrEmpty(a[12]))
                                eAtt.Column13 = a[12];
                            if (!string.IsNullOrEmpty(a[13]))
                                eAtt.Column14 = a[13];
                            if (!string.IsNullOrEmpty(a[14]))
                                eAtt.Column15 = a[14];
                            if (!string.IsNullOrEmpty(a[15]))
                                eAtt.Column16 = a[15];
                            if (!string.IsNullOrEmpty(a[16]))
                                eAtt.Column17 = a[16];
                            if (!string.IsNullOrEmpty(a[17]))
                                eAtt.Column18 = a[17];
                            if (!string.IsNullOrEmpty(a[18]))
                                eAtt.Column19 = a[18];
                            if (!string.IsNullOrEmpty(a[19]))
                                eAtt.Column20 = a[19];
                            obj.T_EquipmentAttribute.Add(eAtt);
                            db.T_Equipment.Add(obj);
                            db.SaveChanges();
                            result.IsSuccess = true;
                        }
                        else
                        {
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "Create Equipment", Message = "Tên Đã Tồn Tại,Vui Lòng Chọn Tên Khác" });
                        }
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Create Equipment", Message = "Đối Tượng Không tồn tại" });
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public ResponseBase Update(ModelEquipment model, int companyId, List<string> a, bool isOwner)
        {
            ResponseBase result = new ResponseBase();
            result.IsSuccess = false;
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    if (!CheckEquipmentName(model.Name, model.Id, model.CompanyId))
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "UpdateEquipment", Message = "Trùng Tên. Vui lòng chọn lại" });
                    }
                    else
                    {
                        T_Equipment obj = db.T_Equipment.FirstOrDefault(x => x.Id == model.Id && !x.IsDeleted);
                        if (obj != null)
                        {
                            if (!checkPermis(obj, model.ActionUser,isOwner))
                            {
                                result.IsSuccess = false;
                                result.Errors.Add(new Error() { MemberName = "update", Message = "Bạn không phải là người tạo thiết bị này nên bạn không cập nhật được thông tin cho thiết bị này." });
                            }
                            else
                            {
                                obj.Code = model.Code;
                                obj.Name = model.Name;
                                obj.Expend = model.Expend;
                                obj.EquipmentGroupId = model.EquipmentGroupId;
                                obj.EquipmentTypeId = model.EquipmentTypeId;
                                obj.Description = model.Description;
                                obj.CompanyId = model.CompanyId;
                                obj.UpdatedDate = DateTime.Now;
                                obj.UpdatedUser = model.ActionUser;

                                var id = db.T_EquipmentAttribute.FirstOrDefault(x => !x.IsDeleted && x.EquipmentId == obj.Id).Id;
                                var eAttr = db.T_EquipmentAttribute.FirstOrDefault(x => !x.IsDeleted && x.Id == id);
                                eAttr.Id = id;
                                eAttr.EquipmentTypeId = obj.EquipmentTypeId;
                                eAttr.EquipmentId = obj.Id;
                                if (!string.IsNullOrEmpty(a[0]))
                                {
                                    eAttr.Column1 = a[0];
                                }
                                if (!string.IsNullOrEmpty(a[1]))
                                {
                                    eAttr.Column2 = a[1];
                                }
                                if (!string.IsNullOrEmpty(a[2]))
                                {
                                    eAttr.Column3 = a[2];
                                }
                                if (!string.IsNullOrEmpty(a[3]))
                                {
                                    eAttr.Column4 = a[3];
                                }
                                if (!string.IsNullOrEmpty(a[4]))
                                {
                                    eAttr.Column5 = a[4];
                                }
                                if (!string.IsNullOrEmpty(a[5]))
                                {
                                    eAttr.Column6 = a[5];
                                }
                                if (!string.IsNullOrEmpty(a[6]))
                                {
                                    eAttr.Column7 = a[6];
                                }
                                if (!string.IsNullOrEmpty(a[7]))
                                {
                                    eAttr.Column8 = a[7];
                                }
                                if (!string.IsNullOrEmpty(a[8]))
                                {
                                    eAttr.Column9 = a[8];
                                }
                                if (!string.IsNullOrEmpty(a[9]))
                                {
                                    eAttr.Column10 = a[9];
                                }
                                if (!string.IsNullOrEmpty(a[10]))
                                {
                                    eAttr.Column11 = a[10];
                                }
                                if (!string.IsNullOrEmpty(a[11]))
                                {
                                    eAttr.Column12 = a[11];
                                }
                                if (!string.IsNullOrEmpty(a[12]))
                                {
                                    eAttr.Column13 = a[12];
                                }
                                if (!string.IsNullOrEmpty(a[13]))
                                {
                                    eAttr.Column14 = a[13];
                                }
                                if (!string.IsNullOrEmpty(a[14]))
                                {
                                    eAttr.Column15 = a[14];
                                }
                                if (!string.IsNullOrEmpty(a[15]))
                                {
                                    eAttr.Column16 = a[15];
                                }
                                if (!string.IsNullOrEmpty(a[16]))
                                {
                                    eAttr.Column17 = a[16];
                                }
                                if (!string.IsNullOrEmpty(a[17]))
                                {
                                    eAttr.Column18 = a[17];
                                }
                                if (!string.IsNullOrEmpty(a[18]))
                                {
                                    eAttr.Column19 = a[18];
                                }
                                if (!string.IsNullOrEmpty(a[19]))
                                {
                                    eAttr.Column20 = a[19];
                                }
                                eAttr.UpdatedDate = DateTime.Now;
                                eAttr.UpdatedUser = model.ActionUser;
                                db.SaveChanges();
                                result.IsSuccess = true;
                            }
                        }
                        else
                        {
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "UpdateEquipment", Message = "Thông tin nhập không đúng Vui lòng kiểm tra lại!" });
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
                    var Equipment = db.T_Equipment.FirstOrDefault(c => !c.IsDeleted && c.Id == id);
                    if (Equipment != null)
                    {
                        if (!checkPermis(Equipment, userId,isOwner))
                        {
                            rs.IsSuccess = false;
                            rs.Errors.Add(new Error() { MemberName = "Delete Customer Type", Message = "Bạn không phải là người tạo thiết bị này nên bạn không xóa được xóa thiết bị này." });
                        }
                        else
                        {
                            Equipment.IsDeleted = true;
                            Equipment.DeletedUser = userId;
                            Equipment.DeletedDate = DateTime.Now;
                            db.SaveChanges();
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
        public double CalculationMachineTMU(int equipmentId, int equipmentType, float distance, double stopPrecision, int applyPressure, double natureCut)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var eAtrr = db.T_EquipmentAttribute.FirstOrDefault(x => !x.IsDeleted && x.EquipmentId == equipmentId); 
                    double machineTMU = 0;
                    if (equipmentType == (int)eEquipType_Default.isSewingMachine)
                    {
                        // tinh theo cong thuc may may
                        var RPM = double.Parse(eAtrr.Column1);
                        var St = double.Parse(eAtrr.Column2);
                        var BST = (St / (RPM * 0.0006));
                        var SST = (BST * distance) + 18 + stopPrecision;
                        machineTMU = SST;
                    }
                    else
                    {
                        // tinh theo cong thuc may cat
                        var F = applyPressure;
                        var CT = natureCut;
                        var Weight = double.Parse(eAtrr.Column1);
                        var BCT = Math.Sqrt((Weight * 0.01) / applyPressure) * 27.8;
                        var C = (BCT * natureCut * distance) + stopPrecision;
                        machineTMU = C;
                    }
                    return Math.Round(machineTMU, 3);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
    }
}
