import { Chart, ChartConfiguration, ChartType } from 'chart.js';
import { I18N } from "aurelia-i18n";
import { inject } from "aurelia-framework";
import { KudoService } from "../../services/kudoService";
import { AuthService } from "../../services/authService";
import { List, Enumerable } from 'linqts';
import { Kudo } from "../../viewmodels/kudo";

@inject(I18N, KudoService, AuthService)
export class Dashboard {

    private kudosReceived: number = 0;
    private kudosGiven: number = 0;


    constructor(
        private i18n: I18N,
        private kudoService: KudoService,
        private authService: AuthService
    ) { }


    async attached() {

        let user = this.authService.getUser();
        if (!user) {
            return;
        }

        let sent = (await this.kudoService.getSentByUser(user.id)).length;
        let given = (await this.kudoService.getReceivedByUser(user.id)).length;

        var ctx = document.getElementById("myChart") as HTMLCanvasElement;
        let config = this.getKudoRationChartConfig(given, sent);
        new Chart(ctx, config);
    }

    private getKudoRationChartConfig(given: number, received: number) {

        let config: ChartConfiguration = {
            type: 'doughnut',
            data: {
                datasets: [{
                    data: [given, received],
                    backgroundColor: [
                        '#2ecc71',
                        '#3498db'
                    ],
                }],
                labels: [
                    this.i18n.tr('kudo.received'),
                    this.i18n.tr('kudo.sent')
                ]
            },
            options: {
                responsive: true,
                legend: {
                    position: 'top',
                },
                title: {
                    display: true,
                    text: this.i18n.tr('dashboard.kudo_ratio')
                },
                animation: {
                    duration: 1000,
                }
            }
        };

        return config;
    }
}