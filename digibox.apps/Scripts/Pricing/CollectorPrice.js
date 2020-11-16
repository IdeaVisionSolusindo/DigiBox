function openData(page) {
    let searchparams = $('#searchString').val();
    let val = $('#paramtype').val()
    let base = $('#_base').val();

    let link = base + "/Pricing/MaterialByCollector";
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
