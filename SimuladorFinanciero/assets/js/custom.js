$(document).ready(function() {

	$(document).on('change', '.btn-file :file', function() {
		var input = $(this),
			numFiles = input.get(0).files ? input.get(0).files.length : 1,
			label = input.val().replace(/\\/g, '/').replace(/.*\//, '');
		input.trigger('fileselect', [numFiles, label]);
	});

	$(document).ready( function() {
		$('.btn-file :file').on('fileselect', function(event, numFiles, label) {
			var input = $(this).parents('.input-group').find(':text'),
				log = numFiles > 1 ? numFiles + ' files selected' : label;
			if( input.length ) {
				input.val(log);
			}
		});
	});

	$(".sidebar.right").sidebar({side: "right"});

	$(".navbar-toggle.collapsed").click(function(){
		$(".sidebar.right").css("display","inherit");
		$(".sidebar.right").trigger("sidebar:open");
	});

	$(".btn-close").click(function(){
		$(".sidebar.right").trigger("sidebar:close");
	});

	$('a.sider_menu_item').click(function(){
		$(".sidebar.right").trigger("sidebar:close");
	});

});