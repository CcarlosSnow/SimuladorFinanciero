$(document).ready(function () {
    $('.input-daterange').datepicker({
        language: "es",
        orientation: "auto",
        todayHighlight: true,
        endDate: '0',
        autoclose: true
    });

    //$("#table-1").DataTable({
    //    lengthChange: false,
    //    ordering: false,
    //    searching: false,
    //    info: false,
    //    pageLength: 1,
    //    order: [[0, "DESC"]],
    //    language: {
    //        url: '../assets/js/datatable-spanish.json'
    //    }
    //});
});