$(document).ready(function () {
    loadInputs();

    $('#formRegister')
        .submit(submitRegister)
        .find('.next-button')
        .on('click', next);

    urlBack = $('#formRegister')
        .data('redirect');
});

const firstStep = [
    '#Name',
    '#Email',
    '#Phone',
    '#DateOfBirth'
];

const toggleLogoTime = 2500;

var tid = setTimeout(toggleLogo, toggleLogoTime);

function toggleLogo() {
    $('.logo')
        .toggleClass('hovered');

    tid = setTimeout(toggleLogo, toggleLogoTime);
}

function getRegister() {
    return {
        Email: $('#Email').val(),
        Name: $('#Name').val(),
        Phone: $('#Phone').val(),
        Password: $('#Password').val(),
        ConfirmPassword: $('#ConfirmPassword').val(),
        Culture: $('#Culture').val(),
        DateOfBirth: $('#DateOfBirth').val(),
        AcceptTerms: new Boolean($('#AcceptTerms').val())
    }
}

function next() {
    hide('.first-step');
    show('.second-step');

    var button = $('.step #register');
    button.html('Create');
    button.off('click', next)
    button.on('click', register);
}

function back() {
    show('.first-step');
    hide('.second-step');

    var button = $('.step #register');
    button.html('Next <span class="fa fa-lg fa-arrow-right"></span>');
    button.off('click', register)
    button.on('click', next);
}

function register() {
    $('#formRegister').submit();
}

function submitRegister(event) {
    event.preventDefault();
    let account = getRegister();

    $.post({
        url: '',
        data: account,
        dataType: 'json',
        type: 'json',
        method: 'POST',
        success: function (data) {
            if (data.valid) {
                $.ajax({
                    method: 'PUT',
                    url: apiHost + 'Account/Register',
                    contentType: 'application/json',
                    type: 'json',
                    data: JSON.stringify(account),
                    error: function (data) {
                        let errors = data.responseJSON.errors;
                        checkRegisterErrors(errors);
                    },
                    success: function () {
                        redirectTo(urlBack);
                    }
                })
            }
        },
        error: function (data) {
            var errors = data.responseJSON;
            checkRegisterErrors(errors);
        }
    });
}

function checkRegisterErrors(errors) {
    showErrors(errors);

    var first = Object.keys(errors)
        .find(f => firstStep.find(finder => finder == '#' + f) != undefined) != undefined;

    if (first) {
        back();
    }
}

function toLogin() {
    redirectAndReturn('/Authentication', false, $('#formRegister')
        .data('redirect'));
}

function registerOk() {
    let account = getRegister();
    login(account.Email, account.Password, $('#formRegister').data('redirect'));
}