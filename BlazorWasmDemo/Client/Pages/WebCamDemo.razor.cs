using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using WebCamComponent;

namespace BlazorWasmDemo.Client.Pages
{
    public partial class WebCamDemo
    {
        WebCam Camera { get; set; }
        string imageData;


        protected void OnCamera()
        {
            Camera.StartCamera();
        }

        protected void GetOffCamera() 
        {
            Camera.StopCamera();
        }

        protected async void Snapshot()
        {
            imageData = await Camera.GetSnapShot();
            if (imageData.Replace("data:,", "").Length == 0) imageData = null;
            StateHasChanged();
        }
    }
}
