function openData(page) {
    let base = $('#_base').val();

    let link = base + "/Material/AssignMaterial";
    const idx = $('#collector').val();

    //get radio button.
    const FilterList = $("input[name='FilterList']:checked").val();

    let param = {
        id:idx,
        sortOrder: "ASC",
        searchString: $('#searchString').val(),
        page: page,
        showparam: FilterList,
        paramType: $('#paramtype').val()
    }
    $.get(link, param, function (res) {
        $('#tblData').html(res);
    })
}

$(function () {
    $('#chkAll').change(function () {
        const checked = $(this).is(':checked'); 
        $('.chkchild').prop("checked", checked);
    });
    $('.rdassigned').change(function () {
        openData(1);
    });

    openData(1);
})

function assign() {
    //userid
    const colid = $('#collector').val();
    let idsx = [];
    $(".chkchild").each(function () {
        if ($(this).is(':checked')) {
            idsx.push($(this).val());
        }
    });

    let base = $('#_base').val();
    let link = base + "/User/Assign";

    if (idsx.length === 0) {
        toastr.error("Select Material")
    }
    $.post(link, { id: colid, ids: idsx }, function (res) {
        if (res.isSuccess) {
            toastr.success(res.Messages)
        } else {
            toastr.error(res.Messages)
        }
    });
}

function collectorsChanged() {
    openData(1);
}

