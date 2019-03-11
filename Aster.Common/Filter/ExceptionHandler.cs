using Aster.Common.Exceptions;
using Aster.Common.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;

namespace Aster.Common.Filter
{
    public class ExceptionHandler
    {
        public static JsonResult ExceptionToJson(Exception ex, IStringLocalizer localizer)
        {
            if (ex is MyException)
            {
                var e = ex as MyException;
                return new JsonResult(new ApiResponseModel()
                {
                    Ret = -1,
                    ErrCode = e.ErrCode,
                    ErrStr = e.ErrMsg
                });
            }
            //else if (ex is InvokerFailException)
            //{
            //    var e = ex as InvokerFailException;
            //    return new JsonResult(new ApiResponseModel()
            //    {
            //        Ret = -1,
            //        ErrCode = e.ErrCode,
            //        ErrStr = e.Message
            //    });
            //}
            else
            {
                return new JsonResult(new ApiResponseModel()
                {
                    Ret = -1,
                    ErrCode = null,
                    ErrStr = ex.Message ?? localizer["内部错误，请联系Aster客服"]
                });
            }
        }
    }
}
