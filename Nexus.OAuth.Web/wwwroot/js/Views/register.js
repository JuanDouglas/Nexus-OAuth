function register() {
    var form = document.getElementById('formRegister');
}

function next() {
    var rst = validFields([
        '#Name',
        '#Email',
        '#PhoneNumber'
    ]);

    if (rst == false) {
        return;
    }

    $('.first-step').addClass('hidden');
    $('.second-step').removeClass('hidden');

    var button = $('.step #register');
    button.html('Create');
    button.attr('onclick', 'register()');
}

function register() {
    $('#formRegister').submit();
}

$(document).ready(function () {
    loadInputs();
});