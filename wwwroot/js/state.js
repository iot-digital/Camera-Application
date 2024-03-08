document.addEventListener("DOMContentLoaded", function () {
    let connection = new signalR.HubConnectionBuilder()
        .withUrl("/hub/status")
        .build();

    connection.start()
        .then(() => {
            console.log("SignalR connected");

            connection.on("ReceiveMessage", (address, status) => {
                console.log("Received data from SignalR:", address);

                try {
                    let spanElement = document.getElementById(`d-${address}`);
                    if (spanElement) {
                        spanElement.innerText = status;

                        if (status === "ONLINE") {
                            spanElement.style.background = "green";
                        } else if (status === "OFFLINE") {
                            spanElement.style.background = "red";
                        }
                    }
                } catch (error) {
                    console.error("Error finding or updating the element:", error);
                }
            });
        })
        .catch((err) => {
            console.error("Error starting SignalR connection:", err.toString());
        });
});