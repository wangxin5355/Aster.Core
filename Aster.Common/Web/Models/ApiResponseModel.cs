using Aster.Common.Utils;

namespace Aster.Common.Web.Models
{
    public class ApiResponseModel
    {
        public ApiResponseModel()
        {
            Ret = 0;
            Timestamp = DateTimeUtil.GetTimestamps();
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

        /// <summary>
        /// 环境，0正式，1测试
        /// </summary>
        public int Env { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public long Timestamp { get; set; }
    }

    public class ApiResponseModel<T> : ApiResponseModel
    {
        public ApiResponseModel(T data)
        {
            Data = data;
        }

        public T Data { get; set; }
    }

}
