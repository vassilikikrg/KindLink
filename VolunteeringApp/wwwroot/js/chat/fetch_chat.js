$(document).on('click', '.list-group-item', function (e) {
    e.preventDefault(); // Prevent default link behavior

    // Get the user ID from the data-userid attribute of the clicked link
    var userId = $(this).data('userid');

    // Call the RenderChat action with the user ID
    $.ajax({
        url: '/Chat/RenderChat',
        data: { receiverId: userId }, // Send the user ID as data
        success: function (partialView) {
            $('#chatRoomContainer').html(partialView);
        },
        error: function () {
            console.error('Error occurred while fetching chat room content.');
        }
    });
});
