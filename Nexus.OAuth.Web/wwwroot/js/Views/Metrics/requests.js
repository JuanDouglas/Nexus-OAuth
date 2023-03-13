var cpuUsage, requestCount, lastPosition;
const graphResolution = 100;

$(document).ready(function (event) {
    const socket = new WebSocket('wss://localhost:44360/api/metrics/requests');

    // Configura o gráfico
    cpuUsage = new Chart('cpuUsage', {
        type: 'line',
        data: {
            labels: new Array(graphResolution).fill(''),
            datasets: [{
                label: 'Uso de CPU %',
                data: new Array(graphResolution).fill(0),
                backgroundColor: "rgba(151,187,205,0.2)",
                borderColor: "rgba(151,187,205,1)",
                fill: true
            }]
        },
        options: {
            responsive: true,
            interaction: {
                mode: 'index',
                intersect: false
            },
            scales: {
                x: {
                    display: true,
                    title: {
                        display: true
                    }
                },
                y: {
                    display: true,
                    min: 0,
                    max: 100,
                    title: {
                        display: true,
                        text: 'Percentage',

                    }
                }
            }
        }
    });
    requestCount = new Chart('requestCount', {
        type: 'line',
        data: {
            labels: new Array(graphResolution).fill(''),
            datasets: [{
                label: 'Requisições',
                data: new Array(graphResolution).fill(0),
                borderColor: 'rgb(255, 99, 132)',
                fill: false
            }]
        },
        options: {
            responsive: true,
            scales: {
                x: [{
                    min: 0
                }],
                y: [{
                }]
            }
        }
    });

    socket.addEventListener('message', newMessageOfMetrics);
});

// Atualiza o gráfico com os dados recebidos do WebSocket
function newMessageOfMetrics(event) {
    if (!(lastPosition < graphResolution)) {
        lastPosition = 0;

        cpuUsage.data.labels.fill('')
        cpuUsage.data.datasets[0].data.fill(0)
        cpuUsage.update();

        requestCount.data.labels.fill('')
        requestCount.data.datasets[0].data.fill(0)
        requestCount.update();
    }

    let data = JSON.parse(event.data);

    cpuUsage.data.labels[lastPosition] = moment($.now()).format("HH:mm:ss");
    cpuUsage.data.datasets[0].data[lastPosition] = data.CpuUsage;
    cpuUsage.update();

    requestCount.data.labels[lastPosition] = moment($.now()).format("HH:mm:ss");
    requestCount.data.datasets[0].data[lastPosition] = data.RequestPerSecond;
    requestCount.update();

    lastPosition++;
}