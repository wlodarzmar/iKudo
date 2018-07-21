import { Chart, ChartConfiguration, ChartType } from 'chart.js';
import { I18N } from "aurelia-i18n";
import { inject } from "aurelia-framework";
import { KudoService } from "../../services/kudoService";
import { AuthService } from "../../services/authService";
import { List, Enumerable } from 'linqts';
import { Kudo } from "../../viewmodels/kudo";
import { EventAggregator } from "aurelia-event-aggregator";
import { AuthenticationChangedEventData } from "../../services/models/authentication-changed-event-data.model";

@inject(I18N, KudoService, AuthService, EventAggregator)
export class Dashboard {

    public showChart: boolean = false;
    private isAttached: boolean = false;

    constructor(
        private i18n: I18N,
        private kudoService: KudoService,
        private authService: AuthService,
        private eventAggregator: EventAggregator
    ) {
        this.eventAggregator.subscribe('authenticationChange', async (response: AuthenticationChangedEventData) => {

            this.showChart = response.isAuthenticated;
            if (this.isAttached) {
                this.loadChart();
            }
        });

    }

    activate() {
    }

    async attached() {

        this.isAttached = true;
        if (this.authService.isAuthenticated) {
            this.loadChart();
        }
    }

    private async loadChart() {

        let user = this.authService.getUser();
        if (!user) {
            this.showChart = false;
            return;
        }

        this.showChart = true;
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