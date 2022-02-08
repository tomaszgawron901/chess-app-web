using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessApp.Web.Components
{
    public partial class CopyUrlField: ComponentBase
    {
        [Inject] protected IJSRuntime JSRuntime { get; set; }
        [Parameter] public string Url { get; set; }

        protected ElementReference UrlInput;

        protected void OnInputClick()
        {
            JSRuntime.InvokeVoidAsync("chessAppJsFunctions.setSelect", UrlInput);
        }

        protected void OnCopyUrlClicked()
        {
            JSRuntime.InvokeVoidAsync("chessAppJsFunctions.setSelectAndCopy", UrlInput);
        }
    }
}
