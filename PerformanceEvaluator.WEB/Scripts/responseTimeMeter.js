function measureResponseTime() {
    //hideInfoMessagehideInfoMessage();
    $("#helpContainer").hide();
    var urlText = $("#urlText").val();
    var result = urlText.match("^(https?://)(www\\.)?([-a-z0-9]{1,63}\\.)*?[a-z0-9][-a-z0-9]{0,61}[a-z0-9]\\.[a-z]{2,6}(/[-\\w@\\+\\.~#\\?&/=%]*)?$");
    
    if (result == null) {
        showCheckUrlInfoMessage();
    } else {
        getResponseTime(urlText);
        runProgressProccess(true);
    }
}

function getResponseTime(urlText) {
    var result = $.ajax({
        type: "GET",
        url: "/Home/GetTime",
        contentType: "application/json; charset=utf-8",
        data: { urlText: urlText },
        dataType: "json"
    });
    result.done(showResultTable);
    result.done(drawChart);
    result.done(getWebsitesHistory);
   
    result.fail(showGettingResponseTimeErrorMsg);
}

function hideProgressBar() {
    runProgressProccess(false);
    $("#outerProgressContainer").hide();
}

function showProgressBar() {
    $("#outerProgressContainer").show();
}

var refreshIntervalId;

function runProgressProccess(isRun) {
    if (isRun) {
        refreshIntervalId = window.setInterval(getProgress, 500);
    } else {
        window.clearInterval(refreshIntervalId);
    }
}

function drawChart(data) {

    hideProgressBar();

    var getLocation = function(href) {
        var location = document.createElement("a");
        location.href = href;
        return location;
    };

    var midTimeArray = data.map(function (item) {
        return item.MidResponseTime;
    });

    var minTimeArray = data.map(function (item) {
        return item.MinResponseTime;
    });

    var maxTimeArray = data.map(function (item) {
        return item.MaxResponseTime;
    });

    var urlArray = data.map(function(item) {
        var location = getLocation(item.Url);
        return location.pathname;
    });

    var siteDomainMin = "Min";
    var siteDomainMax = "Max";
    var siteDomainMid = "Mid";
    var siteDomain = getLocation(data[0].Url).hostname;
    
    $("#chartContainer").show();

    $(function() {
        $('#chartContainer').highcharts({
            title: {
                text: siteDomain,
                x: -20 //center
            },
            subtitle: {
                text: '',
                x: -20
            },
            xAxis: {
                categories: urlArray
            },
            yAxis: {
                title: {
                    text: ' Response time (ms)'
                },
                plotLines: [
                    {
                        value: 0,
                        width: 1,
                        color: '#808080'
                    }
                ]
            },
            tooltip: {
                valueSuffix: ' ms'
            },
            legend: {
                layout: 'vertical',
                align: 'right',
                verticalAlign: 'middle',
                borderWidth: 0
            },
            series: [
                {
                    name: siteDomainMid,
                    data: midTimeArray
                },
                {
                    name: siteDomainMax,
                    data: maxTimeArray
                },
                {
                    name: siteDomainMin,
                    data: minTimeArray
                }
            ]
        });
    });
}
//

function showResultTable(data) {

    if (data.length > 0) {
        var tableHtml = "<tr>" +
                            "<th colspan=\"3\">Response time (ms)</th>" +
                            "<th rowspan=\"2\">Page URL</th>" +
                        "</tr>"+
                        "<tr>" +
                            "<th>Min</th>" +
                            "<th>Mid</th>" +
                            "<th>Max</th>" +
                            //"<th>Page URL</th>" +
                        "</tr>";

        for (var i = 0; i < data.length; i++) {
            tableHtml += "<tr>" +
                            "<td>" + data[i].MinResponseTime + "</td>" +
                            "<td>" + data[i].MidResponseTime + "</td>" +
                            "<td>" + data[i].MaxResponseTime + "</td>" +
                            "<td>" + data[i].Url + "</td>" +
                          "</tr>";
        }

        $("#resultTable").html(tableHtml);
        $("#tableContainer").show();
    } else {
        showCheckUrlInfoMessage();
    }
}

function hideInfoMessage() {
    $("#infoMessage").hide();
} 

function showCheckUrlInfoMessage() {
    var tableHtmlCopy = "Please check the URL";
    $("#infoMessage").html(tableHtmlCopy);
    $("#infoMessage").show();
}

function showGettingResponseTimeErrorMsg() {
    var messageText = "Getting response time error";
    $("#infoMessage").html(messageText);
    $("#infoMessage").show();
}