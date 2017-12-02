import { inject } from 'aurelia-framework';
import * as iziToast from 'izitoast';

@inject(iziToast)
export class Notifier {

    public info(message: string) {
        iziToast.default.show({ message: message, color: 'blue', position: 'topRight', icon: 'glyphicon glyphicon-info-sign' });
    }

    public success(message: string)  {
        iziToast.default.show({ message: message, color: 'green', position: 'topRight', icon: 'glyphicon glyphicon-ok-sign' });
    }

    public error(message: string) {
        iziToast.default.show({ message: message, color: 'red', position: 'topRight', icon: 'glyphicon glyphicon-remove-sign' });
    }
}