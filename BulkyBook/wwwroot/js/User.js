var dataTable;

$(document).ready(() => {
	loadDataTable();
}) 

function loadDataTable() {
	dataTable = $("#tblData").DataTable({
		"ajax": {
			"url": "/Admin/User/GetAll"
		},
		"columns": [
			{ "data": "name", "width": "15%" },
			{ "data": "email", "width": "15%" },
			{ "data": "phoneNumber", "width": "15%" },
			{ "data": "company.name", "width": "15%" },
			{ "data": "role", "width": "15%" },
			{
				"data": {
					id: "id",
					lockoutEnd:"lockoutEnd"
				}, "render": function (data) {
					var today = new Date().getTime();
					var lockout = new Date(data.lockoutEnd).getTime()
					if (lockout > today) {
						//use is locked
						//add code to unlock user
						return `
							<div class="text-center">
								<a onclick=LockUnlock("${data.id}") class="btn btn-danger text-white"><i class="fas fa-lock-open"></i>Unlock</a>
								
							</div>
							`
					} else {
						return `
							<div class="text-center">
								<a onclick=LockUnlock("${data.id}") class="btn btn-success text-white"><i class="fas fa-lock"></i>Lock</a>
								
							</div>
							`
					}
				}, 
				"width":"25%"
			}
		]
	})
}

function LockUnlock(id) {
	swal({
		title: "Are you sure you want to lock",
		icon: "warning",
		buttons: true,
		dangerMode:true 
	}).then((res) => {
		//if resp is true to delete
		if (res) {
			$.ajax({
				type: "post",
				url: `/Admin/User/LockUnlock`,
				data: JSON.stringify(id),
				contentType:"application/json",
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