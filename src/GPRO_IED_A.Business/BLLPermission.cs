using System;
using System.Collections.Generic;
using System.Linq; 
using Hugate.Framework.Infrastructure.Data;
using PagedList;
using IED_Alone.Data;
using IED_Alone.Data.Repositories;
using IED_Alone.Business.Interface;
using IED_Alone.Business.Interface.Model;
using IED_Alone.Object;

namespace IED_Alone.Business
{
    public class BLLPermission : IBLLPermission
    {
        private readonly ISPermissionRepository repPermission;
        private readonly IUnitOfWork<IED_AloneEntities> unitOfWork;
        private readonly IBLLUserRole bllUserRole;
        private readonly IBLLRolePermission bllRolePermission;
        public BLLPermission(IUnitOfWork<IED_AloneEntities> _unitOfWork, ISPermissionRepository _repPermission, IBLLUserRole _bllUserRole, IBLLRolePermission _bllRolePermission)
        {
            this.unitOfWork = _unitOfWork;
            this.repPermission = _repPermission;
            this.bllUserRole = _bllUserRole;
            this.bllRolePermission = _bllRolePermission;
        }

        public ResponseBase Create(SPermission asset)
        {
            throw new NotImplementedException();
        }

        public ResponseBase Update(SPermission asset)
        {
            throw new NotImplementedException();
        }

        public ResponseBase DeleteById(int id, int userId)
        {
            throw new NotImplementedException();
        }

        public ResponseBase DeleteByListId(List<int> listId, int userId)
        {
            throw new NotImplementedException();
        }

        public PagedList<SPermission> GetList(int counTryId, int startIndexRecord, int pageSize, string sorting)
        {
            throw new NotImplementedException();
        }

        public ResponseBase GetListPermissionByListRoleId(List<int> listRoleId)
        {
            ResponseBase responResult = null;
            try
            {
                //var listPermission = repPermission.GetMany(x=>x.)
            }
            catch (Exception ex)
            {                
                throw ex;
            }
            return responResult;
        }
        public List<SPermission> GetListPermissionByFeatureID(int FeatureId)
        {
            List<SPermission> listPermisson = null;
            try
            {
                 listPermisson = repPermission.GetMany(x => !x.IsDeleted && x.FeatureId == FeatureId).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return listPermisson;
        }
    }
}
