function ClearLapTimeTable() {
    $("#lapTimesTable:not(:first)").remove();
}

function LoadLapTimes(lapTimes) {
    debugger;
    ClearLapTimeTable();
    for (var i = 0; i < lapTimes.length; i++) {
        var row = "<tr>";
        row += "<td>"  + lapTimes[i].lapNumber  + "</td>";
        row += "<td>" + lapTimes[i].formattedLapTime + "</td>";
        row += "<td>" + lapTimes[i].formattedFuelRemaining + "</td>";
        row += "<td>" + lapTimes[i].formattedAverageSpeed + "</td>";         
        row += "<td>" + lapTimes[i].formattedPercentFullThrottle + "</td>";    
        row += "<td>" + lapTimes[i].formattedPercentBrakeApplied + "</td>";    
        row += "<td>" + lapTimes[i].formattedMinSpeed + "</td>";
        row += "<td>" + lapTimes[i].formattedMaxSpeed + "</td>";   
        row += "<td>" + lapTimes[i].formattedFuelUsed + "</td>";
        row += "<td>" + lapTimes[i].isBestLap + "</td>";
        row += "</tr>"

        $("#lapTimesTable").append(row);
    }
}
