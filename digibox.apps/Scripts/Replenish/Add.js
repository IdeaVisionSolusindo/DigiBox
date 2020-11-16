//Replenish Java Script




//global material
let materials = [];
$(function () {
    $('.dte').datetimepicker({
        format: 'DD MMM YYYY',
        userCurrent: false,
    });
});


function add() {
    let base = $('#_base').val();
    let distributorid = $('#distributor').val();
    let link = base + "/Replenish/AddItem";
    $("#dgModal").remove();
    $.get(link, { distributorid }, function (dta) {
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
                    if (res.response.isSuccess === true) {
                        $('.modal-backdrop').hide(); // for black background
                        $('body').removeClass('modal-open'); // F/or scroll run
                        $('#dgModal').modal('hide');
                        $("#dgModal").remove();

                        let exists = materials.find(x => x.id === res.model.id);
                        if (exists) {
                            toastr.error('This Item Exists')
                            return;
                        }
                        toastr.success('Data is added')
                        //insert row to table
                        insertRow(res.model);
                        materials = materials.concat(res.model);
                    } else {
                        $('.modal-backdrop').hide(); // for black background
                        $('body').removeClass('modal-open'); // For scroll run
                        $('#dgModal').modal('hide');
                        $("#dgModal").remove();
                        toastr.error(res.response.Messages)
                    }
                }
            });
            ev.preventDefault();
            ev.stopImmediatePropagation();
            return false;
        })
    })
}


function insertRow(dta) {
    let tbl = document.getElementById('tblDeail');//$('#tblPriceList');
    let row = tbl.insertRow(1);

    let c1 = row.insertCell(0);
    let c2 = row.insertCell(1);
    let c3 = row.insertCell(2);
    let c4 = row.insertCell(3);
    let c5 = row.insertCell(4);
    c1.innerHTML = `<input type='hidden' name='materials' value='${dta.id}' id='hid_${dta.id}' /><label>${dta.partno}</label>`;
    c2.innerHTML = dta.material;
    c3.innerHTML = `<input type='hidden' name='amount' value='${dta.amount}' id='hidp_${dta.id}' /><a href='#' class='amount' data-value=${dta.amount}></a>`;
    c4.innerHTML = `<label>${dta.unit}</label>`;
    c5.innerHTML = `<a title='remove' class='btn btn-white clsremove' data-item='hid_${dta.id}'><i class='fa fa-times text-danger text-sm'></i></a>`

    $('.amount').editable({
        type: 'text',
        mode: 'inline',
        title: 'Enter Amount',
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