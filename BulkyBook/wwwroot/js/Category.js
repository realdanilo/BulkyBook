var dataTable;

$(document).ready(() => {
	loadDataTable();
})

function loadDataTable() {
	dataTable = $("#tblData").DataTable({
		"ajax": {
			"url": "/Admin/Category/GetAll"
		},
		"columns": [
			{ "data": "name", "width": "60%" },
			{
				"data": "id", "render": function (data) {
					return `
							<div class="text-center">
								<a href="/Admin/Category/Upsert/${data}" class="btn btn-success text-white">Edit</a>
								<a onclick="deleteCategory()" class="btn btn-danger text-white">Delete</a>

							</div>
							`}, 
				"width":"40%"
			}
		]
	})
}