namespace Aster.Common.Mvc.Models
{
    public class ApiResponseModel
    {
        public ApiResponseModel()
        {
            Ret = 0;
        }

        /// <summary>
        /// 正常:0,不正常:非0
        /// </summary>
        public int Ret { get; set; }

        /// <summary>
        /// 错误编码
        /// </summary>
        public string ErrCode { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrStr { get; set; }
    }

    public class ApiResponseModel<T>: ApiResponseModel
    {
        public ApiResponseModel(T data)
        {
            Data = data;
        }

        public T Data { get; set; }
    }
    
}
