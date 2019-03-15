using Aster.Common.Data.Core;
using Aster.Common.Data.Core.Attributes;
using System;

namespace Aster.Entities
{
    //[DataContext("DefaultConnectionString")]默认的就是这个，如果是其他的库。就需要声明
    [TableName("t_user")]
    public class User: IEntity
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///密码
        /// </summary>
        public string PassWord { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 电子邮件
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 电子邮件
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime LastUpdateTime { get; set; }
    }
}
