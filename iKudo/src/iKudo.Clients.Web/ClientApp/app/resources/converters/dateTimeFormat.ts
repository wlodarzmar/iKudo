import * as moment from 'moment';

export class DateTimeFormatValueConverter {
    toView(value : any) {
        return moment(value).format('M/D/YYYY hh:mm');
    }
}