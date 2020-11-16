function openData(page) {
    let base = $('#_base').val();
    let link = base + "/User/OpenData";
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
    var base = $("#_base").val();
    var link = base + "/User/Add"
    $("#dgModal").remove();
    $.get(link, function (dta) {
        $("body").append(dta);
        $("#dgModal").modal('show');

        //seting disable and enable ppjk
        var optionSelected = $('#roleid').find("option:selected");
        var textSelected = optionSelected.text();
        $('#ppjkid').prop('disabled', textSelected !== "PPJK");

        $('#roleid').change(function () {
            var optionSelected = $(this).find("option:selected");
            var textSelected = optionSelected.text();
            $('#ppjkid').prop('disabled', textSelected !== "PPJK");
        });

        var frm = $('#frmAdd');
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

    let link = base + "/User/Edit"
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

function remove(idx, info) {
    //Getting current data
    //tampil swal

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
            let link = base + "/User/Delete";
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