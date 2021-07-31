using GPRO.Core.Mvc;
using SanXuatCheckList.Business.Model;
using SanXuatCheckList.Data;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanXuatCheckList.Business
{
    public class BLLIEDConfig
    {
        #region constructor
        SanXuatCheckListEntities db;
        static object key = new object();
        private static volatile BLLIEDConfig _Instance;
        public static BLLIEDConfig Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLIEDConfig();

                return _Instance;
            }
        }
        private BLLIEDConfig() { }
        #endregion

        public PagedList<T_IEDConfig> GetIEDConfigs(int startIndexRecord, int pageSize, string sorting)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    if (string.IsNullOrEmpty(sorting))
                    {
                        sorting = "Id DESC";
                    }
                    var iedConfig = db.T_IEDConfig.Where(x => !x.IsDeleted).OrderByDescending(x => x.Id);
                    var pageNumber = (startIndexRecord / pageSize) + 1;
                    return new PagedList<T_IEDConfig>(iedConfig.ToList(), pageNumber, pageSize);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResponseBase Update(T_IEDConfig model)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var result = new ResponseBase();
                    T_IEDConfig obj = db.T_IEDConfig.FirstOrDefault(x => x.Id == model.Id);
                    if (obj != null)
                    {
                        obj.IsDeleted = true;
                        obj.DeletedUser = model.DeletedUser;
                        obj.DeletedDate = model.DeletedDate;

                        obj = new T_IEDConfig();
                        obj.Name = model.Name;
                        obj.Value = model.Value;
                        obj.CreatedUser = model.DeletedUser ?? 0;
                        obj.CreatedDate = model.DeletedDate ?? DateTime.Now;
                        db.T_IEDConfig.Add(obj);

                        db.SaveChanges();
                        result.IsSuccess = true;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { MemberName = "", Message = "Lỗi Không tìm thấy dữ liệu." });
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public String GetValueByCode(string code)
        {
            try
            {
                using (db = new SanXuatCheckListEntities())
                {
                    var config = db.T_IEDConfig.FirstOrDefault(x => !x.IsDeleted && x.Name.Trim().ToUpper().Equals(code.Trim().ToUpper()));
                    if (config != null)
                        return config.Value;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return string.Empty;
        }

    }
}
