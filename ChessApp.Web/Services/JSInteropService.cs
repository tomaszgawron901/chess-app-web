using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace ChessApp.Web.Services
{
    public class JSInteropService : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> LazyModule;

        public JSInteropService(IJSRuntime jsRuntime)
        {
            LazyModule = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
               "import", "./JsInterop.js").AsTask());
        }

        private async Task InvokeVoidAsync(string functionName, params object[] parameters)
        {
            var module = await LazyModule.Value;
            await module.InvokeVoidAsync(functionName, parameters);
        }

        public Task SetSelectAndCopy(ElementReference element) => InvokeVoidAsync("SetSelectAndCopy", element);
        public Task SetSelect(ElementReference element) => InvokeVoidAsync("SetSelect", element);

        public async ValueTask DisposeAsync()
        {
            if (LazyModule.IsValueCreated)
            {
                var module = await LazyModule.Value;
                await module.DisposeAsync();
            }
        }
    }
}
