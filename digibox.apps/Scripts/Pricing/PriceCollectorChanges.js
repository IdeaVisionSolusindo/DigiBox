function openData(page) {
    let base = $('#_base').val();
    let link = base + "/Pricing/PriceCollectorProposalList";
    let param = {
        sortOrder: "ASC",
        searchString: $('#searchString').val(),
        paramType: $('#paramtype').val(),
        page: page
    }
    $.get(link, param, function (res) {
        $('#tblData').html(res);
    })
}


function Propose(idx) {
    //Getting current data
    //tampil swal

    Swal.fire({
        type: 'question',
        title: `Do you want to propose this price?`,
        showDenyButton: true,
        showCancelButton: true,
        confirmButtonText: `Propose Price`,
        denyButtonText: `Cancel`,
    }).then((result) => {
        /* Read more about isConfirmed, isDenied below */
        if (result.isConfirmed) {
            let base = $('#_base').val();
            let link = base + "/Pricing/Propose";
            $.post(link, { id: idx }, function (res) {
                if (res.isSuccess === true) {
                    toastr.success("Record is proposed")
                    openData(1);
                } else {
                    toastr.error(res.Messages)
                }
            });
        }
    })
}

function remove(idx) {
    Swal.fire({
        type: 'question',
        title: `Do you want to delete this record ?`,
        showDenyButton: true,
        showCancelButton: true,
        confirmButtonText: `Delete`,
        denyButtonText: `Cancel`,
    }).then((result) => {
        /* Read more about isConfirmed, isDenied below */
        if (result.isConfirmed) {
            let base = $('#_base').val();
            let link = base + "/Pricing/Delete";
            $.post(link, { id: idx }, function (res) {
                if (res.isSuccess === true) {
                    toastr.success("Record is Deleted")
                    openData(1);
                } else {
                    toastr.error("Fail to Delete")
                }
            });
        }
    })
}