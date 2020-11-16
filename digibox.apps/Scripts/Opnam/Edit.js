function remove(idx) {
    //Getting current data
    //tampil swal
    let base = $('#_base').val();

    Swal.fire({
        type: 'question',
        title: `Do you want to delete?`,
        showDenyButton: true,
        showCancelButton: true,
        confirmButtonText: `Delete`,
        denyButtonText: `Cancel`,
    }).then((result) => {
        /* Read more about isConfirmed, isDenied below */
        if (result.isConfirmed) {
            let link = base + "/Opnam/removeDetail";
            $.post(link, { id: idx }, function (res) {
                $('#tblContent').html(res);
            });
        }
    })
}

function save() {
    let base = $('#_base').val();
    let link = base + "/Opnam/SaveEdit";
    $.post(link, function (res) {
        if (res.isSuccess === true) {
            toastr.success('Data is Saved')
            window.location.href = base + "/Opnam";
        } else {
            toastr.error(res.Messages)
        }
    })
}
