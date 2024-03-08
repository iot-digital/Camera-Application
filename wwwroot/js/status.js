// Array to store unique camera IDs for each network address
let uniqueCameraIds = {};

// Mapping for camera IDs to image paths for each network address
let cameraImagePaths = {};

let connection = new signalR.HubConnectionBuilder()
    .withUrl("/hub/state")
    .build();

connection.start()
    .then(() => {
        console.log("SignalR connected");

        connection.on("ReceiveMessage", (message, imgPaths) => {
            console.log("Received data from SignalR:", message);

            try {
                let data = typeof message === 'string' ? JSON.parse(message) : message;
                let msgTypes = data.msg_content.split(":");

                if (msgTypes[0] === 'm') {
                    handleObjectMessage(data, imgPaths);
                } else if (msgTypes[0] === 'h') {
                    handleHeartbeatMessage(data,);
                }
            } catch (error) {
                console.error("Error parsing JSON or extracting values:", error);
            }
        });
    })
    .catch((err) => {
        console.error("Error starting SignalR connection:", err.toString());
    });

function handleObjectMessage(data, imgPaths) {
    let msgContent = data.msg_content.slice(2);
    let zoneDataArray = msgContent.split('|');

    // Extract and log the address
    let receivedAddress = `${data.network_add}.${data.node_add}`;

    // Check if the received address matches any address in the HTML model
    let dashboardElement = document.getElementById(`dashboard${receivedAddress}`);

    // Initialize uniqueCameraIds array for the current network address if not present
    if (!uniqueCameraIds[receivedAddress]) {
        uniqueCameraIds[receivedAddress] = [];
    }

    let cameraIds = zoneDataArray.map(zoneData => zoneData.split(',')[0]);

    // Clear data for cameras that are not present in the new data for the same network
    uniqueCameraIds[receivedAddress].forEach(existingCameraId => {
        if (!cameraIds.includes(existingCameraId)) {
            clearTableForCamera(receivedAddress, existingCameraId);
        }
    });

    // Update uniqueCameraIds array for the current network address
    uniqueCameraIds[receivedAddress] = cameraIds;

    // Initialize cameraImagePaths array for the current network address if not present
    if (!cameraImagePaths[receivedAddress]) {
        cameraImagePaths[receivedAddress] = {};
    }

    imgPaths.forEach(imgPath => {
        Object.keys(imgPath).forEach(cameraId => {
            cameraImagePaths[receivedAddress][cameraId] = imgPath[cameraId];
        });
    });

    // Check if received address exists in the model
    if (dashboardElement !== null) {
        // Clear old data and update camera table
        clearAndRefreshCameraTable(receivedAddress, zoneDataArray, data);
    } else {
        console.warn(`Received address ${receivedAddress} and id does not match Error.`);
    }
}

function clearAndRefreshCameraTable(receivedAddress, zoneDataArray, data) {
    // Clear data for the entire network
    clearDataForNetwork(receivedAddress);

    // Update camera table with new data
    updateCameraTable(zoneDataArray, data, receivedAddress);
}

function clearDataForNetwork(receivedAddress) {
    // Clear data for all cameras in the network
    uniqueCameraIds[receivedAddress].forEach(cameraId => {
        clearTableForCamera(receivedAddress, cameraId);
    });
}

function updateCameraTable(zoneDataArray, data, receivedAddress) {
    zoneDataArray.forEach((zoneData) => {
        let values = zoneData.split(',');

        let cameraId = values[0];
        let zoneId = values[1];
        let peopleCount = values[2];
        let carCount = values[3];
        let truckCount = values[4];
        let motorcycleCount = values[5];
        let miscCount = values[6];

        if (cameraId === null || cameraId === undefined) {
            return;
        }

        let formattedZoneId = `Zone${zoneId.replace('|', '_')}`;

        let tableCell = $(`#tableZone${cameraId} [data-zone="${formattedZoneId}"]`);

        tableCell.find('.people').text(` ${peopleCount}`);
        tableCell.find('.car').text(` ${carCount}`);
        tableCell.find('.truck').text(` ${truckCount}`);
        tableCell.find('.bike').text(` ${motorcycleCount}`);
        tableCell.find('.misc').text(` ${miscCount}`);

        // Display image paths for each camera
        if (cameraImagePaths[receivedAddress] && cameraImagePaths[receivedAddress][cameraId]) {
            $(`#ImageCard${cameraId} .image-card`).attr('src', cameraImagePaths[receivedAddress][cameraId]);
        }
    });
}

function handleHeartbeatMessage(data) {
    // TODO: get the element of ONLINE-OFFLINE and change the text when heartbeat is received
    let spanElement = document.getElementById(`d-${data.network_add}.${data.node_add}`);
}

function clearTableForCamera(receivedAddress, cameraId) {
    // Clear data for the specified camera ID
    $(`#tableZone${cameraId} [data-zone] td:not(:first-child)`).text('-');
}









//"use strict";

//const MILLI_SECONDS_TO_REFRESH = 65_000;

//let timeOut = setTimeout(refresh, MILLI_SECONDS_TO_REFRESH); //refresh after 1 min 5 secs
//let uniqueCameraIds = [];

//let connection = new signalR.HubConnectionBuilder()
//    .withUrl("/hub/status")
//    .build();

//connection.on("ReceiveMessage", function (message) {
//    console.log('**********signalR msg reached****************');
//    console.log(message);

//    let lotIdFormServer = parseInt(message);
//    let currentLotId = parseInt(document.getElementById('lotId').value);

//    if (lotIdFormServer === currentLotId) {
//        refresh();
//        clearTimeout(timeOut);
//        timeOut = setTimeout(refresh, MILLI_SECONDS_TO_REFRESH);
//    }
//});

//connection.start().then(function () {
//    console.log('********signalR started*************');
//}).catch(function (err) {
//    return console.error(err.toString());
//});

//connection.onclose(reconnect);
//function reconnect() {
//    connection.start().then(function () {
//        console.log('********signalR reconnected*************');
//    }).catch(function (err) {
//        return console.error(err.toString());
//    });
//}

//function refresh() {
//    console.log('**********signalR refreshing****************');
//    connection.connection.stop();
//    location = window.location.href;
//}