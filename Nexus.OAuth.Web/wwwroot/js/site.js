﻿// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

/*const apiHost = 'https://localhost:44360/api/'; */  // --> Local api url
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
    hide('#component');
    show('#loader');
}

function closeLoader() {
    hide('#loader');
    show('#component');
}

function show(id) {
    $(id).removeClass('visually-hidden');
}

function hide(id) {
    $(id).addClass('visually-hidden');
}

function redirectToLogin() {
    var urlback = urlBack;
    if (urlback == undefined) {
        urlback = window.location;
    }
    redirectAndReturn('../Authentication', false, urlback);
}

function redirectAndBack(url, containsQuery) {
    redirectAndReturn(url, containsQuery, window.location);
}

function redirectAndReturn(url, containsQuery, backUrl) {
    var backQuery = 'after=' + encodeURIComponent(backUrl);
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
        window.location.href = url;
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

function addError(selector, error) {
    $('.form-group ' + selector).each((e, obj) => {
        var input = $(obj);
        input.addClass('input-validation-error');

        var label = input
            .closest('.form-group')
            .find('span');

        label.addClass('field-validation-error');
        label.html(error);
    })
    $('.form-check ' + selector).each((e, obj) => {
        var input = $(obj);
        input.addClass('input-validation-error');

        var label = input
            .closest('.form-check')
            .find('span');

        label.addClass('field-validation-error');
        label.html(error);
    })
}

function showErrors(data) {
    $(Object.keys(data))
        .each((p, obj) => {
            let error = data[obj].find(f => true);
            addError('#' + obj, error);
        })
}

function loadInputs() {
    $('.form-group').each((e, obj) => {
        var input = $(obj).find('input');

        input.on('click', function () {
            removeError(this.id)
        })
    })

    $('input[type="phone"]')
        .on('keyup', phone)
}

function downloadFile(fileName, type, resourceType, extension, callback) {
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