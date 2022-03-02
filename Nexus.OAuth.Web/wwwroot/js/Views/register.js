$(document).ready(function () {
    loadInputs();
});

function next() {
    var rst = validFields([
        '#Name',
        '#Email',
        '#PhoneNumber'
    ]);

    if (rst == false) {
        return;
    }

    hide('first-step');
    show('second-step');

    var button = $('.step #register');
    button.html('Create');
    button.attr('onclick', 'register()');
}

function register() {
    $('#formRegister').submit();
}