﻿const rand = () => Math.random(0).toString(36).substr(2);
const token = (length) => (rand() + rand() + rand() + rand()).substr(0, length);
const authHeader = (tokenType, token, firstStepToken) => tokenType + ' ' + token + '.' + firstStepToken;

var qrCode, auth, sck, fsToken,
    awaitStep = false,
    step = 1,
    codeInputs = $('.number-inputs input');

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

    $('input').keypress(function (event) {
        // Verifica se a tecla pressionada é a tecla Enter (código 13)
        if (event.keyCode === 13) {
            if (step === 1) {
                firstStep();
            } else if (step === 2) {
                secondStep();
            }
        }
    });

    $('#twoFactor div.buttons button')
        .on('click', tfaTypeClick)

    codeInputs = $('.number-inputs input');

    codeInputs.keydown(function () {
        $(this).val('');
    });

    codeInputs.keyup(async function (event) {
        var currentInput = $(this).val();

        // Verifica se a tecla pressionada é backspace ou delete
        if (event.keyCode === 8 || event.keyCode === 46) {
            return; // Não chama a função sendTfaCode
        }

        // Verifica se o valor é um número
        if (!isNaN(currentInput)) {
            var currentIndex = codeInputs.index(this);

            // Verifica se o caractere digitado foi a tecla Enter (código 13)
            if (currentIndex === codeInputs.length - 1) {
                // O usuário pressionou Enter no último input
                await sendTfaCode();
            } else {
                // Passa para o próximo input
                $(codeInputs[currentIndex + 1]).focus();
            }
        }
    });

    loadInputs();
    getQrCode(true, theme, 5);

    $('#User')
        .focus();
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
    if (awaitStep) {
        return;
    }

    awaitStep = true;

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
        url: apiHost + 'Authentications/FirstStep?hCaptchaToken=' + hcaptcha.getResponse() + '&user=' + encodeURIComponent(user),
        headers: getClientKeyHeader(),
        success: firstStepFinish,
        error: async function (xhr) {
            if (xhr.status == 404) {
                addError('#User', 'Usuário inválido ou não existe!');
            }

            await bLoader.stop();
            show('#firstStep');
            show('.qr-code-component');

            awaitStep = false;
        }
    });
}

async function firstStepFinish(result) {
    fsToken = result;
    awaitStep = false;

    $('#btnLogin')
        .on('click', secondStep);

    $('#btnLogin button')
        .off('click', firstStep);

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
    $("#Password").focus();
}

function secondStep() {
    if (awaitStep) {
        return;
    }

    awaitStep = true;

    let url = apiHost + 'Authentications/SecondStep?pwd=' + encodeURIComponent($('#Password').val()) + '&fs_id=' + fsToken.id + '&token=' + fsToken.token;
    hide('#secondStep');
    hide('.qr-code-component');
    bLoader.start();

    $.ajax({
        method: 'GET',
        url: url,
        headers: getClientKeyHeader(),
        success: async function (response, txt, xhr) {
            await bLoader.stop();
            show('#secondStep');
            show('.qr-code-component');

            auth = {
                token: response.token,
                fsToken: fsToken.token,
                tokenType: response.tokenType
            };

            if (xhr.status == 226) {
                $('#twoFactor')
                    .modal('show');

                $('#twoFactor').on('hide.bs.modal', function (e) {
                    e.preventDefault();
                });

                return;
            }

            awaitStep = false;
            sck.close();
            setAuthenticationCookie(auth.token, auth.token, auth.tokenType);
        },
        error: async function (xhr) {
            if (xhr.status == 401) {
                addError('#Password', 'Usuário ou senha incorretos!')
            }

            awaitStep = false;
            await bLoader.stop();
            show('#secondStep');
            show('.qr-code-component');
        }
    });
}

function getClientKey() {
    var original = window.localStorage.getItem('clientKey');

    if (original == null) {
        original = token();
        window.localStorage.setItem('clientKey', original);
    }

    return original;
}

function setAuthenticationCookie() {
    var clientKey = getClientKey();
    var url = apiHost + 'Authentications/SetCookie';

    var xhr = new XMLHttpRequest();
    xhr.open('OPTIONS', url, true);
    xhr.origin = origin;
    xhr.withCredentials = true;
    xhr.onload = function () {
        console.log('Define Authentication token cookie!');

        let redirect = $('#secondStep')
            .data('redirect');

        redirectTo(redirect);
    }

    xhr.setRequestHeader('Authorization', authHeader(auth.tokenType, auth.token, auth.fsToken));
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

async function tfaTypeClick(event) {
    auth.tfaType = $(event.currentTarget).data('key');
    await sendTfaType();
}
async function sendTfaType() {
    let url = apiHost + 'Authentications/TwoFactor/Send?type=' + encodeURIComponent(auth.tfaType);

    hide('#tfaCode');
    hide('#tfaType');
    show('#tfaLoader');

    let response = await $.ajax({
        method: 'GET',
        url: url,
        headers: {
            "Client-Key": getClientKey(),
            "Authorization": authHeader(auth.tokenType, auth.token, auth.fsToken)
        }
    });

    hide('#tfaLoader');
    show('#tfaCode');

    awaitStep = false;
    let tfaCode = $('#tfaCode p'); // Obtém o conteúdo HTML do elemento
    var newContent = tfaCode.html().replace('{tfaType}', $('button[data-key="' + auth.tfaType + '"]').data('name')); // Substitui a palavra "exemplo" por "teste"
    tfaCode.html(newContent); // Atualiza o conteúdo HTML do elemento com a nova string
}

async function sendTfaCode() {
    if (awaitStep) {
        return;
    }

    awaitStep = true;

    hide('#tfaCode');
    show('#tfaLoader');

    let code = "";
    $("#tfaCode .input-number").each(function () {
        code += $(this).val();
    });

    let url = apiHost + 'Authentications/TwoFactor/Confirm?type=' + encodeURIComponent(auth.tfaType) + '&code=' + encodeURIComponent(code);

    let response = await $.ajax({
        method: 'POST',
        url: url,
        headers: {
            "Client-Key": getClientKey(),
            "Authorization": authHeader(auth.tokenType, auth.token, auth.fsToken)
        }
    }).catch((e) => {
        awaitStep = false;
        hide('#tfaLoader');
        show('#tfaCode');
    });;

    if (awaitStep === false) {
        return;
    }

    awaitStep = false;
    setAuthenticationCookie(auth.token, auth.token, auth.tokenType);
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