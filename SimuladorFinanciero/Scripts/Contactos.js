$(document).ready(function () {
    $('.input-daterange').datepicker({
        language: "es",
        orientation: "auto",
        todayHighlight: true,
        endDate: '0',
        autoclose: true
    });

    //$('#btnFiltrar').click(function () {
    //    debugger;
    //    var Desde = $('#fechaInicio').val();
    //    var Hasta = $('#fechaFin').val();
    //    var Tipo = $('#Tipo').prop('selectedIndex');

    //    var formData = new FormData();
    //    formData.append('Desde', Desde);
    //    formData.append('Hasta', Hasta);
    //    formData.append('Tipo', Tipo);

    //    $.ajax({
    //        type: "POST",
    //        url: '@Url.Action("Contactos", "Administrador")',
    //        data: formData,
    //        dataType: 'json',
    //        contentType: false,
    //        processData: false,
    //        success: function (response) {
    //            alert("sucess");
    //        },
    //        error: function (error) {
    //            alert("error");
    //        }
    //    });
    //});
});