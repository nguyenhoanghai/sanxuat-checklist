using GPRO.Core.Mvc;
using GPRO.Ultilities;
using SanXuatCheckList.Business.Model;
using SanXuatCheckList.Data;
using Hugate.Framework;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SanXuatCheckList.Business
{
    public class BLLManipulationLibrary
    {
        #region constructor
        SanXuatCheckListEntities db;
        static object key = new object();
        private static volatile BLLManipulationLibrary _Instance;
        public static BLLManipulationLibrary Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLManipulationLibrary();

                return _Instance;
            }
        }
        private BLLManipulationLibrary() { }
        #endregion

        public List<T_ManipulationLibrary> GetListManipulation()
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var maniTypes = db.T_ManipulationLibrary.Where(x => !x.IsDeleted);
                    if (maniTypes != null)
                        return maniTypes.ToList();
                    return new List<T_ManipulationLibrary>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PagedList<ManipulationLibraryModel> GetList(string keyWord, int searchBy, int maniType, int startIndexRecord, int pageSize, string sorting)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    if (string.IsNullOrEmpty(sorting))
                    {
                        sorting = "Id DESC";
                    }
                    var pageNumber = (startIndexRecord / pageSize) + 1;

                    IQueryable<T_ManipulationLibrary> manipulations = null;

                    if (!string.IsNullOrEmpty(keyWord))
                    {
                        keyWord = keyWord.Trim().ToUpper();
                        switch (searchBy)
                        {
                            case 1:
                                manipulations = db.T_ManipulationLibrary.Where(x => !x.IsDeleted && !x.T_ManipulationTypeLibrary.IsDeleted && x.Name.Trim().ToUpper().Contains(keyWord));
                                break;
                            case 2:
                                manipulations = db.T_ManipulationLibrary.Where(x => !x.IsDeleted && !x.T_ManipulationTypeLibrary.IsDeleted && x.Code.Trim().ToUpper().Contains(keyWord));
                                break;
                        }
                    }
                    else
                        manipulations = db.T_ManipulationLibrary.Where(x => !x.IsDeleted && !x.T_ManipulationTypeLibrary.IsDeleted);

                    if (maniType != 0 && manipulations != null && manipulations.Count() > 0)
                        manipulations = manipulations.Where(x => x.ManipulationTypeId == maniType);

                    if (manipulations != null && manipulations.Count() > 0)
                    {
                        var manis = manipulations.Select(x => new ManipulationLibraryModel()
                        {
                            Id = x.Id,
                            Name = x.Name,
                            ManiTypeCode = x.T_ManipulationTypeLibrary.Code,
                            Code = x.Code,
                            Description = x.Description,
                            StandardTMU = x.StandardTMU,
                            UserTMU = x.UserTMU,
                            ManipulationTypeId = x.ManipulationTypeId,
                            ManipulationTypeName = x.T_ManipulationTypeLibrary.Name,
                            EquipmentTypeId = x.EquipmentTypeId,
                            EquipmentName = x.T_EquipmentType.Name,
                            StopPrecisionId = x.StopPrecisionId,
                            ApplyPressureId = x.ApplyPressureId,
                            NatureCutsId = x.NatureCutsId,
                            Distance = x.Distance
                        }).OrderBy(sorting).ToList(); ;
                        return new PagedList<ManipulationLibraryModel>(manis, pageNumber, pageSize);
                    }
                    else
                        return new PagedList<ManipulationLibraryModel>(new List<ManipulationLibraryModel>(), pageNumber, pageSize);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResponseBase InsertOrUpdate(ManipulationLibraryModel model, List<ManipulationEquipmentModel> TMUModel, List<FileUploadModel> fileModel)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var result = new ResponseBase();
                    T_ManipulationLibrary mani = null;
                    T_ManipulationEquipment maniEquipment = null;
                    T_ManipulationFile maniFile = null;
                    T_File file = null;
                    if (CheckExists(model.Name.Trim().ToUpper(), model.Id, true))
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Insert  ", Message = "Tên Thao Tác này đã tồn tại . Vui lòng chọn lại Tên khác !." });
                        return result;
                    }
                    if (!string.IsNullOrEmpty(model.Code))
                    {
                        if (CheckExists(model.Code.Trim().ToUpper(), model.Id, false))
                        {
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "Insert  ", Message = "Mã Thao Tác này đã tồn tại này. Vui lòng chọn lại Mã khác !." });
                            return result;
                        }
                    }
                    if (model.Id == 0)
                    {
                        #region add
                        mani = new T_ManipulationLibrary();
                        mani.Id = model.Id;
                        mani.Name = model.Name;
                        mani.Code = model.Code;
                        mani.Description = model.Description;
                        mani.ManipulationTypeId = model.ManipulationTypeId;
                        mani.StandardTMU = Math.Round(model.StandardTMU, 2);
                        mani.UserTMU = Math.Round(model.UserTMU, 2);
                        if (model.Distance == 0)
                        {
                            mani.EquipmentTypeId = null;
                            mani.StopPrecisionId = null;
                            mani.ApplyPressureId = null;
                            mani.NatureCutsId = null;
                            mani.Distance = null;
                        }
                        else
                        {
                            mani.EquipmentTypeId = model.EquipmentTypeId;
                            mani.ApplyPressureId = model.ApplyPressureId;
                            mani.NatureCutsId = model.NatureCutsId;
                            mani.StopPrecisionId = model.StopPrecisionId;
                            mani.Distance = model.Distance;
                        }
                        mani.CreatedUser = model.ActionUser;
                        mani.CreatedDate = DateTime.Now;


                        if (TMUModel != null && TMUModel.Count > 0)
                        {
                            mani.T_ManipulationEquipment = new System.Collections.ObjectModel.Collection<T_ManipulationEquipment>();
                            foreach (var item in TMUModel)
                            {
                                maniEquipment = new T_ManipulationEquipment();
                                Parse.CopyObject(item, ref maniEquipment);
                                maniEquipment.T_ManipulationLibrary = mani;
                                maniEquipment.CreatedUser = mani.CreatedUser;
                                maniEquipment.CreatedDate = mani.CreatedDate;
                                mani.T_ManipulationEquipment.Add(maniEquipment);
                            }
                        }

                        if (fileModel != null && fileModel.Count > 0)
                        {
                            mani.T_ManipulationFile = new System.Collections.ObjectModel.Collection<T_ManipulationFile>();
                            foreach (var item in fileModel)
                            {
                                // add file
                                file = new T_File();
                                Parse.CopyObject(item, ref file);
                                file.T_ManipulationFile = new System.Collections.ObjectModel.Collection<T_ManipulationFile>();
                                file.CreatedUser = mani.CreatedUser;
                                file.CreatedDate = mani.CreatedDate;


                                // add commodity file
                                maniFile = new T_ManipulationFile();
                                maniFile.T_ManipulationLibrary = mani;
                                //maniFile.FileId = file.Id;
                                maniFile.CreatedUser = mani.CreatedUser;
                                maniFile.CreatedDate = mani.CreatedDate;
                                maniFile.T_File = file;
                                mani.T_ManipulationFile.Add(maniFile);
                                file.T_ManipulationFile.Add(maniFile);
                                db.T_File.Add(file);
                            }
                        }

                        db.T_ManipulationLibrary.Add(mani);
                        #endregion
                    }
                    else
                    {
                        #region Update
                        mani = db.T_ManipulationLibrary.FirstOrDefault(x => !x.IsDeleted && x.Id == model.Id);
                        if (mani == null)
                        {
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "Update  ", Message = "Dữ liệu bạn đang thao tác đã bị xóa hoặc không tồn tại. Vui lòng kiểm tra lại !." });
                            return result;
                        }
                        else
                        {
                            mani.Name = model.Name;
                            mani.Code = model.Code;
                            mani.Description = model.Description;
                            mani.ManipulationTypeId = model.ManipulationTypeId;
                            mani.StandardTMU = Math.Round(model.StandardTMU, 2);
                            mani.UserTMU = Math.Round(model.UserTMU, 2);
                            //if (maniModel.Distance == null)
                            //{
                            //    mani.EquipmentTypeId = null;
                            //    mani.StopPrecisionId = null;
                            //    mani.ApplyPressureId = null;
                            //    mani.NatureCutsId = null;
                            //    mani.Distance = null;
                            //}
                            //else
                            //{
                            mani.EquipmentTypeId = model.EquipmentTypeId;
                            mani.ApplyPressureId = model.ApplyPressureId;
                            mani.NatureCutsId = model.NatureCutsId;
                            mani.StopPrecisionId = model.StopPrecisionId;
                            mani.Distance = model.Distance;
                            //}
                            mani.UpdatedUser = model.ActionUser;
                            mani.UpdatedDate = DateTime.Now;

                            //remove old
                            if (model.isListMachineTMUChange)
                            {
                                var maniEquipOld = db.T_ManipulationEquipment.Where(x => !x.IsDeleted && x.ManipulationId == model.Id);
                                if (maniEquipOld != null && maniEquipOld.Count() > 0)
                                {
                                    foreach (var item in maniEquipOld)
                                    {
                                        item.IsDeleted = true;
                                        item.DeletedUser = mani.UpdatedUser;
                                        item.DeletedDate = mani.UpdatedDate;
                                    }
                                }
                                // add new
                                if (TMUModel != null && TMUModel.Count() > 0)
                                {
                                    foreach (var item in TMUModel)
                                    {
                                        maniEquipment = new T_ManipulationEquipment();
                                        Parse.CopyObject(item, ref maniEquipment);
                                        maniEquipment.ManipulationId = mani.Id;
                                        maniEquipment.CreatedUser = mani.UpdatedUser ?? 0;
                                        maniEquipment.CreatedDate = mani.UpdatedDate ?? DateTime.Now;
                                        db.T_ManipulationEquipment.Add(maniEquipment);
                                    }
                                }
                            }

                            #region File
                            if (model.isListAttachFileChange)
                            {
                                var maniFiles = db.T_ManipulationFile.Where(x => !x.IsDeleted && x.ManipulationId == mani.Id);
                                if (maniFiles != null && maniFiles.Count() > 0)
                                {
                                    foreach (var item in maniFiles)
                                    {
                                        item.IsDeleted = true;
                                        item.DeletedUser = mani.UpdatedUser;
                                        item.DeletedDate = mani.UpdatedDate;
                                    }
                                }

                                if (fileModel != null && fileModel.Count > 0)
                                {
                                    // mani.T_ManipulationFile = new System.Collections.ObjectModel.Collection<T_ManipulationFile>();
                                    foreach (var item in fileModel)
                                    {
                                        // add file
                                        file = new T_File();
                                        Parse.CopyObject(item, ref file);
                                        file.T_ManipulationFile = new System.Collections.ObjectModel.Collection<T_ManipulationFile>();
                                        file.CreatedUser = mani.UpdatedUser ?? 0;
                                        file.CreatedDate = mani.UpdatedDate ?? DateTime.Now;


                                        // add commodity file
                                        maniFile = new T_ManipulationFile();
                                        //  maniFile.T_ManipulationLibrary = mani; 
                                        maniFile.ManipulationId = model.Id;
                                        maniFile.CreatedUser = mani.UpdatedUser ?? 0;
                                        maniFile.CreatedDate = mani.UpdatedDate ?? DateTime.Now;
                                        maniFile.T_File = file;
                                        //mani.T_ManipulationFile.Add(maniFile);
                                        file.T_ManipulationFile.Add(maniFile);
                                        db.T_File.Add(file);
                                    }
                                }
                            }
                            #endregion
                            #endregion
                        }
                    }
                    db.SaveChanges();
                    result.IsSuccess = true;
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
                T_ManipulationLibrary maniType = null;
                keyword = keyword.Trim().ToUpper();
                if (isCheckName)
                {
                    maniType = db.T_ManipulationLibrary.FirstOrDefault(x => !x.IsDeleted && x.Id != Id && x.Name.Trim().ToUpper().Equals(keyword));
                }
                else
                {
                    maniType = db.T_ManipulationLibrary.FirstOrDefault(x => !x.IsDeleted && x.Id != Id && x.Code.Trim().ToUpper().Equals(keyword));
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
                    var maniType = db.T_ManipulationLibrary.FirstOrDefault(x => !x.IsDeleted && x.Id == Id);
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

        public PagedList<ManipulationLibraryModel> GetList(string keyWord, int searchBy, int startIndexRecord, int pageSize, string sorting)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    if (string.IsNullOrEmpty(sorting))
                    {
                        sorting = "Id DESC";
                    }
                    var pageNumber = (startIndexRecord / pageSize) + 1;

                    IQueryable<T_ManipulationLibrary> manipulations = null;

                    if (!string.IsNullOrEmpty(keyWord))
                    {
                        keyWord = keyWord.Trim().ToUpper();
                        switch (searchBy)
                        {
                            case 1:
                                manipulations = db.T_ManipulationLibrary.Where(x => !x.IsDeleted && x.Name.Trim().ToUpper().Equals(keyWord));
                                break;
                            case 2:
                                manipulations = db.T_ManipulationLibrary.Where(x => !x.IsDeleted && x.Code.Trim().ToUpper().Equals(keyWord));
                                break;
                        }
                    }
                    else
                        manipulations = db.T_ManipulationLibrary.Where(x => !x.IsDeleted);

                    if (manipulations != null && manipulations.Count() > 0)
                    {
                        var manis = manipulations .Select(x => new ManipulationLibraryModel()
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Code = x.Code,
                            ManiTypeCode = x.T_ManipulationTypeLibrary.Code,
                            Description = x.Description,
                            StandardTMU = x.StandardTMU,
                            UserTMU = x.UserTMU,
                            ManipulationTypeId = x.ManipulationTypeId,
                            EquipmentTypeId = x.EquipmentTypeId,
                            EquipmentName = x.T_EquipmentType.Name,
                            StopPrecisionId = x.StopPrecisionId,
                            ApplyPressureId = x.ApplyPressureId,
                            NatureCutsId = x.NatureCutsId,
                            Distance = x.Distance
                        }).OrderBy(sorting).ToList();
                        return new PagedList<ManipulationLibraryModel>(manis, pageNumber, pageSize);
                    }
                    else
                        return new PagedList<ManipulationLibraryModel>(new List<ManipulationLibraryModel>(), pageNumber, pageSize);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PagedList<ManipulationEquipmentModel> GetManipulationEquipment(int Id, int startIndexRecord, int pageSize, string sorting)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    if (string.IsNullOrEmpty(sorting))
                    {
                        sorting = "Id DESC";
                    }
                    var manis = db.T_ManipulationEquipment.Where(x => !x.IsDeleted && x.ManipulationId == Id).OrderByDescending(x => x.CreatedDate).Select(x => new ManipulationEquipmentModel()
                    {
                        Id = x.Id,
                        ManipulationId = x.ManipulationId,
                        MachineTMU = x.MachineTMU,
                        UserTMU = x.UserTMU,
                        Note = x.Note,
                        EquipmentId = x.EquipmentId,
                        EquipmentName = x.T_Equipment.Name
                    }).OrderBy(sorting).ToList();
                    var pageNumber = (startIndexRecord / pageSize) + 1;
                    return new PagedList<ManipulationEquipmentModel>(manis, pageNumber, pageSize);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ManipulationEquipmentModel> GetManipulationEquipment(int Id)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var manis = db.T_ManipulationEquipment.Where(x => !x.IsDeleted && x.ManipulationId == Id).OrderBy(x => x.CreatedDate).Select(x => new ManipulationEquipmentModel()
                    {
                        Id = x.Id,
                        ManipulationId = x.ManipulationId,
                        MachineTMU = x.MachineTMU,
                        UserTMU = x.UserTMU,
                        Note = x.Note,
                        EquipmentId = x.EquipmentId,
                        EquipmentName = x.T_Equipment.Name
                    }).ToList();
                    return manis;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<T_File> GetManipulationFileById(int Id)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var filesID = db.T_ManipulationFile.Where(x => !x.IsDeleted && x.ManipulationId == Id && !x.T_ManipulationLibrary.IsDeleted && !x.T_File.IsDeleted).Select(x => x.FileId).ToList();
                    if (filesID != null && filesID.Count > 0)
                    {
                        var files = db.T_File.Where(x => !x.IsDeleted && filesID.Contains(x.Id)).ToList();
                        if (files != null && files.Count() > 0)
                            return files.ToList();
                        return null;
                    }
                    else
                        return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<string> GetListManipulationCode()
        {
            using (db = new SanXuatCheckListEntities())
            {
                List<string> listCode = null;
                try
                {
                    listCode = db.T_ManipulationLibrary.Where(x => !x.IsDeleted && !x.T_ManipulationTypeLibrary.IsDeleted).Select(x => x.Code).ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return listCode;
            }
        }

        public ResponseBase GetManipulationEquipmentInfoByCode(int equipmentId, int equiptypedefaultId, int applyPressure, string code)
        {
            ResponseBase result = new ResponseBase();
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    if (!string.IsNullOrEmpty(code))
                    {
                        ManipulationLibrarySmallModel model = null;
                        code = code.Trim().ToUpper();
                        if (code.Length < 4)
                            result.Errors.Add(new Error() { Message = "Mã nhập không đúng định dạng.", MemberName = "GetManipulationEquipmentInfoByCode" });
                        else
                        {

                            string stopPrecisionCode = code.Substring(code.Length - 1, 1);
                            if (equipmentId == 0)
                                result.Errors.Add(new Error() { Message = "Không tìm thấy thông tin thiết bị. Vui lòng kiểm tra lại thông tin thiết bị", MemberName = "GetManipulationEquipmentInfoByCode" });
                            else if (equiptypedefaultId == 0)
                                result.Errors.Add(new Error() { Message = "Không xác định được loại thiết bị. Vui lòng kiểm tra lại thông tin thiết bị", MemberName = "GetManipulationEquipmentInfoByCode" });
                            else
                            {
                                var stopPrecision = db.T_StopPrecisionLibrary.FirstOrDefault(c => c.Code.Trim().ToUpper().Equals(stopPrecisionCode.Trim().ToUpper()) && !c.IsDeleted);
                                if (stopPrecision == null)
                                    result.Errors.Add(new Error() { Message = "Không tìm thấy thông tin độ dừng chính xác.", MemberName = "GetManipulationEquipmentInfoByCode" });
                                else
                                {
                                    float distance = 0;
                                    if (code.Substring(0, 2).Equals("SE"))
                                    {
                                        float.TryParse(code.Substring(2, code.Length - 3), out distance);
                                        if (distance == 0)
                                            result.Errors.Add(new Error() { Message = "Thông tin khoảng cách may không chính xác, hoặc bằng 0.", MemberName = "GetManipulationEquipmentInfoByCode" });
                                        else
                                        {
                                            model = new ManipulationLibrarySmallModel();
                                            model.Code = code;
                                            model.Distance = distance;
                                            model.StandardTMU = BLLEquipment.Instance.CalculationMachineTMU(equipmentId, equiptypedefaultId, distance, stopPrecision.TMUNumber, 0, 0);

                                            if (double.IsNaN(model.StandardTMU))
                                                result.Errors.Add(new Error() { Message = "Thông số thiết bị không nhận được hoặc bằng 0.Vui lòng kiểm tra lại thông số thiết bị", MemberName = "GetManipulationEquipmentInfoByCode" });
                                            else
                                            {
                                                model.Description = "May một đoạn " + distance.ToString() + " cm.";
                                                model.Name = model.Description;
                                                result.Data = model;
                                                result.IsSuccess = true;
                                            }
                                        }
                                    }
                                    else if (code.Substring(0, 1).Equals("C"))
                                    {
                                        float.TryParse(code.Substring(1, code.Length - 3), out distance);
                                        string natureCutCode = code.Substring(code.Length - 2, 1);
                                        var natureCut = db.T_NatureCutsLibrary.FirstOrDefault(c => c.Code.Trim().ToUpper().Equals(natureCutCode.Trim().ToUpper()) && !c.IsDeleted);
                                        if (applyPressure == 0)
                                            result.Errors.Add(new Error() { Message = "Không tìm thấy thông tin số lớp cắt.", MemberName = "GetManipulationEquipmentInfoByCode" });
                                        else if (natureCut == null)
                                            result.Errors.Add(new Error() { Message = "Không tìm thấy thông tin tính chất căt.", MemberName = "GetManipulationEquipmentInfoByCode" });
                                        else if (distance == 0)
                                            result.Errors.Add(new Error() { Message = "Thông tin khoảng cách may không chính xác, hoặc bằng 0.", MemberName = "GetManipulationEquipmentInfoByCode" });
                                        else
                                        {
                                            model = new ManipulationLibrarySmallModel();
                                            model.Code = code;
                                            model.Distance = distance;
                                            model.StandardTMU = BLLEquipment.Instance.CalculationMachineTMU(equipmentId, equiptypedefaultId, distance, stopPrecision.TMUNumber, applyPressure, natureCut.Factor);
                                            if (double.IsNaN(model.StandardTMU))
                                                result.Errors.Add(new Error() { Message = "Thông số thiết bị không nhận được hoặc bằng 0.Vui lòng kiểm tra lại thông số thiết bị", MemberName = "GetManipulationEquipmentInfoByCode" });
                                            else
                                            {
                                                model.Description = "Cắt một đoạn " + distance.ToString() + " cm.";
                                                model.Name = model.Description;
                                                result.Data = model;
                                                result.IsSuccess = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (result.Errors.Count > 0)
                            result.IsSuccess = false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public ResponseBase GetAllManipulation()
        {
            var result = new ResponseBase();
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var list = db.T_ManipulationLibrary.Where(x => !x.IsDeleted && !x.T_ManipulationTypeLibrary.IsDeleted).Select(x => new ManipulationLibraryModel()
                    {
                        Id = x.Id,
                        Code = x.Code,
                        Name = x.Name,
                        Description = x.Description,
                        ManipulationTypeId = x.ManipulationTypeId,
                        isUseMachine = x.T_ManipulationTypeLibrary.IsUseMachine,
                        StandardTMU = x.StandardTMU,
                        UserTMU = x.UserTMU
                    });
                    if (list != null && list.Count() > 0)
                        result.Data = list.ToList();
                    result.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

    }
}
