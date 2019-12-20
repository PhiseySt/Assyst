using System;
using Assyst.Models;
using Microsoft.AspNetCore.Mvc;

namespace Assyst.Controllers
{
    public class HtmlHelperController : Controller
    {
        #region ActionResults

        public IActionResult PopupPartialView(PopupSettings item)
        {
            return PartialView(item);
        }

        public IActionResult ViewMessagePartialView(ViewMessageSettings settings)
        {
            return PartialView(settings);
        }

        #endregion

        #region Properties & Methods

        #endregion
    }
}
