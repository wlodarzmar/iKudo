import { inject } from 'aurelia-framework';
import * as iziToast from 'izitoast';

export class Notifier {

    public info(message: string) {
        (iziToast as any).show({ message: message, color: 'blue', position: 'topRight', icon: 'glyphicon glyphicon-info-sign' });
    }

    public success(message: string)  {
        (iziToast as any).show({ message: message, color: 'green', position: 'topRight', icon: 'glyphicon glyphicon-ok-sign' });
    }

    public error(message: string) {
        (iziToast as any).show({ message: message, color: 'red', position: 'topRight', icon: 'glyphicon glyphicon-remove-sign' });
    }
}