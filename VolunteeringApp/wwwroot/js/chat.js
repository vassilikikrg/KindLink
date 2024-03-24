"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
var receiverId = document.getElementById("userId").value;

document.getElementById("sendButton").disabled = true; // disable the send button until connection is established.

connection.on("ReceiveMessage", function (userId, message) {
    var activeUserId = document.getElementById("userId").value; // get value from input field
    if (userId === activeUserId) { // if message belongs to the open chat
        $.ajax({
            url: '/Chat/RenderMessage',
            data: { userId: userId, message: message }
        }).done(function (result) {
            document.getElementById("messagesList").insertAdjacentHTML('beforeend', result);
        });
    } else { // display badge to indicate unread messages
        console.log("message was from other user");
        $('.list-group-item[data-userid="' + userId + '"] .badge').text('New message');
    }

});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessageToUser", receiverId ,message).catch(function (err) {
        return console.error(err.toString());
    });
    document.getElementById("messageInput").value = ''; //clear message input field
    
    event.preventDefault();
});