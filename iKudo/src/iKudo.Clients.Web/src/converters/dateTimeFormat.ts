let moment = require('moment');

export class DateTimeFormatValueConverter {
    toView(value) {
        return moment(value).format('M/D/YYYY hh:mm');
    }
}