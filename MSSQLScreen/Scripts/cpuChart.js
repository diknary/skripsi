
//////////////////////////////////////////////////////////////////////////

function initCPUChart() {

    var data = generateInitialData();

    // set up the updating of the chart each second
    var series = this.series[0];

    series.setData(data);

    setInterval(function () {
        var x = (new Date()).getTime() + 25200000, // current time
            y = cpu;
        series.addPoint([x, y], true, true);
    }, 1000);
}

function formatTooltip() {
    return '<b>' + this.series.name + '</b><br/>' +
        Highcharts.dateFormat('%Y-%m-%d %H:%M:%S', this.x) + '<br/>' +
        Highcharts.numberFormat(this.y, 2);
}

function generateInitialData() {

    // generate an array of random data
    var data = [],
        time = (new Date()).getTime() + 25200000,
        i;

    for (i = -19; i <= 0; i += 1) {
        data.push({
            x: time + i * 1000,
            y: 0
        });
    }
    return data;

}