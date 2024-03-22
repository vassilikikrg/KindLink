"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
var receiverId;
//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {

    $.ajax({
        url: '/Chat/RenderMessage',
        data: { user: user, message:message }
    }).done(function (result) {
        document.getElementById("messagesList").insertAdjacentHTML('beforeend', result);
    });
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessageToUser", receiverId ,message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});