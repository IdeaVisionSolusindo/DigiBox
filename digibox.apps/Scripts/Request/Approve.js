let selectedmaterial = [];
$(function () {
    //materialList();
    //selectedMaterial();
});


function add(idx, materialidx) {
    let base = $('#_base').val();
    let link = base + "/Request/AddOutputMaterial";
    $("#dgModal").remove();
    $.get(link, { requestid: idx, materialid: materialidx}, function (dta) {
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
                    if (res.res.isSuccess === true) {
                        $('.modal-backdrop').hide(); // for black background
                        $('body').removeClass('modal-open'); // F/or scroll run
                        $('#dgModal').modal('hide');
                        $("#dgModal").remove();

                        //check data sudah ada dalam array atau belum, jika sudah ada, batalkan.
                        let exists = selectedmaterial.filter(x => x.rfidcode === res.dta.rfidcode);
                        if (exists.length!==0) {
                            toastr.error('Data exists')
                            return;
                        }

                        //update rcv label
                        selectedmaterial.push(res.dta);
                        let currentmaterial = selectedmaterial.filter(x => x.materialid === res.dta.materialid)
                        //calculate total current materaial;
                        let total = currentmaterial.reduce((total, x) => {
                            return total + x.amount
                        }, 0);

                        $(`#lblrcp_${idx}`).html(total);
                        //amount 
                        let amt = $(`#amt_${idx}`).val() * 1.0;
                        let diff = amt - total;
                        $(`#dff_${idx}`).html(diff)
                        toastr.success('Data is Saved')
                        //adding row below
                        let rwi = $(`#tr_${idx}`).index();
                        let tbl = document.getElementById('tblDetail');//$('#tblPriceList');
                        let row = tbl.insertRow(rwi+2);
                        let c0 = row.insertCell(0);
                        let c1 = row.insertCell(1);
                        let c2 = row.insertCell(2);
                        let c3 = row.insertCell(3);
                        let c4 = row.insertCell(4);
                        let c5 = row.insertCell(5);
                        let c6 = row.insertCell(6);
                        let c7 = row.insertCell(7);

                        let dta = res.dta;
                        c0.innerHTML = `<input type='hidden' id='hid${dta.id}'>`;
                        c1.innerHTML = `<input type='hidden' id='mat${dta.id}' >`;
                        c2.innerHTML = `<input name='inventoryid' type='hidden' value='${dta.inventoryid}' >`;
                        c3.innerHTML = `<input name='requestid' type='hidden' value='${idx}' >`;
                        c4.innerHTML = dta.rfidcode;
                        c5.innerHTML = `<input name='amount' type='hidden' value='${dta.amount}' /><label class='label label-danger'>${dta.amount}</label>`;
                        c6.innerHTML = ""
                        c7.innerHTML = `<a title='remove' class='btn btn-white clsremove' data-item='${dta.id}' data-material='${dta.materialid}' parent-id='${idx}'><i class='fa fa-times text-danger text-sm'></i></a>`

                        $('.clsremove').bind("click", function (ev) {
                            //get id
                            let idx = $(this).attr("data-item")
                            let materialid = $(this).attr("data-material")
                            let parentrowid = $(this).attr("parent-id")

                            //get firs itm 
                            let myhid = $(`#hid${idx}`);
                            row = myhid.parent().parent();
                            row.remove();

                            /***LATER */
                            let exists = selectedmaterial.filter(x => x.id !== idx);
                            selectedmaterial = exists;

                            //materialid
                            let currentmaterial = selectedmaterial.filter(x => x.materialid === materialid)
                            let total = currentmaterial.reduce((total, x) => {
                                return total + x.amount
                            }, 0);

                            $(`#lblrcp_${parentrowid}`).html(total);
                            //amount 
                            let amt = $(`#amt_${parentrowid}`).html(total).val() * 1.0;
                            let diff = amt - total;
                            $(`#dff_${parentrowid}`).html(diff)


                        });

                        //Add Row to detail in the bottom.

                        //openData(attrname);
                    } else {
                        $('.modal-backdrop').hide(); // for black background
                        $('body').removeClass('modal-open'); // For scroll run
                        $('#dgModal').modal('hide');
                        $("#dgModal").remove();
                        toastr.error(res.res.Messages)
                    }
                }
            });
            ev.preventDefault();
            ev.stopImmediatePropagation();
            return false;
        })
    })
}

