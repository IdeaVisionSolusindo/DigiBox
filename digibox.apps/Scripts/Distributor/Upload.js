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
            url: base + "/Distributor/Upload",
            data: formData,
            cache: false,
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.res===undefined) {
                    toastr.success("Data is Uploaded")
                    $('#tblData').html(res);
                    DisplayData();
                } else {
                    toastr.error(res.res.Messages)
                }
            }
        });
        ev.stopImmediatePropagation();
        return false;
    })
});

function DisplayData() {
    let frmsave = $('#frmsaveMultiple');
    let base = $("#_base").val();
    frmsave.on("submit", function (ev) {
        $.ajax({
            type: "POST",
            url: base + "/Distributor/SaveMultiple",
            success: function (res) {
                if (res.isSuccess === true) {
                    toastr.error(res.Messages)
                    window.location.href = base + "/Distributor";
                } else {
                    toastr.error(res.Messages)
                }
            }
        });
        ev.preventDefault();
        ev.stopImmediatePropagation();
        return false;
    });
}



