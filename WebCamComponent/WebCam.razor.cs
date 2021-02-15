using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WebCamComponent
{
    public partial class WebCam : IDisposable
    {
        const string JSModulePath = "./_content/WebCamComponent/webcam.js";

        [Inject] IJSRuntime JSRuntime{ get; set; }

        Lazy<Task<IJSObjectReference>> jsModuleTask;
        private IJSObjectReference jsModule;

        [Parameter]
        public string Width { get; set; } = "100%";
        [Parameter]
        public string Height { get; set; } = "100%";

        public ElementReference VideoElement { get; set; }
        string errorMessage = null;
        bool isCameraStreaming = false;
        string CssCameraWrapper => isCameraStreaming ? "camera streaming" : "camera unavailable";

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                jsModuleTask = new(() => JSRuntime.InvokeAsync<IJSObjectReference>("import", JSModulePath).AsTask());
                jsModule = await jsModuleTask.Value;
            }
        }

        [JSInvokable]
        public void OnCameraStreaming()
        {
            isCameraStreaming = true;
            errorMessage = null;
            StateHasChanged();
        }

        [JSInvokable]
        public void OnCameraStreamingError(string error)
        {
            isCameraStreaming = false;
            errorMessage = error;
            StateHasChanged();
        }

        public async void StartCamera()
        {
            await jsModule.InvokeVoidAsync("startCamera", VideoElement, DotNetObjectReference.Create(this));
        }

        public async void StopCamera()
        {
            await jsModule.InvokeVoidAsync("stopCamera", VideoElement);
        }

        public async Task<string> GetSnapShot()
        {
            return await jsModule.InvokeAsync<string>("getSnapshot", VideoElement);
        }

        public async void Dispose()
        {
            if (jsModuleTask.IsValueCreated)
            {
                await jsModule.InvokeVoidAsync("stopCamera", VideoElement);
                await jsModule.DisposeAsync();
            }
            GC.SuppressFinalize(this);
        }
    }
}
