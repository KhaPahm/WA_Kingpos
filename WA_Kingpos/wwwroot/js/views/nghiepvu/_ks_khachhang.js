

$(document).ready(async function () {
    await loadFaceApi();
})

async function loadFaceApi() {
    let modelDir = "/lib/face-api/models/";
    await Promise.all([
        faceapi.nets.tinyFaceDetector.loadFromUri(modelDir + 'tiny_face_detector_model-weights_manifest.json'),
    ]);
}

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
let hiddenImage;

//Lưu thông tin
$('#btnSave').on('click', function () {
    var btn = $(this);
    btn.prop("disabled", true).text("Đang lưu...");
    var $modal = $('#customerModal');
    //var $form = $modal.find('#customerForm');
    //if ($form.length === 0) return;
    $form = $('#customerForm')
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
                if ($modal.length != 0) {
                    $modal.modal('hide');
                }
                alert("Lưu thành công!");
                // Optional: reload the list or update row inline
                location.reload();
            } else {
                if ($modal.length != 0) {
                    $modal.find('.modal-body').html(json);
                }
                // If server returned HTML (validation errors), replace body
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
        },
        complete: function () {
            // Re-enable button and restore text
            btn.prop("disabled", false).text("Lưu");
        }
    });

});


//------------Start: Xử lý chụp ảnh---------------------

//Dừng dùng camera camera
function stopStream() {
    if (stream) { stream.getTracks().forEach(t => t.stop()); stream = null; }
}

//Mở camera
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
    hiddenImage = document.getElementById('hiddenImage');

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

    canvas.width = 300 * video.videoWidth / video.videoHeight
    canvas.height = 300
    
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

async function switchCameraClick() {
    currentFacing = currentFacing === 'user' ? 'environment' : 'user';
    await startCamera();
}

function closeCameraModalClick() {
    stopStream();
    $('#cameraModal').modal('hide');
}

async function usePhotoClick() {

    const dataUrl = canvas.toDataURL('image/jpeg', 0.9);
    $('#cameraModal').modal('hide');
    await processImage(dataUrl);
    stopStream();
}
//------------End: Xử lý chụp ảnh---------------------

//------------Start: Xử lý upload ảnh---------------------
async function fileInputChange(element) {
    await handleImageInput(element);
}

async function handleImageInput(input) {
    //if (input.files && input.files[0]) {
    //    resizeImageTo300x300(input.files[0], (base64) => createImageReview(base64));
    //}
    const f = input.files && input.files[0];
    if (f) await processFile(f);
}

async function processFile(file) {
    if (!hiddenImage)
        hiddenImage = document.getElementById('hiddenImage');
    if (!hiddenBase64)
        hiddenBase64 = document.getElementById('photoBase64');
    const url = URL.createObjectURL(file);
    await processImage(url);
    URL.revokeObjectURL(url);
}
//------------End: Xử lý upload ảnh---------------------

//------------Start: Xử lý cắt và vẽ ảnh lên canvas---------------------
async function processImage(url) {
    var $photoReview = $('#photoReview');
    $photoReview.html('<button class="btn btn-outline-primary" type="button" disabled><span class="spinner-grow spinner-grow-sm text-primary" role="status" aria-hidden="true"></span> Đang kiểm tra ảnh...</button>');

    return new Promise((resolve) => {
        hiddenImage.onload = async () => {
            try {
                const detections = await faceapi.detectAllFaces(hiddenImage, new faceapi.TinyFaceDetectorOptions());
                if (detections.length <= 0) {
                    $photoReview.html('')
                    $photoReview.html('<div class="text-danger">KHÔNG PHÁT HIỆN KHUÔN MẶT TRONG ẢNH</div>');
                    resolve();
                    return;
                }
                else if (detections.length > 1) {
                    $photoReview.html('')
                    $photoReview.html('<div class="text-danger">PHÁT HIỆN NHIỀU HƠN 1 KHUÔN MẶT TRONG ẢNH</div>');
                    resolve();
                    return;
                }

                const box = detections[0].box; // {x, y, width, height}
                // Calculate a square crop centered on the face center
                const centerX = box.x + box.width / 2;
                const centerY = box.y + box.height / 2;
                console.log("box.width: ", box.width);
                console.log("box.height: ", box.height);
                const cropSize = Math.max(box.width, box.height) * 1.6; // add padding so face is nicely centered

                // Source rectangle
                const sx = Math.max(0, Math.round(centerX - cropSize / 2));
                const sy = Math.max(0, Math.round(centerY - cropSize / 2 - 20));
                const sw = Math.min(cropSize, hiddenImage.naturalWidth - sx);
                const sh = Math.min(cropSize, hiddenImage.naturalHeight - sy);

                const outCanvas = document.createElement('canvas');
                outCanvas.classList.add("img-thumbnail");
                const size = 300;

                outCanvas.width = size;
                outCanvas.height = size;
                $photoReview.html('')
                $photoReview.append(outCanvas);

                const outCtx = outCanvas.getContext('2d');

                outCtx.clearRect(0, 0, outCanvas.width, outCanvas.height);
                outCtx.drawImage(hiddenImage, sx, sy, sw, sh, 0, 0, 300, 300);

                const srcBase64 = outCanvas.toDataURL('image/jpeg', 0.9);
                hiddenBase64.value = srcBase64.split(',')[1];
            } catch (err) {
                console.error(err);
                $photoReview.html('')
                $photoReview.html('<div class="text-danger">Lỗi nhận diện khuôn mặt</div>');
            }
            resolve();
        };
        hiddenImage.onerror = () => { alert('Không thể load hình ảnh'); resolve(); };
        hiddenImage.src = url;
    });
}
//------------End: Xử lý cắt và vẽ ảnh lên canvas---------------------


