var dataTable;
var url;
$(document).ready(() => {
	loadDataTable();
	url = window.location.search;
	if (url.includes("inprocess")) { loadDataTable("GetOrderList?status=inprocess") }
	else if (url.includes("pending")) { loadDataTable("GetOrderList?status=pending") }
	else if (url.includes("completed")) { loadDataTable("GetOrderList?status=completed") }
	else if (url.includes("rejected")) { loadDataTable("GetOrderList?status=rejected") }
	else loadDataTable("GetOrderList?status=rejected") 

})

function loadDataTable(urlGoTo) {
	dataTable = $("#tblData").DataTable({
		"ajax": {
			"url": "/Admin/Order/"+urlGoTo
		},
		"columns": [
			{ "data": "id", "width": "10%" },
			{ "data": "name", "width": "10%" },
			{ "data": "phoneNumber", "width": "10%" },
			{ "data": "applicationUser.email", "width": "10%" },
			{ "data": "orderStatus", "width": "10%" },
			{ "data": "orderTotal", "width": "10%" },
			{
				"data": "id", "render": function (data) {
					return `
							<div class="text-center">
								<a href="/Admin/Order/Details/${data}" class="btn btn-success text-white">Edit</a>
							</div>
							`}, 
				"width":"5%"
			}
		]
	})
}

