function openData(page) {
    let base = $('#_base').val();
    let link = base + "/Pricing/PriceManagerProposalList";
    let param = {
        sortOrder: "ASC",
        searchString: $('#searchString').val(),
        paramType: $('#paramtype').val(),
        page: page
    }
    $.get(link, param, function (res) {
        $('#tblData').html(res);
    })
}