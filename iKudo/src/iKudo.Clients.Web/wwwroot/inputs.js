(function () {
    if (!String.prototype.trim) {
        (function () {
            var rtrim = /^[\s\uFEFF\xA0]+|[\s\uFEFF\xA0]+$/g;
            String.prototype.trim = function () {
                return this.replace(rtrim, '');
            };
        })();
    }

    [].slice.call(document.querySelectorAll('input.input-field')).forEach(function (inputEl) {
        if (inputEl.value.trim() !== '') {
            inputEl.parentNode.classList.add("input--filled");
        }

        inputEl.addEventListener('focus', onInputFocus);
        inputEl.addEventListener('blur', onInputBlur);
    });

    function onInputFocus(ev) {
        ev.target.parentNode.classList.add("input--filled");
    }

    function onInputBlur(ev) {
        if (ev.target.value.trim() === '') {
            ev.target.parentNode.classList.remove("input--filled");
        }
    }

    [].slice.call(document.querySelectorAll('.textarea-container > textarea')).forEach(function (element) {

        if (element.value.trim() !== '') {
            element.parentNode.classList.add('input--filled');
        }
        element.addEventListener('focus', onTextAreaFocus);
        element.addEventListener('blur', onTextAreaBlur);
    });

    function onTextAreaFocus(ev) {

        ev.target.parentNode.classList.add('line-up');
        ev.target.parentNode.classList.add('line-down');
    }

    function onTextAreaBlur(ev) {

        if (ev.target.value.trim() === '') {
            ev.target.parentNode.classList.remove('input--filled');
            ev.target.parentNode.classList.remove('line-up');
            ev.target.parentNode.classList.remove('line-down');
        }
        else {

            ev.target.parentNode.classList.add('input--filled');
        }
    }

})();
