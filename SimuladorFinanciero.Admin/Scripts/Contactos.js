$(document).ready(function () {
    $('.input-daterange').datepicker({
        language: "es",
        orientation: "auto",
        todayHighlight: true,
        endDate: '0',
        autoclose: true
    });

    $('input[type="checkbox"]').change(function () {
        var Estado = "";
        if ($(this).is(':checked')) {
            Estado = "0702";
        }
        else {
            Estado = "0701";
        }

        console.log($(this).data('idsugerencia'));
        console.log(Estado);

        $.ajax({
            type: "GET",
            url: $("#url").data("urlcambiarestado") + "?IdSugerencia=" + $(this).data('idsugerencia') + "&Estado=" + Estado,
            //data: { IdSugerencia: $(this).data('IdSugerencia'), Estado: Estado },
            contentType: false,
            processData: false,
            success: function (data) { },
            error: function (error) {
                if ($(this).is(':checked')) {
                    $(this).checked = false;
                }
                else {
                    $(this).checked = true;
                }

                $.alert({
                    title: "Aviso!",
                    content: "Ha ocurrido un Error",
                    confirmButton: 'Aceptar',
                    confirmButtonClass: 'error'
                });
            }
        });
    });

    $('.ver-mensaje').click(function () {
        var mensaje = $(this).data("id");
        $.ajax({
            type: "GET",
            url: $("#url").data("urlmostrardetalle") + "?IdSugerencia=" + $(this).data('idsugerencia'),
            //data: { IdSugerencia: $(this).data('IdSugerencia'), Estado: Estado },
            contentType: false,
            processData: false,
            success: function (data) {
                $.alert({
                    title: 'Mensaje',
                    content: data,
                    confirmButton: 'Cerrar',
                    confirmButtonClass: 'error',
                    closeIcon: true
                });
            },
            error: function (error) {
                $.alert({
                    title: "Aviso!",
                    content: "Ha ocurrido un error",
                    confirmButton: 'Aceptar',
                    confirmButtonClass: 'error'
                });
            }
        });




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