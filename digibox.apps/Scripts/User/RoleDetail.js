function CheckAll() {
    let ischeckall = $('#checkAll').prop('checked');
    if (ischeckall) {
        $(".isInList").attr("checked","checked");
    } else {
        $(".isInList").removeAttr("checked");
    }
}

function saveMultiple() {
    const roleid = $('#role').val();
    let idsx = [];
    $(".chkchild").each(function () {
        if ($(this).is(':checked')) {
            idsx.push($(this).val());
        }
    });

    let base = $('#_base').val();
    let link = base + "/User/SaveRoles";

    if (idsx.length === 0) {
        toastr.error("Select Function Role")
    }


    $.post(link, { id: roleid, detail: idsx }, function (res) {
        if (res.isSuccess) {
            toastr.success(res.Messages)
            window.location.href = base + "/User/Role";
        } else {
            toastr.error(res.Messages)
        }
    });

}