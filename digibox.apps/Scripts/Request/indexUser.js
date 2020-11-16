function openData(page) {
    let searchparams = $('#searchString').val();
    let val = $('#paramtype').val()
    if ((val !== 'name') && (val !== 'partno')) {
        searchparams = $('#cbosearchString').val();
    }

    let base = $('#_base').val();

    let link = base + "/Request/RequestByUser";
    let param = {
        sortOrder: "ASC",
        searchString: searchparams,
        paramType: $('#paramtype').val(),
        page: page
    }
    $.get(link, param, function (res) {
        $('#tblData').html(res);
    })
}


function Post(idx) {
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
            let link = "/Request/PostRequest";
            $.post(link, { id: idx }, function (res) {
                if (res.isSuccess === true) {
                    toastr.success("Record is proposed")
                    openData(1);
                } else {
                    toastr.error("Fail to Proposed")
                }
            });
        }
    })
}

function remove(idx) {
    //Getting current data
    //tampil swal

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
            let link = base + "/Request/Delete";
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