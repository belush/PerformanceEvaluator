function showWebsite(id) {
    var result = $.ajax({
        type: "GET",
        url: "/Home/ShowWebsite",
        contentType: "application/json; charset=utf-8",
        data: { id: id },
        dataType: "json"
    });

    result.done(showResultTable);
    result.done(drawChart);
    result.fail(showGettingPagesErrorMsg);
}

function getWebsitesHistory() {
    var result = $.ajax({
        type: "GET",
        url: "/Home/GetWebsites",
        contentType: "application/json; charset=utf-8",
        dataType: "json"
    });

    result.done(showWebsiteHistoryTable);
    result.fail(showWebsiteErrorMsg);
}

function getProgress() {
    var result = $.ajax({
        type: "GET",
        url: "/Home/GetPagesProcessedNumber",
        contentType: "application/json; charset=utf-8",
        dataType: "json"
    });
    
    result.done(showProgressProcess);
    result.fail(showWebsiteErrorMsg);
}

function showProgressProcess(data) {
        showProgressBar();
        var curValue = Math.round(100 * data[0] / data[1]);
        var progressDiv = $("#progressContainer");
        progressDiv.css("width", curValue + "%");
        document.getElementById("progressValueSpan").innerHTML = curValue + "%";
}

function showWebsiteHistoryTable(data) {
    var tableHtml;

    if (data.length > 0) {
        tableHtml = "<tr>" +
                      "<th>Website URL</th>" +
                      "<th></th>" +
                  "</tr>";

        for (var i = 0; i < data.length; i++) {
            tableHtml += "<tr onclick=\"showWebsite(" + data[i].Id + ")\">" +
                            "<td>" + data[i].Url + "</td>" +
                            "<td style=\"color:grey\">click to show</td>" +
                          "</tr>";
        }

    } else {
        tableHtml = "<tr>" +
                      "<th>Website history is empty</th>" +
                      "<th></th>" +
                    "</tr>";
    }

    $("#resultWebsiteTable").html(tableHtml);
    $("#tableWebsiteContainer").show();
}

function showWebsiteErrorMsg() {
    var messageText = "Website history error";
    $("#infoMessage").html(messageText);
    $("#infoMessage").show();
}

function showGettingPagesErrorMsg() {
    var messageText = "Getting pages error";
    $("#infoMessage").html(messageText);
    $("#infoMessage").show();
}