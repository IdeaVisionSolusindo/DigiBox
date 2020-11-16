//Pricing Java Script




//global material
let materials = [];
let selectedmaterial = [];
$(function () {
    materialList();
    $('.dte').datetimepicker({
        format: 'DD MMM YYYY',
        userCurrent: false,
    });


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

function addToList() {
    //get selected on sel2.
    let selected = $('#ddlMaterial').val();
    if (selected.length === 0)
        return;
    //tampil di table
    let rst2 = selected.map(x => {
        return materials.find(y => y.id === x);
    });

    selectedmaterial = selectedmaterial.concat(rst2);
    insertRow(rst2);

    let item = materials.map(function (x) {
        let isselected = selectedmaterial.find(y => { return y !== x.id });
        if (isselected === undefined) {
            return {
                id: x.id, text: x.name
            }
        }
    });
//    let items = item.filter(x => x);

    $('#ddlMaterial').empty();
    $('#ddlMaterial').select2({
        data: items,
        theme: 'bootstrap4',
    })

}

function insertRow(dta) {
    let tbl = document.getElementById('tblPriceList');//$('#tblPriceList');

    dta.forEach(function (x, i) {
        let row = tbl.insertRow(i+1);
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
        mode:'inline',
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


function clearRow() {
    let tbl = document.getElementById('tblPriceList');//$('#tblPriceList');
    var rowcount = tbl.rows;
    tbl.row.deleteRow(1);
}