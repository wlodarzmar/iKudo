export class InputsHelper {

    public Init() {

        $('input.input-field, .textarea-container > textarea').each((i, element) => {

            let value = (<HTMLInputElement>element).value;

            if (value.trim() !== '') {
                this.makeInputFilled(element);
            }

            element.addEventListener('focus', this.onInputFocus);
            element.addEventListener('blur', this.onInputBlur);
        });
    }

    onInputFocus = (e: FocusEvent) => {

        let element: Element = <Element>e.target;
        this.makeInputFilled(element);
    }

    private makeInputFilled(element: Element) {

        element.parentElement.classList.add('input--filled');

        if (element instanceof HTMLTextAreaElement) {
            element.parentElement.classList.add('line-up');
            element.parentElement.classList.add('line-down');
        }
    }

    private onInputBlur(e: Event) {

        let element: HTMLInputElement = (<HTMLInputElement>e.target);

        if (element.value.trim() === '') {
            element.parentElement.classList.remove('input--filled');
            element.parentElement.classList.remove('line-up');
            element.parentElement.classList.remove('line-down');
        }
        else {
            element.parentElement.classList.add('input--filled');
        }
    }
}