// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

/*const apiHost = 'https://localhost:44360/api/';  */ // --> Local api url
const apiHost = 'https://nexus-oauth-api.azurewebsites.net/api/';// -->  publish site url

function getAccount(redirect) {
    var url = apiHost + 'Accounts/MyAccount';

    var xhr = new XMLHttpRequest();
    xhr.open('GET', url, true);
    xhr.origin = origin;
    xhr.withCredentials = true;
    xhr.responseType = 'json';

    xhr.onload = function () {
        var status = xhr.status;

        if (status == 401 && redirect) {
            redirectAndBack('/Authentication');
        }

        if (status == 200) {
            account = xhr.response;
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
    element.classList.remove('hidden');
}

function hide(id) {
    var element = document.getElementById(id);
    element.classList.add('hidden');
}

function redirectAndBack(url, containsQuery) {
    var backQuery = 'after=' + encodeURIComponent(window.location);
    if (containsQuery) {
        backQuery = '&' + backQuery;
    } else {
        backQuery = '?' + backQuery;
    }
    redirectTo(url + backQuery);
}

function redirectTo(url) {
    $('#redirectModal')
        .modal('show', { backdrop: 'static', keyboard: false });

    document.title = 'Redirecting...';

    setTimeout(function () {
        window.location = url;
    }, 2500);
}

function removeError(id) {
    $('.form-group, .form-check').each((e, obj) => {
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
    $('.form-group').each((e, obj) => {
        var input = $(obj).find('input');

        if (input[0] != undefined) {
            if (input[0].id == id) {
                input.addClass('input-validation-error');

                var label = $(obj).find('span');
                label.addClass('field-validation-error');
                label.html(error);
            }
        }
    })

    $('.form-check').each((e, obj) => {
        var input = $(obj).find('input');

        if (input[0] != undefined) {
            if (input[0].id == id) {
                input.addClass('input-validation-error');

                var label = $(obj).find('span');
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

        if (input.attr('type') == "phone") {
            input.on('keyup', phone);
        }
    })
}

function redirectToLogin() {
    redirectAndBack('../Authentication', false);
}

function downloadFile(fileName,type, resourceType, extension, callback) {
    var xhr = new XMLHttpRequest();
    var url = apiHost + 'Files/' + encodeURIComponent(type) + '/Download?fileName=' + encodeURIComponent(fileName)
        + '&resourceType=' + encodeURIComponent(resourceType)
        + '&extension=' + encodeURIComponent(extension);

    let file;

    xhr.open('GET', url, true);
    xhr.origin = origin;
    xhr.withCredentials = true;
    xhr.responseType = 'blob';
    xhr.onload = function () {
        var blob = xhr.response;

        if (xhr.status != 200) {
            return undefined;
        }

        file = URL.createObjectURL(blob);

        callback(file);
    }

    xhr.send(null);
}