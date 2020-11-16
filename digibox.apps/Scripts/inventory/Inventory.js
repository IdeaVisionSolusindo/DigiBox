function openData(page) {
    let base = $('#_base').val();
    let idx = $('#id').val();
    let link = base + "/Inventory/DetailHistory";
    let param = {
        id:idx,
        sortOrder: "ASC",
        page: page
    }
    $.get(link, param, function (res) {
        $('#tblData').html(res);
    })
}