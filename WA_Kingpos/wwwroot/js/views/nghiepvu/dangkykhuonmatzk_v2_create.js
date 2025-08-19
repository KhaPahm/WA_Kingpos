$(document).ready(() => {
    const btnCreate = document.getElementById("btn-create");
    btnCreate.click();

})

$(document).on('click', '#btn-create', function(e) {
    e.preventDefault();
    var url = this.href;
    var $card = $('#customer-create-face-card');
    var $body = $card.find('.card-body');
    console.log(this)
    $body.html('<div class="spinner-border" style="width: 3rem; height: 3rem;" role="status"><span class="sr-only">Loading...</span></div>');

    $body.load(url, function (response, status) {
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
})

