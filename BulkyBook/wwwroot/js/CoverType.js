var dataTable;

$(document).ready(() => {
	loadDataTable();
})

function loadDataTable() {
	dataTable = $("#tblData").DataTable({
		"ajax": {
			"url": "/Admin/CoverType/GetAll"
		},
		"columns": [
			{ "data": "name", "width": "60%" },
			{
				"data": "id", "render": function (data) {
					return `
							<div class="text-center">
								<a href="/Admin/CoverType/Upsert/${data}" class="btn btn-success text-white">Edit</a>
								<a onclick=deleteCategory("/Admin/CoverType/Delete/${data}") class="btn btn-danger text-white">Delete</a>
								
							</div>
							`}, 
				"width":"40%"
			}
		]
	})
}

function deleteCategory(url) {
	swal({
		title: "Are you sure you want to delete",
		text: "you wont be able to restore the data",
		icon: "warning",
		buttons: true,
		dangerMode:true 
	}).then((res) => {
		//if resp is true to delete
		if (res) {
			$.ajax({
				type: "delete",
				url,
				success: function ({ success, message }) {
					if (success) {
						toastr.success(message)
						dataTable.ajax.reload()
					} else {
						toastr.error(message)
					}
				}
			})
		}
	})
}