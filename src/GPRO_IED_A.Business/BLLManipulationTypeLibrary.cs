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
using Hugate.Framework;

namespace SanXuatCheckList.Business
{
    public class BLLManipulationTypeLibrary
    {
        #region constructor
        SanXuatCheckListEntities db;
        static object key = new object();
        private static volatile BLLManipulationTypeLibrary _Instance;
        public static BLLManipulationTypeLibrary Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLManipulationTypeLibrary();

                return _Instance;
            }
        }
        private BLLManipulationTypeLibrary() { }
        #endregion

        public List<ManipulationTypeModel> GetListManipulationType()
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    return db.T_ManipulationTypeLibrary.Where(x => !x.IsDeleted).Select(x => new ManipulationTypeModel()
                    {
                        Id = x.Id,
                        Code = x.Code,
                        Description = x.Description,
                        IsUseMachine = x.IsUseMachine,
                        Name = x.Name,
                        Node = x.Node,
                        ParentId = x.ParentId
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResponseBase InsertOrUpdate(T_ManipulationTypeLibrary model)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var result = new ResponseBase();
                    T_ManipulationTypeLibrary obj = null;
                    bool flag = false;
                    if (CheckExists(model.Name.Trim().ToUpper(), model.Id, true))
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Insert  ", Message = "Tên này đã tồn tại trong Loại Thao Tác này. Vui lòng chọn lại Tên khác !." });
                        flag = true;
                    }
                    if (!string.IsNullOrEmpty(model.Code))
                    {
                        if (CheckExists(model.Code.Trim().ToUpper(), model.Id, false))
                        {
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "Insert  ", Message = "Mã này đã tồn tại trong Loại Thao Tác này. Vui lòng chọn lại Mã khác !." });
                            flag = true;
                        }
                    }
                    if (!flag)
                    {
                        if (model.Id == 0)
                        {
                            obj = new T_ManipulationTypeLibrary();
                            Parse.CopyObject(model, ref obj);
                            obj.Node = obj.Node == "0" ? "0," : obj.Node + obj.ParentId + ",";
                            db.T_ManipulationTypeLibrary.Add(obj);
                        }
                        else
                        {
                            //Update                     
                            obj = db.T_ManipulationTypeLibrary.FirstOrDefault(x => !x.IsDeleted && x.Id == model.Id);
                            if (obj == null)
                            {
                                result.IsSuccess = false;
                                result.Errors.Add(new Error() { MemberName = "Update  ", Message = "Dữ liệu bạn đang thao tác đã bị xóa hoặc không tồn tại. Vui lòng kiểm tra lại !." });
                                flag = true;
                            }
                            else
                            {
                                if (model.IsUseMachine && !obj.IsUseMachine && obj.T_ManipulationLibrary.Count > 0)
                                {
                                    result.IsSuccess = false;
                                    result.Errors.Add(new Error() { MemberName = "Update  ", Message = "Loại Thao Tác này đã có Thao Tác con.\nBạn không thể chuyển sang Loại sử dụng Thiết Bị được.\nVui lòng kiểm tra lại !." });
                                    flag = true;
                                }
                                else
                                {
                                    obj.Name = model.Name;
                                    obj.Code = model.Code;
                                    obj.IsUseMachine = model.IsUseMachine;
                                    obj.Description = model.Description;
                                    obj.UpdatedUser = model.UpdatedUser;
                                    obj.UpdatedDate = model.UpdatedDate;
                                }
                            }
                        }
                        if (!flag)
                        {
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

        private bool CheckExists(string keyword, int Id, bool isCheckName)
        {
            try
            {
                T_ManipulationTypeLibrary maniType = null;
                keyword = keyword.Trim().ToUpper();
                if (isCheckName)
                {
                    maniType = db.T_ManipulationTypeLibrary.FirstOrDefault(x => !x.IsDeleted && x.Id != Id && x.Name.Trim().ToUpper().Equals(keyword));
                }
                else
                {
                    maniType = db.T_ManipulationTypeLibrary.FirstOrDefault(x => !x.IsDeleted && x.Id != Id && x.Code.Trim().ToUpper().Equals(keyword));
                }

                if (maniType == null)
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResponseBase Delete(int Id, int actionUserId)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var result = new ResponseBase();
                    var maniType = db.T_ManipulationTypeLibrary.FirstOrDefault(x => !x.IsDeleted && x.Id == Id);
                    if (maniType != null)
                    {
                        maniType.IsDeleted = true;
                        maniType.DeletedUser = actionUserId;
                        maniType.DeletedDate = DateTime.Now;
                        db.SaveChanges();
                        result.IsSuccess = true;
                        result.Errors.Add(new Error() { MemberName = "", Message = "Xóa Thành Công.!" });
                    }
                    else
                    {
                        result.IsSuccess = true;
                        result.Errors.Add(new Error() { MemberName = "", Message = "Dữ liệu bạn đang thao tác đã bị xóa hoặc không tồn tại.\nVui lòng kiểm tra lại.!" });
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PagedList<ManipulationTypeModel> GetList(string keyWord, int searchBy, int parentId, int startIndexRecord, int pageSize, string sorting)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    if (string.IsNullOrEmpty(sorting))
                        sorting = "Id DESC";

                    var pageNumber = (startIndexRecord / pageSize) + 1;

                    IQueryable<T_ManipulationTypeLibrary> maniTypes = null;

                    if (!string.IsNullOrEmpty(keyWord))
                    {
                        keyWord = keyWord.Trim().ToUpper();
                        switch (searchBy)
                        {
                            case 1:
                                maniTypes = db.T_ManipulationTypeLibrary.Where(x => !x.IsDeleted && x.Name.Trim().ToUpper().Equals(keyWord));
                                break;
                            case 2:
                                maniTypes = db.T_ManipulationTypeLibrary.Where(x => !x.IsDeleted && x.Code.Trim().ToUpper().Equals(keyWord));
                                break;
                        }
                    }
                    else
                        maniTypes = db.T_ManipulationTypeLibrary.Where(x => !x.IsDeleted && x.ParentId == parentId);

                    if (maniTypes != null && maniTypes.Count() > 0)
                        return new PagedList<ManipulationTypeModel>(maniTypes .Select(x => new ManipulationTypeModel()
                        {
                            Id = x.Id,
                            ParentId = x.ParentId,
                            Node = x.Node,
                            Name = x.Name,
                            IsUseMachine = x.IsUseMachine,
                            Description = x.Description,
                            Code = x.Code
                        }).OrderBy(sorting).ToList(), pageNumber, pageSize);
                    else
                        return new PagedList<ManipulationTypeModel>(new List<ManipulationTypeModel>(), pageNumber, pageSize);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<StopPrecisionModel> GetStopPrecisions()
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    return db.T_StopPrecisionLibrary.Where(x => !x.IsDeleted).Select(x => new StopPrecisionModel()
                    {
                        Id = x.Id,
                        Code = x.Code,
                        Description = x.Description,
                        MTM_2 = x.MTM_2,
                        Name = x.Name,
                        StopPrecision = x.StopPrecision,
                        TMUNumber = x.TMUNumber
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<NatureCutsModel> GetNatureCuts()
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    return db.T_NatureCutsLibrary.Where(x => !x.IsDeleted).Select(x => new NatureCutsModel()
                    {
                        Id = x.Id,
                        Code = x.Code,
                        Description = x.Description,
                        Factor = x.Factor,
                        Name = x.Name,
                        Percent = x.Percent
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ApplyPressureLibraryModel> GetApplyPressures()
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    return db.T_ApplyPressureLibrary.Where(x => !x.IsDeleted).Select(x => new ApplyPressureLibraryModel()
                    {
                        Id = x.Id,
                        Description = x.Description,
                        Level = x.Level,
                        Value = x.Value
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
