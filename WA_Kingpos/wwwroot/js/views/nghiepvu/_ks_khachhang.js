

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
        success: function (res) {
            
            if (res && res.ok && !res.views) {
                const updatedRowHtml = res.text();
                console.log("updatedRowHtml: ", updatedRowHtml);
                if ($modal.length != 0) {
                    $modal.modal('hide');
                }
                alert("Lưu thành công!");
                location.reload();
            } else {
                const id = $('#KS_KhachHang_MA_KH').val()
                const ten = $('#KS_KhachHang_TEN').val()

                if ($.fn.dataTable.isDataTable('#tablefull')) {
                    const dt = $('#tablefull').DataTable();
                    if (id.toString() !== '0') {
                        //Update thông tin trong bảng
                        const row = dt.row(`#ks-khachhang-row-${id}`)

                        if (!row.node()) return;

                        // Add the new row as a jQuery object (single <tr>)
                        const $newRow = $(res.views.row.trim());
                        const cells = $newRow.find('td').map(function () { return $(this).html(); }).get();

                        row.data(cells).draw(false)

                        $(row.node()).attr('id', `ks-khachhang-row-${id}`);

                        // Adjust widths and responsive breakpoints
                        dt.columns.adjust().responsive?.recalc();

                        //Update thông tin card
                        const card = $(`#ks-khachhang-card-${id}`)
                        if (card && res.views.card) {
                            card.replaceWith(res.views.card.trim());
                        }
                    } else {
                        const $newRow = $(res.views.row.trim());
                        //$('#tb-body-ks-khachhang').prepend($newRow);
                        dt.row.add($newRow).draw(false);
                        dt.order([0, 'desc']).draw(false);
                        const $newCard = $(res.views.card.trim());
                        $('#card-ks-khachhang-container').prepend($newCard);
                    }

                    if (typeof showToast == 'function') {
                        showToast(`Lưu thông tin khách hàng <strong>${ten}</strong> thành công`, "Thông báo", "success");
                    }
                    else {
                        alert(`Lưu thông tin khách hàng ${ten} thành công`)
                    }
                    $modal.modal('hide');
                }
                else {
                    location.reload(); 
                }
            }
        },
        error: function (err) {
            console.error("Kha log: ", err)
            if (typeof showToast == 'function') {
                showToast("Lỗi khi lưu", "Thông báo", "error");
            }
            else {
                alert("Lỗi khi lưu")
            }
        },
        complete: function () {
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


