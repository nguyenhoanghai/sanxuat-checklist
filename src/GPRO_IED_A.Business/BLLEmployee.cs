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
    public class BLLEmployee
    {
        #region constructor
        SanXuatCheckListEntities db;
        static object key = new object();
        private static volatile BLLEmployee _Instance;
        public static BLLEmployee Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLEmployee();

                return _Instance;
            }
        }
        private BLLEmployee() { }
        #endregion

        bool checkPermis(Employee obj, int actionUser, bool isOwner)
        {
            if (isOwner) return true;
            return obj.CreatedUser == actionUser;
        }
        public ResponseBase Delete(int Id, int actionUserId, int companyId, bool isOwner)
        {
            ResponseBase result;
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    result = new ResponseBase();
                    var obj = db.Employee.Where(c => c.CompanyId == companyId && !c.IsDeleted && c.Id == Id).FirstOrDefault();
                    if (obj == null)
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "Delete", Message = "Dữ liệu đang thao tác không tồn tại. Vui lòng kiểm tra lại!" });
                    }
                    else
                    {
                        if (!checkPermis(obj, actionUserId, isOwner))
                        {
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "Delete Customer Type", Message = "Bạn không phải là người tạo nhân viên này nên bạn không xóa được xóa nhân viên này." });
                        }
                        else
                        {
                            obj.IsDeleted = true;
                            obj.DeletedUser = actionUserId;
                            obj.DeletedDate = DateTime.Now;
                            db.SaveChanges();
                            result.IsSuccess = true;
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

        public ResponseBase CreateOrUpdate(EmployeeModel model, bool isOwner)
        {
            ResponseBase result = null;
            Employee employee = null;
            bool flag = false;
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    result = new ResponseBase();
                    if (!string.IsNullOrEmpty(model.Code))  // kiem tra ma nhan vien neu có
                    {
                        if (CheckExists(model.Code, model.CompanyId, model.Id, 1, db))
                        {
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "add new", Message = "Mã nhân viên này đã tồn tại. Vui lòng chọn lại Mã Khác" });
                            flag = true;
                        }
                    }
                    if (!flag)
                    {
                        if (model.Id == 0)
                        {
                            employee = new Employee();
                            Parse.CopyObject(model, ref employee);
                            if (model.Picture != null)
                                employee.Image = model.Picture; // hinh 
                            employee.CreatedUser = model.ActionUser;
                            employee.CreatedDate = DateTime.Now;
                            db.Employee.Add(employee);
                            db.SaveChanges();
                            result.IsSuccess = true;
                        }
                        else
                        {
                            employee = db.Employee.FirstOrDefault(x => !x.IsDeleted && x.Id == model.Id && x.CompanyId == model.CompanyId);
                            if (employee != null)
                            {
                                if (!checkPermis(employee, model.ActionUser, isOwner))
                                {
                                    result.IsSuccess = false;
                                    result.Errors.Add(new Error() { MemberName = "update", Message = "Bạn không phải là người tạo nhân viên này nên bạn không cập nhật được thông tin cho nhân viên này." });
                                }
                                else
                                {
                                    employee.UserId = model.UserId;
                                    employee.FirstName = model.FirstName;
                                    employee.LastName = model.LastName;
                                    employee.Gender = model.Gender;
                                    employee.Birthday = model.Birthday;
                                    employee.Code = model.Code;
                                    employee.Mobile = model.Mobile;
                                    employee.Email = model.Email;
                                    employee.CompanyId = model.CompanyId;
                                    if (!string.IsNullOrEmpty(model.Picture))
                                        employee.Image = model.Picture.Trim(); // hinh
                                    employee.UpdatedUser = model.ActionUser;
                                    employee.UpdatedDate = DateTime.Now;
                                    db.SaveChanges();
                                    result.IsSuccess = true;
                                }
                            }
                            else
                            {
                                result.IsSuccess = false;
                                result.Errors.Add(new Error() { MemberName = "update employee", Message = "Không tìm thấy Nhân Viên này. \nCó thể nhân viên đã bị xóa hoặc không tồn tại. \nVui lòng kiểm tra lại dữ liệu." });
                            }
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

        private bool CheckExists(string checkValue, int? companyId, int employeeId, int typeOfCheck, SanXuatCheckListEntities db)
        {
            try
            {
                Employee employee = null;
                checkValue = checkValue.Trim().ToUpper();
                if (typeOfCheck == 1)
                    employee = db.Employee.FirstOrDefault(x => !x.IsDeleted && x.CompanyId == companyId && x.Id != employeeId && x.Code.Trim().ToUpper().Equals(checkValue));

                if (employee == null)
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public PagedList<EmployeeModel> Gets(string keyWord, int companyId, int startIndexRecord, int pageSize, string sorting)
        {
            PagedList<EmployeeModel> pagelistReturn = null;
            List<EmployeeModel> employees = null;
            try
            {
                if (string.IsNullOrEmpty(sorting))
                    sorting = "Id DESC";
                employees = GetEmployees(keyWord, sorting, companyId);
                var pageNumber = (startIndexRecord / pageSize) + 1;
                pagelistReturn = new PagedList<EmployeeModel>(employees, pageNumber, pageSize);
                return pagelistReturn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<EmployeeModel> GetEmployees(string keyWord, string sorting, int companyId)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    IQueryable<Employee> objs = db.Employee.Where(x => !x.IsDeleted && x.CompanyId == companyId);
                    if (!string.IsNullOrEmpty(keyWord))
                    {
                        keyWord = keyWord.Trim().ToUpper();
                        objs = objs
                            .Where(x => (x.FirstName.Trim().ToUpper().Contains(keyWord) ||
                        x.LastName.Trim().ToUpper().Contains(keyWord) ||
                        x.Code.Trim().ToUpper().Contains(keyWord) ||
                        x.Mobile.Trim().ToUpper().Contains(keyWord) ||
                        x.Email.Trim().ToUpper().Contains(keyWord)));
                    }
                    return objs
                    .OrderBy(sorting)
                    .Select(x => new EmployeeModel()
                    {
                        Gender = x.Gender,
                        Birthday = x.Birthday,
                        Id = x.Id,
                        Code = x.Code,
                        Email = x.Email,
                        FullName = x.LastName.Trim() + " " + x.FirstName.Trim(),
                        Mobile = x.Mobile,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        Image = x.Image,
                        UserId = x.UserId,
                        UserName = x.UserId.HasValue ? x.SUser.UserName : ""
                    }).ToList();
                }
            }
            catch (Exception)
            {
            }
            return new List<EmployeeModel>();
        }

        public List<ModelSelectItem> GetSelectItem()
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var objs = new List<ModelSelectItem>();
                    var _founds = db.Employee.Where(x => !x.IsDeleted).Select(
                        x => new ModelSelectItem()
                        {
                            Value = x.Id,
                            Name = x.FirstName + " " + x.LastName,
                            Data = x.UserId??0,
                            Code = x.UserId.HasValue ? x.SUser.UserName : ""
                        }).ToList();

                    if (_founds != null && _founds.Count() > 0)
                    {
                        //objs.Add(new ModelSelectItem() { Value = 0, Name = " - -  Chọn khách hàng  - - " });
                        objs.AddRange(_founds);
                    }
                    else
                        objs.Add(new ModelSelectItem() { Value = 0, Name = "  Không có nhân viên  " });
                    return objs;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EmployeeWithSkillModel> GetEmployeeWithSkills(int companyId)
        {
            using (db = new SanXuatCheckListEntities())
            {
                return db.Employee.Where(x => !x.IsDeleted && x.CompanyId == companyId).Select(x => new EmployeeWithSkillModel()
                {
                    EmployeeId = x.Id,
                    EmployeeCode = x.Code,
                    EmployeeName = (x.FirstName + " " + x.LastName),
                    LastName = x.LastName
                }).ToList();
            };
        }

    }
}
