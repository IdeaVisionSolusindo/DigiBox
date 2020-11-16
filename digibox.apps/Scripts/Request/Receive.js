


function selectedMaterial() {
    let base = $('#_base').val();
    let link = base + "/Request/SelectedMaterial";
    let idx = $('#id').val();
    //$("#dgModal").remove();
    $.get(link, { id: idx }, function (res) {
        selectedmaterial = res;
        console.log(selectedmaterial);
        //reset dropdown;
        let material = selectedmaterial.filter(x => x.id !== id);
        console.log(material);
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
            data: items,
            theme: 'bootstrap4'
        })

        let tbl = document.getElementById('tblDetail');//$('#tblPriceList');

        res.forEach(function (x, i) {
            let row = tbl.insertRow(1);
            let c1 = row.insertCell(0);
            let c2 = row.insertCell(1);
            let c3 = row.insertCell(2);
            let c4 = row.insertCell(3);
            let c5 = row.insertCell(4);
            c1.innerHTML = `<input type='hidden' name='materials' value='${x.id}' id='hid_${x.id}' /><label>${x.partno}</label>`;
            c2.innerHTML = x.name;
            c3.innerHTML = `<input type='hidden' name='amount' value='${x.amount}' id='hidp_${x.id}' /><a href='#' class='amount' data-value=${x.amount}></a>`;
            c4.innerHTML = x.unit;
            c5.innerHTML = `<a title='remove' class='btn btn-white clsremove' data-item='hid_${x.id}'><i class='fa fa-times text-danger text-sm'></i></a>`
        });

        //make editable price
        $('.amount').editable({
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
                data: items,
                theme: 'bootstrap4'
            })
            //end mengembalikan ke drop down

        });
    })
}
