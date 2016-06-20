$(document).ready(function () {
    $('#btnSubmit').click(function () {
        $.blockUI({ message: '<h2><img src="../assets/images/loading.gif" /> &nbsp; Espere un momento.</h>' });
        var formData = new FormData();
        var file = $('#file').get(0).files[0];
        formData.append('file', file);
        console.log($('#url').data('urlSubir'));
        $.ajax({
            type: "POST",
            url: $('#url').data('urlsubir'),
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.Estado == "OK") {
                    window.location.href = $('#url').data('urllistado');
                }
                else {
                    $.unblockUI();
                    $.alert({
                        title: response.Titulo,
                        content: response.Texto,
                        confirmButton: 'Aceptar',
                        confirmButtonClass: 'error'
                    });
                }
            },
            error: function (error) {
                $.unblockUI();
                $.alert({
                    title: "Aviso!",
                    content: "Error",
                    confirmButton: 'Aceptar',
                    confirmButtonClass: 'error'
                });
            }
        });
    });
});