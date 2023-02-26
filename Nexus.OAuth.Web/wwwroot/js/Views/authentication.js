const rand = () => Math.random(0).toString(36).substr(2);
const token = (length) => (rand() + rand() + rand() + rand()).substr(0, length);
var qrCode, sck, fsToken;

$(document).ready(async function () {
    urlBack = $('.step#secondStep')
        .data('redirect');

    if (account != undefined)
        redirectTo(urlBack);

    bLoader = new BeautifulLoader('#loader');

    $('#btnNext')
        .on('click', firstStep);

    $('#Password+.input-group-text')
        .on('click', showPassword);

    $('.card-center')
        .parallaxEffect();

    loadInputs();
    getQrCode(true, theme, 5);
});

function showPassword() {
    let tag = $('#Password+.input-group-text i');
    tag.removeClass('fa-eye');
    tag.addClass('fa-eye-slash');

    $('#Password')
        .attr('type', 'text');

    $('#Password+.input-group-text')
        .on('click', hidePassword);
}

function hidePassword() {
    let tag = $('#Password+.input-group-text i');
    tag.removeClass('fa-eye-slash');
    tag.addClass('fa-eye');

    $('#Password')
        .attr('type', 'password');

    $('#Password+.input-group-text')
        .on('click', showPassword);
}

function firstStep() {
    let user = $('#User')
        .val();

    if (user.length < 1) {
        addError('#User', 'Você deve digitar um usuário.');
        return;
    }

    hide('#firstStep');
    hide('.qr-code-component');
    bLoader.start();

    $.ajax({
        method: 'GET',
        url: apiHost + 'Authentications/FirstStep?user=' + encodeURIComponent(user),
        headers: getClientKeyHeader(),
        success: firstStepFinish,
        error: async function (xhr) {
            if (xhr.status == 404) {
                addError('#User', 'Usuário inválido ou não existe!');
            }

            await bLoader.stop();
            show('#firstStep');
            show('.qr-code-component');
        }
    });
}

async function firstStepFinish(result) {
    fsToken = result;

    $('#btnLogin')
        .on('click', secondStep);
    var file = result.profileImage;
    let image = new NFile(file.fileName, file.type, file.resourceType, 'png');

    file = await image.download();

    $('.profile img#Image')
        .attr('src', file);

    $('.profile #Name')
        .html(result.userName);

    $('.profile #User')
        .html($('#User').val());

    await bLoader.stop();
    show('#secondStep');
    show('.qr-code-component');
}

function secondStep() {
    let url = apiHost + 'Authentications/SecondStep?pwd=' + encodeURIComponent($('#Password').val()) + '&fs_id=' + fsToken.id + '&token=' + fsToken.token;
    hide('#secondStep');
    hide('.qr-code-component');
    bLoader.start();

    $.ajax({
        method: 'GET',
        url: url,
        headers: getClientKeyHeader(),
        success: async function (response) {
            let redirect = $('#secondStep')
                .data('redirect');

            await bLoader.stop();

            setAuthenticationCookie(response.token, fsToken.token, response.tokenType);
            redirectTo(redirect);
        },
        error: async function (xhr) {
            if (xhr.status == 401) {
                addError('#Password', 'Usuário ou senha incorretos!')
            }

            await bLoader.stop();
            show('#secondStep');
            show('.qr-code-component');
        }
    });

    var xhr = new XMLHttpRequest();
    xhr.open('GET', encodeURI(url));
    xhr.origin = origin;
    xhr.withCredentials = false;
    xhr.responseType = 'json';
    xhr.onload = function () {
        var status = xhr.status;
        if (status == 200) {

        } else {
            addError('#Password', 'User or password incorrect!');
            closeLoader();
        }
    }
    xhr.setRequestHeader('Client-Key', clientKey);
    xhr.send();
}

function getClientKey() {
    var original = window.localStorage.getItem('clientKey');

    if (original == null) {
        original = token();
        window.localStorage.setItem('clientKey', original);
    }

    return original;
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

function redirectToRegister() {
    var urlback = urlBack;
    if (urlback == undefined) {
        urlback = window.location.href;
    }

    redirectAndReturn('../Account/Register', false, urlback);
}

function redirectToRecover() {
    var urlback = urlBack;
    if (urlback == undefined) {
        urlback = window.location.href;
    }

    redirectAndReturn('../Account/Recovery', false, urlback);
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
            getQrCode(true, theme, 5);
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