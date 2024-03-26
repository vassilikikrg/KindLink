"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
var receiverId = document.getElementById("userId").value;

document.getElementById("sendButton").disabled = true; // disable the send button until connection is established.

connection.on("ReceiveMessage", function (id,username, message) {
    var activeUserId = receiverId; // get value from input field
    if (id === activeUserId) { // if message received belongs to the open chat
        $.ajax({
            url: '/Chat/RenderMessage',
            data: { userId: id, userName: username, message: message,sent:false }
        }).done(function (receivedMessageHtml) {
            addToMessagesList(receivedMessageHtml);
        });
    } else { // display badge to indicate unread messages
        console.log("message was from other user");
        $('.list-group-item[data-userid="' + id + '"] .badge').text('New message');
    }

});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessageToUser", receiverId, message)
        .then(function () { // message was successfully sent
            document.getElementById("messageInput").value = ''; //clear message input field

            $.ajax({
                url: '/Chat/RenderMessage',
                data: { userId: null, userName: null, message: message, sent: true }
            }).done(function (sentMessageHtml) {
                addToMessagesList(sentMessageHtml);
            });

        })
        .catch(function (err) { // message not sent
        return console.error(err.toString());
        });

    
    event.preventDefault();
});
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