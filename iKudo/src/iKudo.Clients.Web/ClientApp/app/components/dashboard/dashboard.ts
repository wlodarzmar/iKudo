import { Chart } from 'chart.js';

export class Dashboard {

    attached() {

        let data = {
            datasets: [{
                data: [10, 20, 30]
            }],

            // These labels appear in the legend and in the tooltips when hovering different arcs
            labels: [
                'Red',
                'Yellow',
                'Blue'
            ]
        };

        var ctx = document.getElementById("myChart") as HTMLCanvasElement;
        var myDoughnutChart = new Chart(ctx, {
            type: 'doughnut',
            data: data
            //options: options
        });
    }
}