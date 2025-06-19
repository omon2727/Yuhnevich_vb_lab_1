using Microsoft.AspNetCore.Mvc;

namespace Yuhnevich_vb_lab.Components
{
    public class CartViewComponent:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}