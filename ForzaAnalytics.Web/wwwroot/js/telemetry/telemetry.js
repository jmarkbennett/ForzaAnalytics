mousePressed = false;
mouseDragged = false;
mousePressedInitialX = null;
mousePressedInitialY = null;
offsetX = 0;
offsetY = 0;
canvasId = "mapCanvas";
currentMap = null;
currentTelemetry = null;
currentMapScale = 1.0;
currentShowLaps = -1; // -1 = ALL
replayInterval = null;
replayGap = 1.67 // default
isRotated = false;
isReplaying = false;
currentOrdinal = 0;
currentVisualType = "default";
heatmapColours = [
        "FF0000",
        "FF8C00",
        "FFA500",
        "DAA520",
        "FFFF00",
        "9ACD32",
        "ADFF2F",
        "7FFF00",
        "32CD32",
        "008000"
];
gearShades = [
        "000080",
        "0000CD",
        "0000FF",
        "1E90FF",
        "4169E1",
        "6495ED",
        "87CEEB",
        "87CEFA",
        "B0E0E6",
        "F0F8FF"
];
accelerationColours =
        [
            "006400",
            "228B22",
            "008000",
            "00FF00",
            "00FF7F"
        ];
brakeColours = [
        "8B0000",
        "B22222",
        "FF0000",
        "DC143C",
        "CD5C5C"
    ];
