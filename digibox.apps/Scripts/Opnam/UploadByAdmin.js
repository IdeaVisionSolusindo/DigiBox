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
            url: base + "/Opnam/UploadByAdmin",
            data: formData,
            cache: false,
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.res === undefined) {
                    toastr.success("Data is Uploaded")
                    $('#tblData').html(res);
                } else {
                    toastr.error(res.res.Messages)
                }
            }
        });
        ev.stopImmediatePropagation();
        return false;
    })
});

function save() {
    let base = $("#_base").val();
    $.ajax({
        type: "POST",
        url: base + "/Opnam/SaveMultiple",
        success: function (res) {
            if (res.isSuccess === true) {
                toastr.error(res.Messages)
                window.location.href = base + "/Opnam";
            } else {
                toastr.error(res.Messages)
            }
        }
    });
}



