

//const video = document.getElementById('cameraVideo');
//const canvas = document.getElementById('cameraCanvas');
//const previewImg = document.getElementById('photoPreview');
//const btnUsePhoto = document.getElementById('btnUsePhoto');
//const btnRetake = document.getElementById('btnRetake');
//const btnTakePhoto = document.getElementById('btnTakePhoto');
//const btnSwitchCamera = document.getElementById('btnSwitchCamera');

$('#studentsTable').DataTable({
    autoWidth: true,     // let DT compute widths
    // responsive: true, // if you use it
});

function showDeleteCustomer(id) {
    $("#btn-confirm-delete").attr('data', id);
    var $modal = $('#confirmDeleteModal');
    $modal.modal('show');
    $("#btn-confirm-delete").attr("data", id);
}

$('#btn-confirm-delete').on('click', function () {
    var id = $("#btn-confirm-delete").attr("data");
    deleteCustomer(id)
})

function deleteCustomer(id) {
    console.log(`?handler=Del`)
    const token = $('input[name="__RequestVerificationToken"]').val();
    $.ajax({
        url: `/Nghiepvu/DangKyKhuonMatZk_V2/_KS_KhachHang?handler=Del`,          // calls OnPostDel
        type: 'POST',
        headers: { 'RequestVerificationToken': token }, // <-- critical
        data: { id },                 // form-encoded binds to int id
        success: function (json) {
            if (json && json.ok) {
                const dt = $('#tablefull').DataTable();
                $(`#ks-khachhang-row-${id}`).remove();
                dt.row(`#ks-khachhang-row-${id}`).remove();
                dt.columns.adjust().responsive?.recalc();
                $(`#ks-khachhang-card-${id}`).remove();


                if (typeof showToast == 'function') {
                    showToast(`Xóa dữ liệu khách hàng thành công`, "Thông báo", "success");
                    var $modal = $('#confirmDeleteModal');
                    $modal.modal('hide');
                }
                else {
                    alert(` dữ liệu khách hàng thành công`)
                    location.reload();
                }
            } else {
                location.reload();
            }
        },
        error: xhr => console.log('Lỗi')
    })
}

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

        $('#reservationdatetime_NgaySinh').datetimepicker({ format: "DD/MM/YYYY" });
        $('#reservationdatetime_TuNgay').datetimepicker({ format: "DD/MM/YYYY HH:mm:ss", icons: { time: 'far fa-clock' } });
        $('#reservationdatetime_DenNgay').datetimepicker({ format: "DD/MM/YYYY HH:mm:ss", icons: { time: 'far fa-clock' } });
    });
});
