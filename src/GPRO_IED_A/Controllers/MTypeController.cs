using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SanXuatCheckList.Business;
using SanXuatCheckList.Business.Model;
using SanXuatCheckList.Data;

namespace SanXuatCheckList.Controllers
{
    public class MTypeController : BaseController
    {
        // GET: MType
        public ActionResult Index()
        {
            return View();
        }

        /********************************************************************************************************************        
                                                           MANIPULATION  
       ********************************************************************************************************************/
        [HttpPost]
        public JsonResult Gets_(string keyword, int searchBy, int maniType, int jtStartIndex = 0, int jtPageSize = 1000, string jtSorting = "")
        {
            try
            {
                var maniTypes = BLLManipulationLibrary.Instance.GetList(keyword, searchBy, maniType, jtStartIndex, jtPageSize, jtSorting);
                JsonDataResult.Records = maniTypes;
                JsonDataResult.Result = "OK";
                JsonDataResult.TotalRecordCount = maniTypes.TotalItemCount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult GetListSmall(string keyword, int searchBy, int jtStartIndex = 0, int jtPageSize = 1000, string jtSorting = "")
        {
            try
            {
                var maniTypes = BLLManipulationLibrary.Instance.GetList(keyword, searchBy, jtStartIndex, jtPageSize, jtSorting);
                JsonDataResult.Records = maniTypes;
                JsonDataResult.Result = "OK";
                JsonDataResult.TotalRecordCount = maniTypes.TotalItemCount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult GetManipulationEquipment(int Id, int jtStartIndex = 0, int jtPageSize = 1000, string jtSorting = "")
        {
            try
            {
                var maniTypes = BLLManipulationLibrary.Instance.GetManipulationEquipment(Id, jtStartIndex, jtPageSize, jtSorting);
                JsonDataResult.Records = maniTypes;
                JsonDataResult.Result = "OK";
                JsonDataResult.TotalRecordCount = maniTypes.TotalItemCount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult GetManipulationEquipmentByManipulationId(int Id)
        {
            try
            {
                var maniTypes = BLLManipulationLibrary.Instance.GetManipulationEquipment(Id);
                JsonDataResult.Data = maniTypes;
                JsonDataResult.Result = "OK";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult GetManipulationFileById(int Id)
        {
            try
            {
                var maniFiles = BLLManipulationLibrary.Instance.GetManipulationFileById(Id);
                JsonDataResult.Data = maniFiles;
                JsonDataResult.Result = "OK";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(JsonDataResult);
        }


        [HttpPost]
        public JsonResult Save_(ManipulationLibraryModel ManiModel, List<ManipulationEquipmentModel> TMUModel, bool isUseMachine, List<FileUploadModel> FileModel)
        {
            ResponseBase responseResult;
            try
            {
                ManiModel.ActionUser = UserContext.UserID;
                responseResult = BLLManipulationLibrary.Instance.InsertOrUpdate(ManiModel, TMUModel, FileModel);

                if (!responseResult.IsSuccess)
                {
                    JsonDataResult.Result = "ERROR";
                    JsonDataResult.ErrorMessages.AddRange(responseResult.Errors);
                }
                else
                    JsonDataResult.Result = "OK";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult Delete_(int Id)
        {
            ResponseBase result;
            try
            {
                result = BLLManipulationLibrary.Instance.Delete(Id, UserContext.UserID);
                if (!result.IsSuccess)
                {
                    JsonDataResult.Result = "ERROR";
                    JsonDataResult.ErrorMessages.AddRange(result.Errors);
                }
                else
                    JsonDataResult.Result = "OK";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult GetNatureCuts()
        {
            try
            {
                var natureCuts = BLLManipulationTypeLibrary.Instance.GetNatureCuts();
                JsonDataResult.Result = "OK";
                if (natureCuts.Count > 0)
                    JsonDataResult.Data = natureCuts;
                else
                    JsonDataResult.Data = new List<T_NatureCutsLibrary>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult GetApplyPressures()
        {
            try
            {
                var applyPressure = BLLManipulationTypeLibrary.Instance.GetApplyPressures();
                JsonDataResult.Result = "OK";
                if (applyPressure.Count > 0)
                    JsonDataResult.Data = applyPressure;
                else
                    JsonDataResult.Data = new List<T_ApplyPressureLibrary>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult GetStopPrecisions()
        {
            try
            {
                var stopPrecision = BLLManipulationTypeLibrary.Instance.GetStopPrecisions();
                JsonDataResult.Result = "OK";
                if (stopPrecision.Count > 0)
                    JsonDataResult.Data = stopPrecision;
                else
                    JsonDataResult.Data = new List<T_StopPrecisionLibrary>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(JsonDataResult);
        }


        [HttpPost]
        public JsonResult GetAllManipulation()
        {
            ResponseBase result;
            try
            {
                result = BLLManipulationLibrary.Instance.GetAllManipulation();
                JsonDataResult.Result = "OK";
                JsonDataResult.Data = result.Data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(JsonDataResult);
        }

        /********************************************************************************************************************        
                                                        MANIPULATION TYPE
        ********************************************************************************************************************/
        [HttpPost]
        public JsonResult GetMTypes()
        {
            try
            {

                var maniTypes = BLLManipulationTypeLibrary.Instance.GetListManipulationType();
                JsonDataResult.Data = maniTypes;
                JsonDataResult.Result = "OK";

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult Save(T_ManipulationTypeLibrary maniType)
        {
            ResponseBase responseResult;
            try
            {
                if (maniType.Id == 0)
                {
                    maniType.CreatedUser = UserContext.UserID;
                    maniType.CreatedDate = DateTime.Now;
                    responseResult = BLLManipulationTypeLibrary.Instance.InsertOrUpdate(maniType);
                }
                else
                {
                    maniType.UpdatedUser = UserContext.UserID;
                    maniType.UpdatedDate = DateTime.Now;
                    responseResult = BLLManipulationTypeLibrary.Instance.InsertOrUpdate(maniType);
                }
                if (!responseResult.IsSuccess)
                {
                    JsonDataResult.Result = "ERROR";
                    JsonDataResult.ErrorMessages.AddRange(responseResult.Errors);
                }
                else
                    JsonDataResult.Result = "OK";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult Delete(int Id)
        {
            ResponseBase result;
            try
            {
                result = BLLManipulationTypeLibrary.Instance.Delete(Id, UserContext.UserID);
                if (!result.IsSuccess)
                {
                    JsonDataResult.Result = "ERROR";
                    JsonDataResult.ErrorMessages.AddRange(result.Errors);
                }
                else
                    result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(JsonDataResult);
        }


        #region ManipulationLibrary
        [HttpPost]
        public JsonResult GetManipulationEquipmentInfoByCode(int equipmentId, int equiptypedefaultId, int applyPressure, string code)
        {
            try
            {
                var result = BLLManipulationLibrary.Instance.GetManipulationEquipmentInfoByCode(equipmentId, equiptypedefaultId, applyPressure, code);
                if (result.IsSuccess)
                {
                    JsonDataResult.Data = result.Data;
                    JsonDataResult.Result = "OK";
                }
                else
                {
                    JsonDataResult.Result = "ERROR";
                    JsonDataResult.ErrorMessages.AddRange(result.Errors);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(JsonDataResult);
        }
        #endregion
    }
}