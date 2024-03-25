"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
var receiverId = document.getElementById("userId").value;

document.getElementById("sendButton").disabled = true; // disable the send button until connection is established.

connection.on("ReceiveMessage", function (id,username, message) {
    var activeUserId = receiverId; // get value from input field
    if (id === activeUserId) { // if message received belongs to the open chat

        var receivedMessageHtml = '<div class="row">' +
            '<div class="col-5 alert alert-secondary text-break" role="alert">' +
            message +
            '<p class="mb-0 text-end fs-6 fst-italic">@' +
            username +
            '</p>' +
            '</div> <div class="col-7"></div></div>';
        document.getElementById("messagesList").insertAdjacentHTML('beforeend', receivedMessageHtml); // append the message to the list
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
            var messageHtml = '<div class="row">' +
                '<div class="col-7"></div>' +
                '<div class="col-5 alert alert-primary text-break" role="alert">' +
                message +
                '<p class="mb-0 text-end fs-6 fst-italic">@' + "me" + '</p>' +
                '</div></div>';
            document.getElementById("messagesList").insertAdjacentHTML('beforeend', messageHtml); 

        })
        .catch(function (err) { // message not sent
        return console.error(err.toString());
        });

    
    event.preventDefault();
});