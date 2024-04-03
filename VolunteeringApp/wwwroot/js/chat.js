"use strict";

if (!window.connection) {
    // Create the SignalR connection
    window.connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

    // Start the connection
    connection.start().then(function () {
        console.log("SignalR connection established successfully.");
        //document.getElementById("sendButton").disabled = true; // disable the send button until connection is established.
    }).catch(function (err) {
        console.error("Error occurred while establishing SignalR connection:", err.toString());
        return console.error(err.toString());
    });

    window.connection.on("ReceiveMessage", function (conversationId,senderId, message) {
        var activeConvoId = receiverConvoId; // get value from input field
        if (conversationId === activeConvoId) { // if message received belongs to the open chat
            $.ajax({
                url: '/Chat/RenderMessage',
                data: { conversationId: conversationId, userId: senderId, message: message}
            }).done(function (receivedMessageHtml) {
                addToMessagesList(receivedMessageHtml);
            });
        } else { // display badge to indicate unread messages
            $('.list-group-item[data-conversationid="' + conversationId + '"] .badge').text('New message');
        }

    });
}

var receiverConvoId = document.getElementById("conversationId").value;

// Add send button on click listener
document.getElementById("sendButton").addEventListener("click", function (event) {
    var messageInput = document.getElementById("messageInput");
    var message = messageInput.value.trim(); // Trim whitespace from the message input
    if (message) { // Check if message is not empty after trimming
        window.connection.invoke("SendMessageToGroup", receiverConvoId, message)
            .then(function () { // Message was successfully sent
                messageInput.value = ''; // Clear message input field
            })
            .catch(function (err) { // Message not sent
                console.error(err.toString());
            });
    }
    event.preventDefault();
});


// Enable users to send messages by clicking enter
document.getElementById("messageInput").addEventListener("keypress", function (event) {
    if (event.key === "Enter") {
        event.preventDefault();
        document.getElementById("sendButton").click();
    }
});

function addToMessagesList(messageHtml) {
    var messagesList = document.getElementById("messagesList");
    // append the message to the list
    messagesList.insertAdjacentHTML('beforeend', messageHtml);
    // scroll to the bottom of the div
    messagesList.scrollTop = messagesList.scrollHeight;
}