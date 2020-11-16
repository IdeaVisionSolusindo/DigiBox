$(function () {
    openDetail();
});

function openDetail() {
    let base = $('#_base').val();
    let id = $('#id').val();
    let link = base + "/Replenish/ReplenishDetailById";
    $.get(link, { id }, function (res) {
        res.forEach(function (x, i) {
            insertRow(x)
        });
    });
}


function insertRow(dta) {
    let base = $('#_base').val();
    let tbl = document.getElementById('tblDetail');//$('#tblPriceList');
    let row = tbl.insertRow(1);

    let c1 = row.insertCell(0);
    let c2 = row.insertCell(1);
    let c3 = row.insertCell(2);
    let c4 = row.insertCell(3);
    let c5 = row.insertCell(4);
    let c6 = row.insertCell(5);
    c1.innerHTML = `<label>${dta.rfidcode}</label>`;
    c2.innerHTML = `<input type='hidden' name='materials' value='${dta.materialid}' id='hid_${dta.materialid}' /><label>${dta.partno}</label>`;
    c3.innerHTML = dta.material;
    c4.innerHTML = dta.amount;
    c5.innerHTML = `<label>${dta.unit}</label>`;
    c6.innerHTML = `<a href="${base}/Replenish/PrintRFID/${dta.id}"> <i class="fa fa-print"></i></a>`;
}