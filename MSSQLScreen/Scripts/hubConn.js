var memory;
var cpu;
$(function () {
    //Create the Hub    
    var chartHub = $.connection.chartHub;


    //Call InitChartData     
    $.connection.hub.start().done(function () {
        chartHub.server.initSaveData();
        chartHub.server.initGetData();
    });

    chartHub.client.getData = function (m, c) {
        updateMemory(m);
        updateCPU(c);
    };


});

function updateMemory(i) {
    memory = i.availableMemory;
}

function updateCPU(j) {
    cpu = j.cpuUsage;
}