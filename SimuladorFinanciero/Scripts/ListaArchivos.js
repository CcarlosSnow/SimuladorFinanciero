$(document).ready(function () {
    $(".remove-file").confirm({
        title: 'Confirmar!',
        content: '¿Está seguro de eliminar el archivo?',
        confirmButton: 'Eliminiar',
        cancelButton: 'Cerrar',
        confirmButtonClass: 'buttonConfirm',
        cancelButtonClass: 'buttonConfirmCancel'
    });

    $(".active-file").confirm({
        title: 'Confirmar!',
        content: '¿Está seguro de activar este archivo?',
        confirmButton: 'Activar',
        cancelButton: 'Cerrar',
        confirmButtonClass: 'buttonConfirm',
        cancelButtonClass: 'buttonConfirmCancel',
        confirm: function () {
            $.blockUI({ message: '<h2><img src="../assets/images/loading.gif" /> &nbsp; Espere un momento.</h2>' });
            window.location = this.$target.attr('href');
        }
    });
});