$(function () {

    //approval
    var frm = $('#frmApproval');
    //$.validator.unobtrusive.parse(frm);
    frm.bind('submit', function (ev) {
        let fileUpload = $("#file").get(0);
        let files = fileUpload.files[0];
        let formData = new FormData();
        formData.append('file', files);
        formData.append('id', $('#id').val());
        if (files === undefined) {
            toastr.error("Please Upload file attachment")
            ev.preventDefault();
            ev.stopImmediatePropagation();
            return;
        }
        let base = $('#_base').val();
        $.ajax({
            type: "POST",
            url: base + "/Pricing/Approve",
            data: formData,
            cache: false,
            contentType: false,
            processData: false,
            success: function (res) {
                console.log(res);
                if (res.isSuccess === true) {
                    toastr.success(res.Messages)
                    window.location.href = base + "/Pricing/PriceProposalChanges"
                } else {
                    toastr.error(res.Messages)
                }
            }
        });
        ev.preventDefault();
        ev.stopImmediatePropagation();
        return false;
    })
})


function Reject(idx) {
    var base = $("#_base").val();
    let link = base + "/Pricing/Reject";
    $("#dgModal").remove();
    $.get(link, {id:idx},function (dta) {
        $("body").append(dta);
        $("#dgModal").modal('show');
        var frm = $('#frmReject');
        frm.bind('submit', function (ev) {
            $.ajax({
                type: frm.attr('method'),
                url: frm.attr('action'),
                data: frm.serialize(),
                success: function (res) {
                    if (res.isSuccess === true) {
                        $('.modal-backdrop').hide(); // for black background
                        $('body').removeClass('modal-open'); // F/or scroll run
                        $('#dgModal').modal('hide');
                        $("#dgModal").remove();
                        toastr.success('Data is Rejected')
                        window.location.href = base + "/Pricing/PriceProposalChanges";
                    } else {
                        $('.modal-backdrop').hide(); // for black background
                        $('body').removeClass('modal-open'); // For scroll run
                        $('#dgModal').modal('hide');
                        $("#dgModal").remove();
                        toastr.error(res.Messages)
                    }
                }
            });
            ev.preventDefault();
            ev.stopImmediatePropagation();
            return false;
        })
    })
}

