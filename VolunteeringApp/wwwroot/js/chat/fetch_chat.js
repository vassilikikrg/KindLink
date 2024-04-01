$(document).on('click', '.list-group-item', function (e) {
    e.preventDefault(); // Prevent default link behavior

    // Get the user ID from the data-userid attribute of the clicked link
    var conversationId = $(this).data('conversationid');
    $(this).find('.badge').text(''); // Remove text inside the badge for unread messages

    // Call the RenderChat action with the user ID
    $.ajax({
        url: '/Chat/RenderChat',
        data: { id: conversationId }, // Send the conversation ID as data
        success: function (partialView) {
            $('#chatRoomContainer').html(partialView);
        },
        error: function () {
            console.error('Error occurred while fetching chat room content.');
        }
    });
});

// Trigger click on the first list-group-item, so that the most recent convo is firstly shown by default
$('.list-group-item:first').click();
