$(document).ready(async function () {
    await loadFaceApi();
})

var stream;
let currentFacing = 'environment';
let hiddenBase64;
const video = document.getElementById('cameraVideo');
const canvas = document.getElementById('cameraCanvas');
const previewImg = document.getElementById('photoPreview');
const btnUsePhoto = document.getElementById('btnUsePhoto');
const btnRetake = document.getElementById('btnRetake');
const btnTakePhoto = document.getElementById('btnTakePhoto');
const btnSwitchCamera = document.getElementById('btnSwitchCamera');

$(document).on('click', '.btn-open-modal', function (e) {
    e.preventDefault();
    var url = this.href; // reliable
    var $modal = $('#customerModal');
    var $body = $modal.find('.modal-body');
    var $title = $modal.find('#modalTitle');
    var $save = $('#btnSave');

    $modal.find('.modal-body').html('<div class="spinner-border" style="width: 3rem; height: 3rem;" role="status"><span class="sr-only">Loading...</span></div>');
    $modal.modal('show');

    $modal.find('.modal-body').load(url, function (response, status) {
        if (status === 'error') {
            $(this).html('<div class="text-danger p-3">Không tải được thông tin.</div>');
        }

        // Read meta flags from loaded fragment
        var meta = $body.find('[data-modal-title],[data-can-save]');
        var title = meta.data('modal-title') || 'Thông tin';
        var canSave = String(meta.data('can-save')).toUpperCase() === 'TRUE';

        $title.text(title);
        $save.toggleClass('d-none', !canSave);

        //Gắn sự kiện chọn ngày cho modal
        // single datepicker
        $('.input-datepicker').daterangepicker({
            singleDatePicker: true,
            showDropdowns: true,
            autoApply: true,
            autoUpdateInput: true,
            minYear: 1901,
            maxYear: 2099,
            locale: {
                format: 'DD/MM/YYYY',
                daysOfWeek: ["CN", "T2", "T3", "T4", "T5", "T6", "T7"],
                "monthNames": ["Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6",
                    "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"],
            },

        }, function (start, end) {
            if ($('.input-datepicker > input').length) {
                $('.input-datepicker > input').val(start.format('DD/MM/YYYY'));
            }
            // Tự động gửi biểu mẫu
            //$('.input-datepicker').closest('form').submit();
        });
    });
});

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
    console.log(`compress image percent: ${quality}, output size: ${imageSize}KB`);
    return base64String;
}

async function dectionFace(img) {
    console.log(img)
    var $photoReview = $('#photoReview');
    const detections = await faceapi.detectAllFaces(img, new faceapi.TinyFaceDetectorOptions());
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
    newPhoto.onload = () => dectionFace(newPhoto);

    var $photoReview = $('#photoReview');
    $photoReview.html('');
    $photoReview.append(newPhoto);
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
    try {
        await startCamera();
        btnTakePhoto.classList.remove('d-none');
        btnSwitchCamera.classList.remove('d-none');
        btnUsePhoto.classList.add('d-none');
        btnRetake.classList.add('d-none');
        canvas.style.display = 'none';
        video.style.display = 'block';
        $('#customerModal').modal('hide');
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
    $('#customerModal').modal('show');
    createImageReview(dataUrl);
    stopStream();
}

async function switchCameraClick() {
    currentFacing = currentFacing === 'user' ? 'environment' : 'user';
    await startCamera();
}


function closeCameraModalClick() {
    $('#cameraModal').modal('hide');
    $('#customerModal').modal('show');
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
