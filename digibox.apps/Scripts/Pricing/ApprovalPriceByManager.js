function openData() {
    let materialid = $('#materialid').val();
    let base = $('#_base').val();
    let link = base + "/Pricing/ApprovalMaterialPriceByManager";
    $.get(link, { id: materialid }, function (res) {
        $('#tblData').html(res);
    })
}

function Approve(idx) {
    Swal.fire({
        type: 'question',
        title: `Do you want to approve this price?`,
        showDenyButton: true,
        showCancelButton: true,
        confirmButtonText: `Approve Price`,
        denyButtonText: `Cancel`,
    }).then((result) => {
        /* Read more about isConfirmed, isDenied below */
        if (result.isConfirmed) {
            let base = $('#_base').val();
            let link = base + "/Pricing/Approve";
            $.post(link, { id: idx }, function (res) {
                if (res.isSuccess === true) {
                    toastr.success("Record is Approved")
                    openData();
                } else {
                    toastr.error("Fail to Approve")
                }
            });
        }
    })
}

function Reject(idx) {
    Swal.fire({
        type: 'question',
        title: `Do you want to approve this price?`,
        showDenyButton: true,
        showCancelButton: true,
        confirmButtonText: `Reject Price`,
        denyButtonText: `Cancel`,
    }).then((result) => {
        /* Read more about isConfirmed, isDenied below */
        if (result.isConfirmed) {
            let base = $('#_base').val();
            let link = base + "/Pricing/Reject";
            $.post(link, { id: idx }, function (res) {
                if (res.isSuccess === true) {
                    toastr.success(res.Messages)
                    openData();
                } else {
                    toastr.error(res.Messages)
                }
            });
        }
    })
}

