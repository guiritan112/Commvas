$(document).ready(function () {
    // Handle approve and reject button clicks
    $('.btn-approve, .btn-reject').click(function () {
        var userId = $(this).data('userid');
        var status = $(this).data('action') === 'approve' ? 1 : 2;

        // Perform AJAX request to update user status
        $.ajax({
            type: 'POST',
            url: '/UserApproval/UpdateUserStatus',
            data: { userId: userId, status: status },
            success: function (data) {
                if (data.success) {
                    // Update status in the table
                    var statusCell = $('#userTable').find('tr[data-userid="' + userId + '"]').find('.status');
                    statusCell.text(data.status);
                    // Update status color if needed
                    if (data.statusColor) {
                        statusCell.css('background-color', data.statusColor);
                    }
                    // Reload the page after updating the status
                    location.reload();
                } else {
                    // Show error message if status update fails
                    Swal.fire('Error', data.error, 'error');
                }
            },
            error: function (xhr, status, error) {
                // Handle error if AJAX request fails
                console.error(xhr.responseText);
            }
        });
    });

    // Handle edit button click
    $('.btn-edit').click(function () {
        var row = $(this).closest('tr');
        var userId = $(this).data('userid');

        // Change button text to 'Save'
        $(this).text('Save').removeClass('btn-primary btn-edit').addClass('btn-success btn-save');

        // Disable other action buttons
        row.find('.btn-approve, .btn-reject, .btn-delete').prop('disabled', true);

        // Enable editing in the row
        row.find('td:nth-child(3)').each(function () {
            var roleId = $(this).text().trim() === "Admin" ? 1 : 2;
            var select = $('<select>').append(
                $('<option>').val(1).text('Admin'),
                $('<option>').val(2).text('Artist')
            ).val(roleId);
            $(this).html(select);
        });

        // Store the userId as a data attribute of the save button
        $('.btn-save').data('userid', userId);
    });

    // Handle save button click
    $(document).on('click', '.btn-save', function () {
        var row = $(this).closest('tr');
        var userId = $(this).data('userid'); // Retrieve userId from the save button's data attribute
        var roleId = row.find('td:nth-child(3) select').val();

        // Perform AJAX request to update user role
        $.ajax({
            type: 'POST',
            url: '/UserApproval/EditUserRole',
            data: { userId: userId, roleId: roleId },
            success: function (response) {
                if (response.success) {
                    // Update UI with saved data
                    var roleName = roleId == 1 ? 'Admin' : 'Artist';
                    row.find('td:nth-child(3)').text(roleName);

                    // Change button text back to 'Edit'
                    row.find('.btn-save').text('Edit').removeClass('btn-success btn-save').addClass('btn-primary btn-edit');

                    // Enable other action buttons
                    row.find('.btn-approve, .btn-reject, .btn-delete').prop('disabled', false);
                } else {
                    // Handle error
                    Swal.fire('Error', response.error, 'error');
                }
            },
            error: function () {
                // Handle error
                Swal.fire('Error', 'Unable to save data.', 'error');
            }
        });
    });

    // Handle delete button click
    $('.btn-delete').click(function () {
        var userId = $(this).data('userid');

        // Confirm deletion
        Swal.fire({
            title: 'Are you sure?',
            text: 'You won\'t be able to revert this!',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.isConfirmed) {
                // Perform AJAX request to delete user
                $.ajax({
                    type: 'POST',
                    url: '/UserApproval/DeleteUser',
                    data: { userId: userId }, // Pass userId to the back-end
                    success: function (response) {
                        if (response.success) {
                            // Remove row from table
                            $('#userTable').find('tr[data-userid="' + userId + '"]').remove();
                            // Show success message
                            Swal.fire(
                                'Deleted!',
                                'User has been deleted.',
                                'success'
                            );
                        } else {
                            // Show error message
                            Swal.fire('Error', response.error, 'error');
                        }
                    },
                    error: function () {
                        // Handle error
                        Swal.fire('Error', 'Unable to delete user.', 'error');
                    }
                });
            }
        });
    });
});
