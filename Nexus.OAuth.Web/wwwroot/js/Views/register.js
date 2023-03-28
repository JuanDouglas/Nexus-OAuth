const showTerms = () => $('#termsAndCaptcha').modal('show');
var account = undefined;
var step = 0;

$(document).ready(function () {
    loadInputs();

    urlBack = $('.terminal')
        .data('redirect');

    if (urlBack == undefined) {
        urlBack = '/Applications';
    }

    $('.terminal input').keydown(async (eve) => {
        if (eve.keyCode === 13 && terminalBlocked === false) {
            enterTerminal();
        }
    });

    $('.terminal button')
        .on('click', enterTerminal);

    sendChat('');

    $('.terminal').parallaxEffect();
});

async function sendChat(text) {
    terminalBlocked = true;
    let trm = $('.terminal');

    try {
        let response = await $.post(`RegisterChat?input=${encodeURIComponent(text)}&step=${step}`);

        if (response.status == 200) {
            step = response.nextStep;

            let effect = true;
            let input = trm.find('#input');

            input.attr('placeholder', response.placeHolder);
            input.attr('type', response.type);

            if (step === 2) {
                account.Name = text;
            } else if (step === 3) {
                account.Email = text.split('\0')[0];
                $('input[type="phone"]')
                    .on('keyup', phone);
            } else if (step === 4) {
                trm.find('input')
                    .off('keyup');
                account.Phone = text;
            } else if (step === 5) {
                account.DateOfBirth = text;
            } else if (step === 6) {
                account.Password = text;
            } else if (step === 7) {
                account.ConfirmPassword = account.Password;
                effect = false;
                $('.terminal input')
                    .attr('disabeld', 'disabled');
                showTerms();
            }

            await terminalAddText(trm, response.object, effect);
        }
        else if (response.status = 202) {
            sendToApi();
        }
    } catch (e) {
        console.log(e);

        if (e.status == 400) {

            if (step == 7) {
                let data = e.responseJSON;

                $(Object.keys(data))
                    .each((p, obj) => {
                        let field = data[obj].errorMessage;
                        let error = data[obj].memberNames;
                        addError('#' + field, error);
                    });
                return;
            }

            let error = e.responseJSON[0];
            await terminalAddText(trm, error.errorMessage, false, false);
        }
    }
}

async function sendAccount() {
    await sendChat($('#AcceptTerms').is(':checked').toString() + '\0' +
        $('#AcceptTermsCaptcha').is(':checked').toString());

    if (step == 8) {
        console.log('Pode continuar');
    }
}

async function sendToApi() {
    account.Culture = navigator.language;
    account.AcceptTerms = true;
    account.hCaptchaToken = hcaptcha.getResponse();
    account.DateOfBirth = account.DateOfBirth.replace(/^(\d{2})\/(\d{2})\/(\d{4})$/, '$3-$2-$1T00:00:00Z');

    try {
        await $.ajax({
            url: apiHost + 'Account/Register',
            type: 'PUT',
            data: JSON.stringify(account),
            contentType: 'application/json'
        });

        login(account.Email, account.Password, $('.terminal').data('redirect'));
    } catch (e) {
        alert(e.errors);
    }
}

async function verifyEmail(email) {
    try {
        await $.ajax({
            method: 'GET',
            url: apiHost + 'Authentications/FirstStep?noContent=true&user=' + encodeURIComponent(email),
            headers: getClientKeyHeader(),
        });
    } catch (e) {
        if (e.status === 404) {
            return true;
        }
    }

    return false;
}

async function login(user, pas, redirect) {
    let url = apiHost + 'Authentications/FirstStep?user=' + encodeURIComponent(user);
    let fs = await $.ajax({
        method: 'GET',
        url: url,
        headers: getClientKeyHeader()
    });

    url = apiHost + 'Authentications/SecondStep?pwd=' + encodeURIComponent(pas) + '&fs_id=' + fs.id + '&token=' + fs.token;

    let response = await $.ajax({
        method: 'GET',
        url: url,
        headers: getClientKeyHeader()
    });

    setAuthenticationCookie(response.token, fs.token, response.tokenType);
    redirectTo(redirect);
}

async function enterTerminal() {
    let trm = $('.terminal');
    let input = trm.find('input').val();
    input = $('<div>').text(input).html();

    if (account === undefined) {
        account = {};
    }

    if (step === 5 || step === 6) {
        await terminalAddText(trm, '*********', false, true);

        if (step === 6) {
            input = account.Password + '\0' + input;
            sendChat(input);
            return;
        }
    } else if (step == 2) {
        await terminalAddText(trm, input, false, true);
        let verified = await verifyEmail(input);
        input = input + '\0' + verified.toString();
    } else {

        if (step === 4) {
            input = input.replace(/(\d{4})-(\d{2})-(\d{2})/, "$2/$3/$1");
        }

        await terminalAddText(trm, input, false, true);
    }

    await sendChat(input);
}

function counter(id, start, end, duration) {
    let obj = document.getElementById(id),
        current = start,
        range = end - start,
        increment = end > start ? 1 : -1,
        step = Math.abs(Math.floor(duration / range)),
        timer = setInterval(() => {
            current += increment;
            obj.textContent = current;
            if (current == end) {
                clearInterval(timer);
            }
        }, step);
}