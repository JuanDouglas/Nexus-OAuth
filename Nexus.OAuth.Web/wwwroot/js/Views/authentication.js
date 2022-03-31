const rand = () => Math.random(0).toString(36).substr(2);
const token = (length) => (rand() + rand() + rand() + rand()).substr(0, length);
var qrCode;
var qrCodeValidation;

$(document).ready(function () {
    urlBack = $('#component')
        .data('redirect');

    loadInputs();
    loadQrCode();
});

function loginClick(redirect) {
    let user = document.getElementById('User').value;
    let password = document.getElementById('Password').value;

    openLoader();
    login(user, password, redirect);
}

function getClientKey() {
    var original = window.localStorage.getItem('clientKey');

    if (original == null) {
        original = token();
        window.localStorage.setItem('clientKey', original);
    }

    return original;
}

function login(user, password, redirect) {
    var clientKey = getClientKey();
    var url = apiHost + 'Authentications/FirstStep?user=' + encodeURIComponent(user);

    var xhr = new XMLHttpRequest();

    xhr.open('GET', encodeURI(url));
    xhr.origin = origin;
    xhr.withCredentials = false;
    xhr.responseType = 'json';
    xhr.onload = function () {
        var status = xhr.status;
        if (status == 200) {
            secondStep(password, xhr.response.id, xhr.response.token, redirect);
        } else {
            addError('#User', 'This user is invalid or not register.');
            closeLoader();
        }
    }

    xhr.setRequestHeader('Client-Key', clientKey);
    xhr.send();
}

function secondStep(pwd, fs_id, token, redirect) {
    var clientKey = getClientKey();
    var url = apiHost + 'Authentications/SecondStep?pwd=' + encodeURIComponent(pwd) + '&fs_id=' + fs_id + '&token=' + token;

    var xhr = new XMLHttpRequest();
    xhr.open('GET', encodeURI(url));
    xhr.origin = origin;
    xhr.withCredentials = false;
    xhr.responseType = 'json';
    xhr.onload = function () {
        var status = xhr.status;
        if (status == 200) {
            setAuthenticationCookie(xhr.response.token, token, xhr.response.tokenType);
            console.debug('Login success!');
            redirectTo(redirect);
        } else {
            addError('#Password', 'User or password incorrect!');
            closeLoader();
        }
    }
    xhr.setRequestHeader('Client-Key', clientKey);
    xhr.send();
}

function setAuthenticationCookie(token, firstStepToken, tokenType) {
    var authorization = tokenType + ' ' + token + '.' + firstStepToken;
    var clientKey = getClientKey();
    var url = apiHost + 'Authentications/SetCookie';

    var xhr = new XMLHttpRequest();
    xhr.open('OPTIONS', url, true);
    xhr.origin = origin;
    xhr.withCredentials = true;
    xhr.onload = function () {
        window.localStorage.setItem('',);
        console.log('Define Authentication token cookie!');
    }

    xhr.setRequestHeader('Authorization', authorization);
    xhr.setRequestHeader('Client-Key', clientKey);
    xhr.send();
}

function getQrCode(transparent, theme, per_module) {
    var clientKey = getClientKey();
    var url = apiHost + 'Authentications/QrCode/Generate?theme=' + theme + '&transparent=' + transparent + '&pixeis_per_module=' + per_module;

    var xhr = new XMLHttpRequest();
    xhr.open('GET', url, true);
    xhr.origin = origin;
    xhr.responseType = 'blob';
    xhr.onload = function () {
        var blob = xhr.response;
        document.getElementById('qrCode').src = URL.createObjectURL(blob);
        qrCode = xhr.getResponseHeader('X-Code');
        qrCodeValidation = xhr.getResponseHeader('X-Validation');
    };

    xhr.setRequestHeader('Client-Key', clientKey);
    xhr.send();
}

function loadQrCode() {
    getQrCode(true, 'dark', 5);
}

function redirectToRegister() {
    var urlback = urlBack;
    if (urlback == undefined) {
        urlback = window.location.href;
    }

    redirectAndReturn('../Account/Register', false, urlback);
}