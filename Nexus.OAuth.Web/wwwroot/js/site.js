// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const apiHost = 'https://localhost:44360/api/';
/* const apiHost = 'https://nexus-oauth.duckdns.org/api/'; */// -->  publish site url

function getAccount(redirect) {
    var url = apiHost + 'Accounts/MyAccount';

    var xhr = new XMLHttpRequest();
    xhr.open('GET', url);
    xhr.origin = origin;
    xhr.withCredentials = true;
    xhr.responseType = 'json';

    xhr.onload = function () {
        var status = xhr.status;

        if (status == 401 && redirect) {

        }

        if (status == 200) {
            return xhr.response;
        }
    }

    xhr.send();
}

function openLoader() {
    hide('component');
    show('loader');
}

function closeLoader() {
    hide('loader');
    show('component');
}

function show(id) {
    var element = document.getElementById(id);
    element.classList.remove('invisible');
}

function hide(id) {
    var element = document.getElementById(id);
    element.classList.add('invisible');
}

function redirectTo(url) {
    $('#redirectModal').modal('show', { backdrop: 'static', keyboard: false });
    setTimeout(function () {
        window.location = url;
        document.title = 'Redirecting...';
    }, 3500);
}

function redirectLogin() {
    redirectTo('../Authentication?redirect=' + encodeURIComponent(window.location));
}

function removeError(id) {
    console.log(id)
    $('.form-group').each((e, obj) => {
        var input = $(obj).find('input');

        if (input[0] != undefined) {
            if (input[0].id == id) {
                input.removeClass('input-validation-error')

                var label = $(obj).find('span');
                label.html('');
            }
        }
    })
}

function addError(id, error) {
    console.log(id)
    $('.form-group').each((e, obj) => {
        var input = $(obj).find('input');

        if (input[0] != undefined) {
            if (input[0].id == id) {
                input.addClass('input-validation-error');

                var label = $(obj).find('span');
                console.log(label);
                label.addClass('field-validation-error');
                label.html(error);
            }
        }
    })
}

function loadInputs() {
    $('.form-group').each((e, obj) => {
        var input = $(obj).find('input');

        input.on('click', function () {
            removeError(this.id)
        })

        if (input.type == "phone") {
            input.on('', phoneMask);
        }
    })
}