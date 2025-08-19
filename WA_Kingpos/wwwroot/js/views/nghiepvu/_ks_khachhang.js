

$(document).ready(async function () {
    await loadFaceApi();
})
var stream;
let currentFacing = 'environment';
let hiddenBase64;
let video;
let canvas;
let previewImg;
let btnUsePhoto;
let btnRetake;
let btnTakePhoto;
let btnSwitchCamera;

$('#btnSave').on('click', function () {
    var $modal = $('#customerModal');
    var $form = $modal.find('#customerForm');
    if ($form.length === 0) return;

    var form = document.getElementById('customerForm')
    if (form.checkValidity() === false) {
        form.classList.add('was-validated');
        return;
    }
    

    $.ajax({
        url: $form.attr('action') || window.location.href + '?handler=Save',
        method: 'POST',
        data: $form.serialize(),
        success: function (json) {
            if (json && json.ok) {
                $modal.modal('hide');
                // Optional: reload the list or update row inline
                location.reload();
            } else {
                // If server returned HTML (validation errors), replace body
                $modal.find('.modal-body').html(json);
            }
        },
        error: function (xhr) {
            // If validation failed and Page() returned HTML, xhr.responseText is HTML
            var ct = xhr.getResponseHeader('Content-Type') || '';
            //if (ct.indexOf('text/html') >= 0) {
            //    $modal.find('.modal-body').html(xhr.responseText);
            //} else {
            //    alert('Lỗi khi lưu.');
            //}
            alert('Lỗi khi lưu.');
        }
    });

});

//-------------------- Xử lý ảnh
function resizeImageTo300x300(file, callback) {
    const reader = new FileReader();
    reader.onload = function (e) {
        const img = new Image();
        img.onload = function () {
            const canvas = document.createElement('canvas');
            const ctx = canvas.getContext('2d');
            const size = 300;

            canvas.width = size;
            canvas.height = size;

            // Fill with white background (optional, for JPG)
            ctx.fillStyle = "#ffffff";
            ctx.fillRect(0, 0, size, size);

            // Draw image scaled to fill 300x300
            ctx.drawImage(img, 0, 0, size, size);

            const resizedDataUrl = canvas.toDataURL('image/jpeg', 0.9);
            callback(resizedDataUrl);
        };
        img.src = e.target.result;
    };
    reader.readAsDataURL(file);
}

// Nén hình ảnh cho đến khi kích thước nhỏ hơn 30 kilobytes
function compressCanvasImage(canvas) {
    let quality = 1.0;
    let dataURL;
    let imageSize;
    const maxSizeKB = 30;
    let base64String;

    do {
        dataURL = canvas.toDataURL('image/jpeg', quality);
        // Extract the base64 part of the data URL
        base64String = dataURL.split(',')[1];
        // Calculate the image size in bytes
        const stringLength = base64String.length;
        const sizeInBytes = 4 * Math.ceil(stringLength / 3) * 0.5624896334383812; // Rough approximation
        imageSize = sizeInBytes / 1024; // Convert bytes to kilobytes

        if (imageSize > maxSizeKB) {
            quality -= 0.05;
        }
    } while (imageSize > maxSizeKB && quality > 0);
    return base64String;
}

async function dectionFace(img) {
    
    var $photoReview = $('#photoReview');
    const detections = await faceapi.detectAllFaces(img, new faceapi.TinyFaceDetectorOptions());
    console.log("a")
    if (detections.length == 1) {
        document.getElementById('photoBase64').value = img.src.split(',')[1];
    }
    else if (detections.length <= 0) {
        $photoReview.html('<div class="text-danger">KHÔNG PHÁT HIỆN KHUÔN MẶT TRONG ẢNH</div>');
    }
    else {
        $photoReview.html('<div class="text-danger">PHÁT HIỆN NHIỀU HƠN 1 KHUÔN MẶT TRONG ẢNH</div>');
    }
}