baseAccelerationColor = "00AAFF";
baseBrakeColour = "FF0000";
baseGearNumberColour = "00FF00";
baseCoastingColour = "FFFF00";
baseSpeedColour = "FFFFFF";
/* 100%  90%   80%   70%   60%   50%   40%   30%   20 or 10 (Only used with Gear Number) */
percentColourGrades = ["FF", "DD", "BB", "99", "77", "55", "33", "11", "00"];
function ClearMap(canvas) {
    context = canvas.getContext('2d');
    context.clearRect(0, 0, canvas.width, canvas.height);
}
function HandleMapScaleChange(event) {
    canvas = document.getElementById(canvasId);
    currentMapScale = event.currentTarget.value;
    ClearMap(canvas);
    ReloadData();
}
function HandleMapOrientationChange(event) {
    isRotated = event.srcElement.checked;
    ReloadData();
}
function HandleShowLapsChange(event) {
    var lapsToShow = event.currentTarget.value;
    ClearMap(canvas);
    if (lapsToShow == "All")
        lapsToShow = -1;
    currentShowLaps = lapsToShow;
    ReloadData();
}
function HandleVisualTypeChange(event) {
    currentVisualType = event.currentTarget.value;
    ReloadData();
}
function HandleStopReplayClick(event) {
    clearInterval(replayInterval);
    isReplaying = false;
    currentOrdinal = 0;
    document.getElementById("replay").removeAttribute("disabled");
    document.getElementById("stop").setAttribute("disabled", "true");
}
function formatTime(seconds) {
    // Calculate hours, minutes, and remaining seconds
    var hours = Math.floor(seconds / 3600);
    var minutes = Math.floor((seconds % 3600) / 60);
    var remainingSeconds = seconds % 60;

    // Format the time string
    var formattedTime = "";

    if (hours >= 10)
        formattedTime += hours + ":";
    else if (hours > 0)
        formattedTime += "0" + hours + ":";
    else
        formattedTime += "00:"

    if (minutes >= 10) {
        formattedTime += minutes + ":";
    }
    else if (minutes > 0) {
        formattedTime += "0" + minutes + ":";
    }
    else {
        formattedTime += "00:"
    }

    if (remainingSeconds >= 10)
        formattedTime += remainingSeconds.toFixed(3);
    else
        formattedTime += "0" + remainingSeconds.toFixed(3);

    return formattedTime;
}
function HandleReplayClick(event) {
    var canvase = document.getElementById(canvasId);
    ClearMap(canvas);
    LoadMap();
    var maxSpeed = 0;
    if (currentTelemetry != null) {
        isReplaying = true;
        var lapNumber = document.getElementById("lapNumber");
        var lapTime = document.getElementById("lapTime");
        var raceTime = document.getElementById("raceTime");
        var speed = document.getElementById("speed");
        var gear = document.getElementById("gear");
        var acceleration = document.getElementById("acceleration");
        var brake = document.getElementById("brake");
        var clutch = document.getElementById("clutch");
        var handbrake = document.getElementById("handbrake");
        var fuel = document.getElementById("fuel");
        document.getElementById("replay").setAttribute("disabled", "true");
        document.getElementById("stop").removeAttribute("disabled");


        replayInterval = window.setInterval(function () {
            if (currentOrdinal < currentTelemetry.length) {

                if (currentTelemetry[currentOrdinal].speed_Mps > maxSpeed) {
                    maxSpeed = currentTelemetry[currentOrdinal].speed_Mps;
                }
                var prevSpeed = 0
                if (currentOrdinal > 10)
                    prevSpeed = currentTelemetry[currentOrdinal - 10].speed_Mps;

                if (currentShowLaps == -1 || currentTelemetry[currentOrdinal].lapNumber == currentShowLaps) {
                    AddTelemetryPoint(canvas, currentTelemetry[currentOrdinal], maxSpeed, prevSpeed);
                    if (currentOrdinal > 0 && currentTelemetry[currentOrdinal].gearNumber != currentTelemetry[currentOrdinal - 1].gearNumber)
                        AddGearNumberLabel(canvas, currentTelemetry[currentOrdinal].gearNumber, currentTelemetry[currentOrdinal].x, currentTelemetry[currentOrdinal].z);

                    lapNumber.innerHTML = currentTelemetry[currentOrdinal].lapNumber;
                    lapTime.innerHTML = formatTime(currentTelemetry[currentOrdinal].lapTime);
                    raceTime.innerHTML = formatTime(currentTelemetry[currentOrdinal].raceTime);
                    speed.innerHTML = currentTelemetry[currentOrdinal].speed_Mph + " mph";
                    gear.innerHTML = currentTelemetry[currentOrdinal].gearNumber;
                    acceleration.innerHTML = currentTelemetry[currentOrdinal].acceleration.toFixed(2) + "%";
                    brake.innerHTML = currentTelemetry[currentOrdinal].brake.toFixed(2) + "%";
                    clutch.innerHTML = currentTelemetry[currentOrdinal].clutch.toFixed(2) + "%";
                    handbrake.innerHTML = currentTelemetry[currentOrdinal].handbrake.toFixed(2) + "%";
                    fuel.innerHTML = (currentTelemetry[currentOrdinal].fuelRemaining * 100).toFixed(2) + "%";
                    currentOrdinal = currentOrdinal + 1;

                }
            }
            else {
                clearInterval(replayInterval);
                isReplaying = false;
                currentOrdinal = 0;
                document.getElementById("replay").removeAttribute("disabled");
                document.getElementById("stop").setAttribute("disabled", "true");
            }
        }, replayGap);
    }
}
function GetAdjustedX(x) {
    return (x + offsetX) * currentMapScale;
}
function GetAdjustedY(y) {
    return (y + offsetY) * currentMapScale;
}
function AddMapPoint(canvas, x, y) {
    context = canvas.getContext('2d')
    context.fillStyle = "#DEDEDE";
    context.fillRect(
        isRotated ? GetAdjustedY(y) : GetAdjustedX(x),
        isRotated ? GetAdjustedX(x) : GetAdjustedY(y), 1, 1);
}
function GetTelemetryColour(position, maxSpeed, prevSpeed) {
    switch (currentVisualType) {
        case "default":
            return "000000";
        case "pedalpressure":
            if (position.acceleration > 90)
                return accelerationColours[4];
            else if (position.acceleration > 80)
                return accelerationColours[3];
            else if (position.acceleration > 60)
                return accelerationColours[2];
            else if (position.acceleration > 40)
                return accelerationColours[1];
            else if (position.acceleration > 20)
                return accelerationColours[0];
            if (position.brake > 90)
                return brakeColours[4];
            else if (position.brake > 80)
                return brakeColours[3];
            else if (position.brake > 60)
                return brakeColours[2];
            else if (position.brake > 40)
                return brakeColours[1];
            else if (position.brake > 20)
                return brakeColours[0];
            else if (position.acceleration == 0 && position.brake == 0)
                return baseCoastingColour;
            break;
        case "gearnumber":
            if (position.gearNumber == "R")
                return "FF0000";
            else if (position.gearNumber != "N")
                return gearShades[position.gearNumber];
            break;
        case "speedheatmap":
            if (position.speed_Mps > (maxSpeed * 0.9))
                return heatmapColours[9];
            else if (position.speed_Mps > (maxSpeed * 0.8))
                return heatmapColours[8];
            else if (position.speed_Mps > (maxSpeed * 0.7))
                return heatmapColours[7];
            else if (position.speed_Mps > (maxSpeed * 0.6))
                return heatmapColours[6];
            else if (position.speed_Mps > (maxSpeed * 0.5))
                return heatmapColours[5];
            else if (position.speed_Mps > (maxSpeed * 0.4))
                return heatmapColours[4];
            else if (position.speed_Mps > (maxSpeed * 0.3))
                return heatmapColours[3];
            else if (position.speed_Mps > (maxSpeed * 0.2))
                return heatmapColours[2];
            else if (position.speed_Mps > (maxSpeed * 0.1))
                return heatmapColours[1];
            break;
        case "acceleration":
            if (prevSpeed == null)
                return "0000FF";
            else if (position.speed_Mps.toFixed(3) > prevSpeed.toFixed(3)) // Accelerating
                return "00FF00";
            else if (position.speed_Mps.toFixed(3) == prevSpeed.toFixed(3)) // Maintaining
            {
                return "0000FF";
            }
            else if (position.speed_Mps.toFixed(3) < prevSpeed.toFixed(3)) // Slowing
                return "FF0000";
            break;
        default:
            return "000000";
    }
    return "000000";
}
function AddTelemetryPoint(canvas, position, maxSpeed, prevSpeed) {
    context = canvas.getContext('2d')
    context.fillStyle = "#" + GetTelemetryColour(position, maxSpeed, prevSpeed);
    context.fillRect(
        isRotated ? GetAdjustedY(position.z) : GetAdjustedX(position.x),
        isRotated ? GetAdjustedX(position.x) : GetAdjustedY(position.z), 1, 1);
}
function AddGearNumberLabel(canvas, gearNumber,x, y,) {
    context = canvas.getContext('2d')
    context.font = "10px Arial";
    context.fillText(gearNumber,
        isRotated ? GetAdjustedY(y) : GetAdjustedX(x),
        isRotated ? GetAdjustedX(x) : GetAdjustedY(y)
    );
}
function SetDefaultOffsets(payload) {
    if (offsetX == 0 && offsetY == 0) {
        for (var i = 0; i < payload.length; i++) {
            if (payload[i].x < offsetX)
                offsetX = payload[i].x
            if (payload[i].z < offsetY)
                offsetY = payload[i].z
        }

        offsetX = Math.abs(offsetX);
        offsetY = Math.abs(offsetY);
    }
}
function ResizeCanvas(){
    canvas = document.getElementById(canvasId);
    if (currentMap != null) {
        var minX = null;
        var maxX = null;
        var minY = null;
        var maxY = null;
        for (var i = 0; i < currentMap.length; i++) {
            var x = GetAdjustedX(currentMap[i].x);
            var y = GetAdjustedY(currentMap[i].z);
            if (minX == null || minX > x)
                minX = x;
            if (maxX == null || maxX < x)
                maxX = x;
            if (minY == null || minY > y)
                minY = y;
            if (maxY == null || maxY < y)
                maxY = y;
        }

        var w = maxX - minX;
        var h = maxY - minY;
        canvas.setAttribute("width",  isRotated ? Math.round(h + 1) : Math.round(w + 1));
        canvas.setAttribute("height", isRotated ? Math.round(w + 1) : Math.round(h + 1));
    }
}
function AddLapsToFilter() {
    showLaps = document.getElementById("showLaps");
    laps = [];
    currentLap = null;
    for (var i = 0; i < currentTelemetry.length; i++) {
        if (currentLap == null) {
            currentLap = currentTelemetry[i].lapNumber;
            laps.push(currentLap);
        }
        if (currentLap != currentTelemetry[i].lapNumber) {
            currentLap = currentTelemetry[i].lapNumber;
            laps.push(currentLap);
        }
    }
    // remove all options except the 'ALL', reverse order to preserve ordinals
    for (var i = showLaps.options.length - 1; i > 0; i--) {
        showLaps.remove(i);
    }

    for (var i = 0; i < laps.length; i++) {
        var item = document.createElement('option');
        item.value = laps[i];
        item.innerHTML = laps[i];
        showLaps.appendChild(item);
    }
}
function LoadTelemetry(payload) {

    if (payload != null)
        currentTelemetry = payload;
    else
        payload = currentTelemetry;

    var maxSpeed = 0;

    SetDefaultOffsets(payload);
    AddLapsToFilter();
    canvas = document.getElementById(canvasId);
    for (var i = 0; i < payload.length; i++) {
        if ((isReplaying && i < currentOrdinal) || !isReplaying) {
            if (currentShowLaps == -1 || payload[i].lapNumber == currentShowLaps) {
                if (payload[i].speed_Mps > maxSpeed) {
                    maxSpeed = payload[i].speed_Mps;
                }
                var prevSpeed = 0
                if (i > 10)
                    prevSpeed = payload[i - 10].speed_Mps;

                AddTelemetryPoint(canvas, payload[i], maxSpeed, prevSpeed);
                if (i > 0 && payload[i].gearNumber != payload[i - 1].gearNumber)
                    AddGearNumberLabel(canvas, payload[i].gearNumber, payload[i].x, payload[i].z);
            }
        }
    }
    if (!isReplaying) {
        if (payload.length > 0) {
            replayGap  = (payload[1].lapTime - payload[0].lapTime) * 100
        }
    }

}
function LoadMap(payload) {
    if (payload != null)
        currentMap = payload;
    else
        payload = currentMap;
    SetDefaultOffsets(payload);

    canvas = document.getElementById(canvasId);
    ResizeCanvas();
    for (var i = 0; i < payload.length; i++) {
        AddMapPoint(canvas, payload[i].x, payload[i].z);
    }
}
function ReloadData() { 
    LoadMap(currentMap);
    LoadTelemetry(currentTelemetry);
}