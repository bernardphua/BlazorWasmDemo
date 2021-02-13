﻿using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WebCamComponent
{
    public partial class WebCam
    {
        const string JSModulePath = "./_content/WebCamComponent/webcam.js";

        [Inject] IJSRuntime JSRuntime{ get; set; }

        Lazy<Task<IJSObjectReference>> moduleTask;
        //IJSObjectReference moduleTask;

        public ElementReference VideoElement { get; set; }
        string errorMessage = null;
        bool isCameraStreaming = false;
        string CssCameraWrapper => isCameraStreaming ? "camera streaming" : "camera unavailable";

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                //moduleTask = JSRuntime.InvokeAsync<IJSObjectReference>("import", JSModulePath).Result;
                moduleTask = new(() => JSRuntime.InvokeAsync<IJSObjectReference>("import", JSModulePath).AsTask());
                var module = await moduleTask.Value;
                await module.InvokeVoidAsync("initialize", VideoElement, DotNetObjectReference.Create(this));
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

        public async Task<string> GetSnapShot()
        {
            var module = await moduleTask.Value;
            return await module.InvokeAsync<string>("getSnapshot", VideoElement);
        }

        public async ValueTask DisposeAsync()
        {
            if (moduleTask.IsValueCreated)
            {
                var module = await moduleTask.Value;
                await module.DisposeAsync();
            }
        }
    }
}