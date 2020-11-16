
function openData(attrname) {
    if (attrname === "UOM") {
        openUOMData(1);
        return;
    }

    if (attrname === "MOVEMENT-TYPE") {
        openMovementTypeData(1);
        return;
    }

    if (attrname === "MATERIAL-TYPE") {
        openMaterialTypeData(1);
        return;
    }

    if (attrname === "SBU") {
        openSBUData(1);
        return;
    }

}

function openUOMData(page) {
    let base = $('#_base').val();
    let link = base + "/Attribute/OpenUOMData";
    let param = {
        sortOrder: "ASC",
        searchString: $('#searchString').val(),
        page: page
    }
    $.get(link, param, function (res) {
        $('#tblData').html(res);
    })
}

function openMovementTypeData(page) {
    let base = $('#_base').val();
    let link = base + "/Attribute/OpenMovementTypeData";
    let param = {
        sortOrder: "ASC",
        searchString: $('#searchString').val(),
        page: page
    }
    $.get(link, param, function (res) {
        $('#tblData').html(res);
    })
}

function openMaterialTypeData(page) {
    let base = $('#_base').val();
    let link = base + "/Attribute/OpenMaterialTypeData";
    let param = {
        sortOrder: "ASC",
        searchString: $('#searchString').val(),
        page: page
    }
    $.get(link, param, function (res) {
        $('#tblData').html(res);
    })
}

function openSBUData(page) {
    let base = $('#_base').val();
    let link = base + "/Attribute/OpenSBUData";
    let param = {
        sortOrder: "ASC",
        searchString: $('#searchString').val(),
        page: page
    }
    $.get(link, param, function (res) {
        $('#tblData').html(res);
    })
}

function add(attrname) {
    let base = $('#_base').val();
    let link = base + "/Attribute/Add";
    $("#dgModal").remove();
    $.get(link, { attrName: attrname}, function (dta) {
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

                        toastr.success('Data is Saved')
                        openData(attrname);
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

function remove(idx, info, attrname) {
    //Getting current data
    //tampil swal
    let base = $('#_base').val();

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
            let link = base + "/Attribute/Delete";
            $.post(link, { id: idx }, function (res) {
                if (res.isSuccess === true) {
                    toastr.success("Record is Deleted")
                    openData(attrname);
                } else {
                    toastr.error("Fail to Delete")
                }
            });
        }
    })
}

function edit(idx, attrname) {
    let base = $('#_base').val();
    let link = base + "/Attribute/Edit"
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
                        toastr.success("Data is Saved!")
                        openData(attrname);
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