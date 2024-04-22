function ClearLapTimeTable() {
    $("#lapTimesTable:not(:first)").remove();
}

function LoadLapTimes(lapTimes) {
    ClearLapTimeTable();
    var bestLapTime = 0
    for (var i = 0; i < lapTimes.length; i++) {
        if (lapTimes[i].isBestLap) {
            debugger;
            bestLapTime = lapTimes[i].timeInSeconds;
        }
    }


    for (var i = 0; i < lapTimes.length; i++) {
        var row = "<tr>";
        var diff = ""
        if (lapTimes[i].isBestLap) {
            row = "<tr class='table-primary'>";
            diff = ""
        }
        else {
            var diff = "+ " + (lapTimes[i].timeInSeconds - bestLapTime).toFixed(3);
        }
        row += "<th scope='row'>"  + lapTimes[i].lapNumber  + "</th>";
        row += "<td>" + lapTimes[i].formattedLapTime + "</td>";
        row += "<td>" + lapTimes[i].formattedFuelRemaining + "</td>";
        row += "<td>" + lapTimes[i].formattedAverageSpeed + "</td>";         
        row += "<td>" + lapTimes[i].formattedPercentFullThrottle + "</td>";    
        row += "<td>" + lapTimes[i].formattedPercentBrakeApplied + "</td>";    
        row += "<td>" + lapTimes[i].formattedMinSpeed + "</td>";
        row += "<td>" + lapTimes[i].formattedMaxSpeed + "</td>";   
        row += "<td>" + lapTimes[i].formattedFuelUsed + "</td>";
        row += "<td style='color:red'>" + diff + "</td>";
        row += "</tr>"

        $("#lapTimesTable").append(row);
    }
} 