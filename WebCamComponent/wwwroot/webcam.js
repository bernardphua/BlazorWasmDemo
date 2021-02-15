const constraints = {
    audio: false,
    video: true
};

// public
export async function startCamera(videoElement, dotnet) {
    if (videoElement.srcObject === null || typeof videoElement.srcObject === 'undefined') {
        try {
            let stream = await navigator.mediaDevices.getUserMedia(constraints);
            handleSuccess(stream, videoElement, dotnet);
        } catch (e) {
            handleError(e, dotnet);
        }
    }
}

export async function stopCamera(videoElement) {
    if (videoElement.srcObject !== null && typeof videoElement.srcObject !== 'undefined') {
        let stream = videoElement.srcObject;
        stream.getTracks().forEach(track => track.stop());
        videoElement.srcObject = null;
    }
}

export function getSnapshot(videoElement) {
    let canvas = document.createElement("canvas");
    let ctx = canvas.getContext("2d");
    canvas.setAttribute('width', videoElement.videoWidth);
    canvas.setAttribute('height', videoElement.videoHeight);
    ctx.drawImage(videoElement, 0, 0, videoElement.videoWidth, videoElement.videoHeight);
    let data = canvas.toDataURL('image/png');
    canvas.remove();
    return data;
}

// private
function handleSuccess(stream, videoElement, dotnet) {
    videoElement.srcObject = stream;
    videoElement.play();
    dotnet.invokeMethodAsync("OnCameraStreaming");
}

function handleError(error, dotnet) {
    if (error.name === 'ConstraintNotSatisfiedError') {
        const v = constraints.video;
        errorMsg(`The resolution ${v.width.exact} x ${v.height.exact} px is not supported by your device.`, error, dotnet);
    } else if (error.name === 'NotAllowedError') {
        errorMsg('Permissions have not been granted to use your camera and ' +
            'microphone, you need to allow the page access to your devices in ' +
            'order for the demo to work.', error, dotnet);
    } else {
        errorMsg(`getUserMedia error: ${error.name}`, error, dotnet);
    }
}

function errorMsg(msg, error, dotnet) {
    if (typeof error !== 'undefined') {
        console.error(error);
    }
    dotnet.invokeMethodAsync("OnCameraStreamingError", msg);
}