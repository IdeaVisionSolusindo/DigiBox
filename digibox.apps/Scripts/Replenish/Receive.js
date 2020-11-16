
let materials = [];

$(function () {
    openDetail();
});

function openDetail() {
    let base = $('#_base').val();
    let id = $('#id').val();
    let link = base + "/Replenish/ReplenishItemsById";
    $.get(link, { id }, function (res) {
        materials = res;
        res.forEach(function (x, i) {
            insertRow(x)
        });
    });
}



function insertRow(dta) {
    let tbl = document.getElementById('tblDetail');//$('#tblPriceList');
    let row = tbl.insertRow(1);

    let c1 = row.insertCell(0);
    let c2 = row.insertCell(1);
    let c3 = row.insertCell(2);
    let c4 = row.insertCell(3);
    let c5 = row.insertCell(4);
    let c6 = row.insertCell(5);
    c1.innerHTML = `<input type='hidden' name='ids' value='${dta.id}' id='hid_${dta.id}' /><label>${dta.partno}</label>`;
    c2.innerHTML = `<label>${dta.material}</label>`;
    c3.innerHTML = `<label id='lblamount_${dta.id}'>${dta.amount}</label>`;
    c4.innerHTML = `<input type='hidden' name='receive' value='${dta.amount}' id='${dta.id}' /><a href='#' class='amount' data-value=${dta.amount}></a>`;
    c5.innerHTML = `<label id='lbldiff_${dta.id}'>0</label>`;
    c6.innerHTML = `<label>${dta.unit}</label>`;

    $('.amount').editable({
        type: 'text',
        mode: 'inline',
        title: 'Enter Amount Received',
        success: function (response, newValue) {
            $(this).siblings('input:hidden').val(newValue);
            let id = $(this).siblings('input:hidden').prop('id');
            let oldvalue = $(`#lblamount_${id}`).html();
            let diff = oldvalue - newValue;
            $(`#lbldiff_${id}`).html(diff);

        }
    });

    $('.clsremove').bind("click", function (ev) {
        let attri = $(this).attr("data-item")
        let id = $(`#${attri}`).val();
        let cell = $(`#${attri}`).parent();
        let row = cell.parent();
        row.remove();

        //begin mengembalikan ke dropdown
        let material = selectedmaterial.filter(x => x.id !== id);
        selectedmaterial = material;
        let item = materials.map(function (x) {
            //find seledted
            let isselected = material.find(y => { return y.id === x.id });
            if (isselected === undefined) {
                return {
                    id: x.id, text: x.name
                }
            }
        });
        let items = item.filter(x => x);

        $('#ddlMaterial').empty();
        $('#ddlMaterial').select2({
            data: items
        })
        //end mengembalikan ke drop down

    });
}