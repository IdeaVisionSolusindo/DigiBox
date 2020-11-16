$(document).ready(function () {
    let frmAddAttachment = $('#frmUpload');
    $.validator.unobtrusive.parse(frmAddAttachment);
    frmAddAttachment.on("submit", function (ev) {
        ev.preventDefault();
        let token = $('input[name="__RequestVerificationToken"]', frmAddAttachment).val();
        let fileUpload = $("#file").get(0);
        let files = fileUpload.files[0];
        let formData = new FormData();
        formData.append('file', files);
        let base = $("#_base").val();
        $.ajax({
            type: "POST",
            url: base + "/Material/Upload",
            data: formData,
            cache: false,
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.isSuccess===true) {
                    toastr.success("Data is Uploaded")
                    openData(1);
                    $('#dvSubmit').css('visibility', 'visible');
                    $('#btnUpload').prop('disabled', 'true');
                } else {
                    toastr.error(res.Messages)
                }
            }
        });
        ev.stopImmediatePropagation();
        return false;
    })
});

function openData(page) {
    let base = $('#_base').val();
    let link = base + "/Material/DisplayUploadedData";
    $.get(link, { page}, function (res) {
        $('#tblData').html(res);
    })
}

function save() {
    let base = $('#_base').val();
    let link = base + "/Material/SaveMultiple";
    $('#btnsave').html('Saving...');
    $('#btnsave').prop('disabled', 'true');
    $.post(link, function (res) {
        if (res.isSuccess === true) {
            toastr.success("Data is Uploaded")
            window.location.href = base + "/Material";
        } else {
            toastr.error(res.Messages)
        }
    })
}