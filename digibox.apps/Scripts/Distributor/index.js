function openData(page) {
    let base = $('#_base').val();
    let link = base + "/Distributor/OpenData";
    let param = {
        sortOrder: "ASC",
        searchString: $('#searchString').val(),
        page: page
    }
    $.get(link, param, function (res) {
        $('#tblData').html(res);
    })
}

function add() {

    const Toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 3000
    });
    let base = $('#_base').val();

    let link = base + "/Distributor/Add";
    $("#dgModal").remove();
    $.get(link, function (dta) {
        $("body").append(dta);
        $("#dgModal").modal('show');
        let frm = $('#frmAdd');
        $.validator.unobtrusive.parse(frm);
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
                        Toast.fire({
                            type: 'success',
                            title: 'Data is Saved !'
                        })
                        openData();
                    } else {
                        $('.modal-backdrop').hide(); // for black background
                        $('body').removeClass('modal-open'); // For scroll run
                        $('#dgModal').modal('hide');
                        $("#dgModal").remove();
                        Toast.fire({
                            type: 'error',
                            title: res.Messages
                        })
                    }
                }
            });
            ev.preventDefault();
            ev.stopImmediatePropagation();
            return false;
        })
    })
}

function remove(idx, info) {
    //Getting current data
    //tampil swal

    const Toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 3000
    });

    Swal.fire({
        type: 'question',
        title: `Do you want to delete \n${info} ?`,
        showDenyButton: true,
        showCancelButton: true,
        confirmButtonText: `Delete`,
        denyButtonText: `Cancel`,
    }).then((result) => {
        /* Read more about isConfirmed, isDenied below */
        if (result.isConfirmed) {
            let base = $('#_base').val();
            let link = base + "/Distributor/Delete";
            $.post(link, { id: idx }, function (res) {
                if (res.isSuccess===true) {
                    Toast.fire({
                        icon:'success',
                        type: 'success',
                        text: "Record is Deleted",
                    })
                    openData();
                } else {
                    Toast.fire({
                        type: 'error',
                        type: 'error',
                        text: "Fail to Delete",
                    })
                }
            });
        } 
    })
} 

function edit(idx) {

    const Toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 3000
    });
    let base = $('#_base').val();

    let link = base + "/Distributor/Edit"
    $("#dgModal").remove();
    $.get(link, { id: idx }, function (dta) {
        $("body").append(dta);
        $("#dgModal").modal('show');
        let frm = $('#frmEdit');
        $.validator.unobtrusive.parse(frm);
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
                        Toast.fire({
                            type: 'success',
                            title: 'Data is Saved !'
                        })
                        openData();
                    } else {
                        $('.modal-backdrop').hide(); // for black background
                        $('body').removeClass('modal-open'); // For scroll run
                        $('#dgModal').modal('hide');
                        $("#dgModal").remove();
                        Toast.fire({
                            type: 'error',
                            title: res.Messages
                        })
                    }
                }
            });
            ev.preventDefault();
            ev.stopImmediatePropagation();
            return false;
        })
    })
}