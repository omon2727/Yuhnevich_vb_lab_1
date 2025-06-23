using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Yuhnevich_vb_lab.TagHelpers
{
    [HtmlTargetElement("img", Attributes = "img-action,img-controller")]
    public class ImageTagHelper : TagHelper
    {
        [HtmlAttributeName("img-action")]
        public string ImgAction { get; set; } = string.Empty;

        [HtmlAttributeName("img-controller")]
        public string ImgController { get; set; } = string.Empty;

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } = null!;

        private readonly IUrlHelperFactory _urlHelperFactory;

        public ImageTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            _urlHelperFactory = urlHelperFactory;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (string.IsNullOrEmpty(ImgAction) || string.IsNullOrEmpty(ImgController))
            {
                return; // Если атрибуты не указаны, ничего не делаем
            }

            // Получаем IUrlHelper из контекста
            var urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);

            // Генерируем URL на основе action и controller
            var src = urlHelper.Action(new UrlActionContext
            {
                Action = ImgAction,
                Controller = ImgController
            });

            // Устанавливаем атрибут src
            output.Attributes.SetAttribute("src", src);

            // Удаляем кастомные атрибуты img-action и img-controller из итогового HTML
            output.Attributes.RemoveAll("img-action");
            output.Attributes.RemoveAll("img-controller");
        }
    }
}