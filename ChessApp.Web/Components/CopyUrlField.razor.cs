using ChessApp.Web.Services;
using Microsoft.AspNetCore.Components;

namespace ChessApp.Web.Components
{
    public partial class CopyUrlField: ComponentBase
    {
        [Inject] protected JSInteropService jSInteropService { get; set; }
        [Parameter] public string Url { get; set; }

        protected ElementReference UrlInput;

        protected void OnInputClick() => jSInteropService.SetSelect(UrlInput);

        protected void OnCopyUrlClicked() => jSInteropService.SetSelectAndCopy(UrlInput);
    }
}
