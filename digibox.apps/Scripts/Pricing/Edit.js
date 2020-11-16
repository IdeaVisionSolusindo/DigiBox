
let materials = [];
let selectedmaterial = [];
$(function () {
//initialize date
    $('.dte').datetimepicker({
        format: 'DD MMM YYYY'
    });

    materialList();
    selectedMaterial();
});


function materialList() {
    let base = $('#_base').val();
    let link = base + "/Pricing/MaterialPriceByDistributor";
    let idx = $('#distributorid').val();
    //$("#dgModal").remove();
    $.get(link, { id: idx }, function (res) {
        materials = res;
        let items = res.map(function (x) {
            return {
                id: x.id, text: x.name
            }
        });
        $('#ddlMaterial').empty();
        $('#ddlMaterial').select2({
            data: items,
            theme: 'bootstrap4',
        })
    })
}

function selectedMaterial() {
    let base = $('#_base').val();
    let link = base + "/Pricing/SelectedMaterial";
    let idx = $('#id').val();
    //$("#dgModal").remove();
    $.get(link, { priceid: idx }, function (res) {
        selectedmaterial = res;
        //console.log(selectedmaterial);
        //reset dropdown;
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

        let tbl = document.getElementById('tblPriceList');//$('#tblPriceList');

        res.forEach(function (x, i) {
            let row = tbl.insertRow(i + 1);
            let c1 = row.insertCell(0);
            let c2 = row.insertCell(1);
            let c3 = row.insertCell(2);
            let c4 = row.insertCell(3);
            let c5 = row.insertCell(4);
            c1.innerHTML = `<input type='hidden' name='materials' value='${x.id}' id='hid_${x.id}' /><label>${x.partno}</label>`;
            c2.innerHTML = x.name;
            c3.innerHTML = `<input type='hidden' name='currentprices' value='${x.currentprice}' id='hidcp_${x.id}' /><label>${x.currentprice}</label>`;
            c4.innerHTML = `<input type='hidden' name='prices' value='${x.proposedprice}' id='hidp_${x.id}' /><a href='#' class='price' data-value=${x.proposedprice}></a>`;
            c5.innerHTML = `<a title='remove' class='btn btn-white clsremove' data-item='hid_${x.id}'><i class='fa fa-times text-danger text-sm'></i></a>`
        });

        //make editable price
        $('.price').editable({
            type: 'text',
            mode: 'inline',
            title: 'Enter price',
            success: function (response, newValue) {
                $(this).siblings('input:hidden').val(newValue);
            }
        });

        //this function is called when by remove item
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
    })
}

function insertRow(dta) {
    let tbl = document.getElementById('tblPriceList');//$('#tblPriceList');

    dta.forEach(function (x, i) {
        let row = tbl.insertRow(i + 1);
        let c1 = row.insertCell(0);
        let c2 = row.insertCell(1);
        let c3 = row.insertCell(2);
        let c4 = row.insertCell(3);
        let c5 = row.insertCell(4);
        c1.innerHTML = `<input type='hidden' name='materials' value='${x.id}' id='hid_${x.id}' /><label>${x.partno}</label>`;
        c2.innerHTML = x.name;
        c3.innerHTML = `<input type='hidden' name='currentprices' value='${x.currentprice}' id='hidcp_${x.id}' /><label>${x.currentprice}</label>`;
        c4.innerHTML = `<input type='hidden' name='prices' value='${0}' id='hidp_${x.id}' /><a href='#' class='price' data-value=${0}></a>`;
        c5.innerHTML = `<a title='remove' class='btn btn-white clsremove' data-item='hid_${x.id}'><i class='fa fa-times text-danger text-sm'></i></a>`
    });

    $('.price').editable({
        type: 'text',
        title: 'Enter price',
        success: function (response, newValue) {
            $(this).siblings('input:hidden').val(newValue);
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

function addToList() {
    //get selected on sel2.
    let selected = $('#ddlMaterial').val();
    if (selected.length === 0)
        return;
    //tampil di table
    let rst2 = selected.map(x => {
        return materials.find(y => y.id === x);
    });

    let item = materials.map(function (x) {
        let isselected = selected.find(y => { return y === x.id });
        if (isselected === undefined) {
            return {
                id: x.id, text: x.name
            }
        }
    });
    let items = item.filter(x => x);
    insertRow(rst2);

    selectedmaterial = selectedmaterial.concat(rst2);
    $('#ddlMaterial').empty();
    $('#ddlMaterial').select2({
        data: items
    })

}