function createImageReview(src) {
    var newPhoto = document.createElement('img');
    newPhoto.alt = "Customer photo";
    newPhoto.className = "img-thumbnail";
    newPhoto.style = "max-width:250px; max-height:250px; min-width:150px; min-height:150px";
    newPhoto.src = src;
    newPhoto.onload = async () => await dectionFace(newPhoto);
    console.log("a1")

    var $photoReview = $('#photoReview');
    $photoReview.html('');
    $photoReview.append(newPhoto);
    console.log("a2")

}

function handleImageInput(input) {

    if (input.files && input.files[0]) {
        resizeImageTo300x300(input.files[0], (base64) => createImageReview(base64));
    }
}

function fileInputChange(element) {
    handleImageInput(element);
}

function stopStream() {
    if (stream) { stream.getTracks().forEach(t => t.stop()); stream = null; }
}

async function startCamera() {
    // Stop any existing stream before starting a new one
    stopStream();

    try {
        stream = await navigator.mediaDevices.getUserMedia({
            video: { facingMode: { exact: currentFacing } }
        });
    } catch {
        // Fallback: try without exact constraint if device doesn't support it
        stream = await navigator.mediaDevices.getUserMedia({
            video: { facingMode: currentFacing }
        });
    }
    video.srcObject = stream;
}


async function openCameraClick() {
    hiddenBase64 = document.getElementById('photoBase64');
    video = document.getElementById('cameraVideo');
    canvas = document.getElementById('cameraCanvas');
    previewImg = document.getElementById('photoPreview');
    btnUsePhoto = document.getElementById('btnUsePhoto');
    btnRetake = document.getElementById('btnRetake');
    btnTakePhoto = document.getElementById('btnTakePhoto');
    btnSwitchCamera = document.getElementById('btnSwitchCamera');

    try {
        await startCamera();
        btnTakePhoto.classList.remove('d-none');
        btnSwitchCamera.classList.remove('d-none');
        btnUsePhoto.classList.add('d-none');
        btnRetake.classList.add('d-none');
        canvas.style.display = 'none';
        video.style.display = 'block';
        //$('#customerModal').modal('hide');
        $('#cameraModal').modal('show');
    } catch (err) {
        alert('Không thể truy cập camera: ' + err.message);
    }
}

function takePhotoClick() {

    const ctx = canvas.getContext('2d');
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    // Draw video frame into canvas, resize to 300x300
    ctx.drawImage(video, 0, 0, canvas.width, canvas.height);

    // Hide video, show canvas
    video.style.display = 'none';
    canvas.style.display = 'block';

    // Show "Use photo" button
    btnTakePhoto.classList.add('d-none');
    btnSwitchCamera.classList.add('d-none');
    btnRetake.classList.remove('d-none');
    btnUsePhoto.classList.remove('d-none');
}

function reTakePhotoClick() {
    video.style.display = 'block';
    canvas.style.display = 'none';

    btnTakePhoto.classList.remove('d-none');
    btnRetake.classList.add('d-none');
    btnUsePhoto.classList.add('d-none');
    btnSwitchCamera.classList.remove('d-none');
}

function usePhotoClick() {
    const dataUrl = canvas.toDataURL('image/jpeg', 0.9);
    $('#cameraModal').modal('hide');
    //$('#customerModal').modal('show');
    createImageReview(dataUrl);
    stopStream();
}

async function switchCameraClick() {
    currentFacing = currentFacing === 'user' ? 'environment' : 'user';
    await startCamera();
}


function closeCameraModalClick() {
    $('#cameraModal').modal('hide');
    //$('#customerModal').modal('show');
}

async function loadFaceApi() {
    let modelDir = "/lib/face-api/models/";
    await Promise.all([
        faceapi.nets.tinyFaceDetector.loadFromUri(modelDir + 'tiny_face_detector_model-weights_manifest.json'),
    ]);
}

$('#cameraModal').on('hidden.bs.modal', function () {
    if (stream) { stream.getTracks().forEach(t => t.stop()); stream = null; }
});
