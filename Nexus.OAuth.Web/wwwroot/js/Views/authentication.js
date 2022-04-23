const rand = () => Math.random(0).toString(36).substr(2);
const token = (length) => (rand() + rand() + rand() + rand()).substr(0, length);
var qrCode, sck;

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

        qrCode = new QrCode(
            xhr.getResponseHeader('X-Code-Id'),
            xhr.getResponseHeader('X-Code'),
            URL.createObjectURL(xhr.response),
            xhr.getResponseHeader('X-Validation'));

        document.getElementById('qrCode').src = qrCode.imageUrl;

        qrCode.awaitAuthorization();

        return;
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

class QrCode {
    constructor(id, code, imageUrl, validation) {
        this.id = id;
        this.code = code;
        this.validation = validation;
        this.imageUrl = imageUrl;
    }

    awaitAuthorization() {
        let url = apiHost.replace('https', 'wss') + 'Authentications/QrCode/AwaitAuthorization?' +
            'client_key=' + encodeURIComponent(getClientKey()) +
            '&qr_code_id=' + encodeURIComponent(this.id) +
            '&validation_token=' + encodeURIComponent(this.validation);

        sck = new WebSocket(url);

        sck.onmessage = function (event) {
            qrCode.socketMessage(event);
        };

        sck.onopen = function (e) {
            console.debug('Open connection in ' + e.target.url);
        };

        sck.onerror = function (event) {
            console.log(event);
        };

        sck.onclose = function (event) {
            console.debug('Close connection with reason "' + event.reason + '"');

            if (qrCode.authorized) {

                return;
            }

            console.debug('Starting new connection');
            loadQrCode();
        }
    }

    async socketMessage(event) {
        var resp = JSON.parse(event.data);

        this.remaingTime = resp.RemaingTime;
        this.authorized = resp.Authorized;

        if (resp.Authorized == false) {
            return;
        }

        this.authorizationToken = resp.Token;
        var auth = await qrCode.getAuthentication();

        setAuthenticationCookie(auth.token, this.validation, auth.TokenType);

        redirectTo(urlBack);
    }

    async getAuthentication() {
        let url = apiHost + 'Authentications/QrCode/AccessToken?' +
            'id=' + encodeURIComponent(this.id) +
            '&validation_token=' + encodeURIComponent(this.validation) +
            '&authorization_token=' + encodeURIComponent(this.authorizationToken);

        return await $.ajax({
            url: url,
            method: 'GET',
            headers: { "Client-Key": getClientKey() },
            success: function () {
                console.debug('QrCode authorization success!');
            }
        });
    }
}