﻿// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

const sleep = ms => new Promise(r => setTimeout(r, ms));
const apiHost = 'https://oauth-api.nexus-company.net/api/';
var needConfirmation = false;
var loginRequired = false;
var account = undefined;
var theme = 'dark';

$(document).ready(async function () {
    await loadAccountAsync(loginRequired, needConfirmation);
});

function getAccount(redirect, needConfirmation = true) {
    var url = apiHost + 'Account/MyAccount';

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

            if (redirect &&
                needConfirmation &&
                account.confirmationStatus == 'NotValided') {
                requestAccountConfirmation();
            }
        }
    }

    xhr.send();
}

function getClientKeyHeader() {
    return { "Client-Key": getClientKey() }
}

async function accountAsync(redirect, needConfirmation = true) {
    let account = await $.get({
        url: apiHost + 'Account/MyAccount',
        xhrFields: { withCredentials: true }
    }).catch((e) => {
        if ((e.status == 401 || e.status == 0) && redirect) {
            redirectAndBack('/Authentication');
        }
    });

    if (redirect &&
        needConfirmation &&
        account.confirmationStatus == 'NotValided') {
        await requestAccountConfirmation();
    }
    return account;
}

async function loadAccountAsync(redirect = true, needConfirmation = true) {
    var account = await accountAsync(redirect, needConfirmation);

    if (account === undefined) {
        return;
    }

    $('#loginPanel')
        .remove();

    let profile = account.profileImage;
    let file = new NFile(profile.fileName, profile.type, profile.resourceType, 'png')

    $('#userPanel img')
        .attr('src', await file.download());

    $('#userPanel #shortName')
        .text(account.shortName);

    $('#userPanel')
        .removeClass('invisible');
}

async function requestAccountConfirmation() {
    var html = await $.get('/Account/ConfirmationModal');

    $('body')
        .append($(html));

    $('#confirmationModal')
        .modal({
            backdrop: false,
            escapeClose: false,
            clickClose: false,
            showClose: false
        });

    $('body')
        .addClass('modal-open');

    $('#confirmationModal')
        .modal('show');
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

async function logout() {
    await $.ajax({
        type: 'POST',
        xhrFields: { withCredentials: true },
        url: apiHost + 'Authentications/Logout',
    });

    redirectTo(window.location);
}

function redirectToLogin() {
    var urlback = urlBack;
    if (urlback == undefined) {
        urlback = window.location.href;
    }
    redirectAndReturn('../Authentication', false, urlback);
}

function redirectAndBack(url, containsQuery) {
    redirectAndReturn(url, containsQuery, window.location.href);
}

function redirectAndReturn(url, containsQuery, backUrl) {
    var origin = window.location.origin;
    backUrl = backUrl.replace(origin, '');

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

                var label = $(obj).find('span.field-validation-error');
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
            .find('span.field-validation-valid');

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

    $('.form-check').each((e, obj) => {
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

class NFile {
    constructor(fileName, type, resourceType, extension) {
        this.fileName = fileName;
        this.type = type;
        this.resourceType = resourceType;
        this.extension = extension;
    }

    async download() {
        let url = apiHost + 'Files/' + encodeURIComponent(this.type) + '/Download?fileName=' + encodeURIComponent(this.fileName)
            + '&resourceType=' + encodeURIComponent(this.resourceType)
            + '&extension=' + encodeURIComponent(this.extension);

        let rst = await $.ajax({
            type: 'GET',
            xhrFields: {
                withCredentials: true,
                responseType: 'blob'
            },
            url: url
        });

        return URL.createObjectURL(rst);
    }
}

class BeautifulLoader {
    constructor(id) {
        this.loader = id;
    }

    start() {
        show(this.loader);
        this.startTime = new Date().getTime();
    }

    async stop() {
        let time = (new Date().getTime() - this.startTime);

        if (time < 2500) {
            await sleep(2500 - time);
        }

        hide(this.loader);
    }
}

$.fn.parallaxEffect = function (options) {
    var settings = $.extend({
        speed: 15,
        smoothness: 1
    }, options);

    return this.each(function () {
        var $this = $(this);
        $this.mousemove(function (e) {
            var mouseX = e.pageX - $this.offset().left;
            var mouseY = e.pageY - $this.offset().top;
            var horzRotation = settings.speed * ((mouseX / $this.width()) - 0.5);
            var vertRotation = settings.speed * ((mouseY / $this.height()) - 0.5);
            $this.css('transform', 'perspective(1000px) rotateX(' + vertRotation / settings.smoothness + 'deg) rotateY(' + horzRotation / settings.smoothness + 'deg)');
        });
        $this.mouseleave(function () {
            $this.css('transform', 'none');
        });
    });
};