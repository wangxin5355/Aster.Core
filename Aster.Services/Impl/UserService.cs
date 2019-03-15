using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Aster.Common.Data.Core.Repositories;
using Aster.Common.Exceptions;
using Aster.Entities;
using Aster.Services.Models;
using Microsoft.Extensions.Localization;

namespace Aster.Services.Impl
{
    public class UserService : IUserService
    {

        private readonly IStringLocalizer _localizer;
        private readonly IRepository<User> _userRepository;
        private readonly string _cahce_user_info = "user_info1_{0}";
        private readonly string _cache_user_last10_login = "user_last_login_{0}";
        private readonly string _cache_user_setting = "user_setting_{0}";
        private readonly ITokenService _tokenService;

        public UserService(IStringLocalizer<UserService> localizer, IRepository<User> userRepository, ITokenService tokenService)
        {
            _localizer = localizer;
            _userRepository = userRepository;
            _tokenService = tokenService;

        }

        public async Task<LoginRpsModel> Login(string userName, string password, string packType)
        {
            if (string.IsNullOrWhiteSpace(userName)) throw new MyException(_localizer["用户名不能为空"]);
            if (string.IsNullOrWhiteSpace(password)) throw new MyException(_localizer["密码不能为空"]);
            var user = await _userRepository.Query(x=>x.UserName== userName).FirstOrDefaultAsync();
            if (user == null) throw new MyException(_localizer["用户名未注册"]);
            //检查登录次数

            //检查密码

            //创建token
            //先删除以前的
            //删除已有的token
            var rm = new LoginRpsModel()
            {
                UserId = user.Id,
                UserName = user.UserName,
                Token = null
            };
            
            await _tokenService.DelToken(user.Id, packType);
            rm.Token = await _tokenService.BuildToken(new TokenInfo(user) { PackType= packType });
            return new LoginRpsModel()
            { };

        }
    }
}
