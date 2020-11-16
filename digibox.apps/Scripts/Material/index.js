function openData(page) {
    let searchparams = $('#searchString').val();
    let val = $('#paramtype').val()
    if ((val !== 'name') && (val !== 'partno')) {
        searchparams = $('#cbosearchString').val();
    }

    let base = $('#_base').val();

    let link = base + "/Material/OpenData";
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


function remove(idx, info) {
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
            let link = base + "/Material/Delete";
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

function setDropDownList(atrname) {
    let base = $('#_base').val();
    let link = base + "/Attribute/GetListSelectItem";
    $.get(link, { attrName: atrname }, function (res) {
        $('#cbosearchString').empty()
        res.map(x => {
            $('#cbosearchString').append(`<option value="${x.Text}"> ${x.Text} </option>`); 
        });
    })
}

function setDistributorList() {
    let base = $('#_base').val();
    let link = base+ "/Distributor/GetListSelectItem";
    $.get(link, function (res) {
        $('#cbosearchString').empty()
        res.map(x => {
            $('#cbosearchString').append(`<option value="${x.Text}"> ${x.Text} </option>`);
        });
    })
}

function paramtypeChanged(val) {
    if ((val === 'name') || (val === 'partno')) {
        $('#dvdropdown').addClass('collapse')
        $('#dvParamText').removeClass('collapse')
        return;
    }
    if (val === "movementtype") {
        setDropDownList("MOVEMENT-TYPE");
    }
    if (val === "sbu") {
        setDropDownList("SBU");
    }
    if (val === "materialtype") {
        setDropDownList("MATERIAL-TYPE");
    }
    if (val === "distributor") {
        setDistributorList();
    }

    $('#dvdropdown').removeClass('collapse')
    $('#dvParamText').addClass('collapse')
}
