function openData() {
    let materialid = $('#materialid').val();
    let base = $('#_base').val();
    let link = base +  "/Pricing/MaterialPrice";
    $.get(link, {id:materialid}, function (res) {
        $('#tblData').html(res);
    })
}

function add() {
    let materialid = $('#materialid').val();
    let base = $('#_base').val();

    let link = base + "/Pricing/Add";
    $("#dgModal").remove();
    $.get(link, { id: materialid }, function (dta) {
        $("body").append(dta);
        $("#dgModal").modal('show');
        let frm = $('#frmAdd');
        $('#datestart').datetimepicker({
            format: 'DD/MM/YYYY'
        });
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

                        toastr.success('Data is Saved')
                        openData();
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

function edit(idx) {
    let base = $('#_base').val();

    let link = base + "/Pricing/Edit"
    $("#dgModal").remove();
    $.get(link, { id: idx }, function (dta) {
        $("body").append(dta);
        $("#dgModal").modal('show');
        let frm = $('#frmEdit');
        $('#datestart').datetimepicker({
            format: 'DD/MM/YYYY'
        });
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
                        toastr.success('Data is Saved')
                        openData();
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
            let link = base+ "/Pricing/Delete";
            $.post(link, { id: idx }, function (res) {
                if (res.isSuccess === true) {
                    toastr.success("Record is Deleted")
                    openData();
                } else {
                    toastr.error("Fail to Delete")
                }
            });
        }
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
            let link = "/Pricing/Propose";
            $.post(link, { id: idx }, function (res) {
                if (res.isSuccess === true) {
                    toastr.success("Record is proposed")
                    openData();
                } else {
                    toastr.error("Fail to Proposed")
                }
            });
        }
    })
}