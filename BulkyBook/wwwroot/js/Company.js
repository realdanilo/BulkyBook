var dataTable;

$(document).ready(() => {
	loadDataTable();
})

function loadDataTable() {
	dataTable = $("#tblData").DataTable({
		"ajax": {
			"url": "/Admin/Company/GetAll"
		},
		"columns": [
			{ "data": "name", "width": "15%" },
			{ "data": "streetAddress", "width": "15%" },
			{ "data": "city", "width": "10%" },
			{ "data": "state", "width": "10%" },
			{ "data": "phoneNumber", "width": "15%" },
			{
				"data": "isAuthorizedCompany", "render": function (data) {
					if (data) {
						return `<input type="checkbox" disable checked/>`
					} else {
						return `<input type="checkbox" disable />`

					}
				}
				, "width": "10%"
			},
			{
				"data": "id", "render": function (data) {
					return `
							<div class="text-center">
								<a href="/Admin/Company/Upsert/${data}" class="btn btn-success text-white">Edit</a>
								<a onclick=deleteCategory("/Admin/Company/Delete/${data}") class="btn btn-danger text-white">Delete</a>
								
							</div>
							`}, 
				"width":"25%"
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