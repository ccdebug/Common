using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Abp.UI;
using Abp.Web.Mvc.Controllers.Results;
using Backend.Application.Authorization.Users;
using Backend.Application.Authorization.Users.Dto;
using Backend.Web.Models.Common;
using Backend.Web.Models.Users;
using Newtonsoft.Json;

namespace Backend.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserAppService _userAppService;

        public UsersController(IUserAppService userAppService)
        {
            _userAppService = userAppService;
        }


        // GET: Users
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> List(GetUserListRequest request)
        {
            var input = new GetUsersInput
            {
                SkipCount = request.Start,
                MaxResultCount = request.Length,
                Filter = request.Filter
            };

            var result = await _userAppService.GetUsers(input);

            var jqResult = new JqDatatableResult<UserListDto>
            {
                Draw = request.Draw,
                RecordsTotal = result.TotalCount,
                RecordsFiltered = result.TotalCount,
                Data = result.Items.ToList()
            };

            return new AbpJsonResult(jqResult);
        }
    }
}