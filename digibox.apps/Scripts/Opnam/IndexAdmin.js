function openData(page) {
    let searchparams = $('#searchString').val();
    let val = $('#paramtype').val()
    if ((val !== 'name') && (val !== 'partno')) {
        searchparams = $('#cbosearchString').val();
    }

    let base = $('#_base').val();

    let link = base + "/Opnam/OpenDataBySuperadmin";
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


function post(idx) {
    Swal.fire({
        type: 'question',
        title: `Do you want to post this data?`,
        showDenyButton: true,
        showCancelButton: true,
        confirmButtonText: `Post Data`,
        denyButtonText: `Cancel`,
    }).then((result) => {
        /* Read more about isConfirmed, isDenied below */
        if (result.isConfirmed) {
            let base = $('#_base').val();
            let link = base + "/Opnam/Post";
            $.post(link, { id: idx }, function (res) {
                if (res.isSuccess === true) {
                    toastr.success(res.Messages);
                    openData(1);
                    window.location.href = base + "/Opnam";
                } else {
                    toastr.success(res.Messages)
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
            let link = base + "/Opnam/Delete";
            $.post(link, { id: idx }, function (res) {
                if (res.isSuccess === true) {
                    toastr.success(res.Messages)
                    openData(1);
                } else {
                    toastr.error(res.Messages)
                }
            });
        }
    })
}