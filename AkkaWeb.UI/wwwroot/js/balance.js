"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/BalanceHub").build();

// Listen for customer balance updates
connection.on("UpdateBalance", snapshot =>
{
    let balance = document.getElementById("CustomerBalance" + snapshot.customerId);
    if (balance)
    {
        balance.innerText = snapshot.balance.toFixed(2).toString();
    }
});

connection.start().then(function ()
{
    console.log("SignalR balance updates connected");
}).catch(function (err)
{
    return console.error(err.toString());
});