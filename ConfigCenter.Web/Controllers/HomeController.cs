using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ConfigCenter.Core;

namespace ConfigCenter.Web.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Add(string group, string key, string value)
        {
            var result = await new ConfZkClient(group).AddAsync(key, value);

            return Content(result ? "success" : "failed");
        }

        public ActionResult Get()
        {
            var value = ConfClient.Get("APPBalance");
            return Content(value);
        }

        //public async Task<ActionResult> Get()
        //{
        //    var value = await ConfClient.GetAsync("APPBalance");
        //    return Content(value);
        //}
    }
}