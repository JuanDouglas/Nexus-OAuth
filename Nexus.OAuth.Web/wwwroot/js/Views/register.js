$(document).ready(function () {
    loadInputs();

    urlBack = $('.terminal')
        .data('redirect');

    if (urlBack == undefined) {
        urlBack = '/Applications';
    }

    $('.terminal input').keydown(async (eve) => {
        if (eve.keyCode === 13 && terminalBlocked == false) {
            let input = eve.target.value;
            input = $('<div>').text(input).html();

            if (account === undefined) {
                account = {};
            }

            if (step === 5 || step === 6) {
                await terminalAddText($(eve.currentTarget).parent(), '*********', false, true);

                if (step === 6) {
                    sendChat(account.Password + '\0' + input);
                    return;
                }
            } else {

                if (step === 4) {
                    input = input.replace(/(\d{4})-(\d{2})-(\d{2})/, "$2/$3/$1");
                }

                await terminalAddText($(eve.currentTarget).parent(), input, false, true);
            }

            await sendChat(input);
        }
    })

    sendChat('');
});

var showTerms = () => $('#termsAndCaptcha').modal('show');
var account = undefined;
var step = 0;
async function sendChat(text) {
    terminalBlocked = true;
    let trm = $('.terminal');

    try {
        let response = await $.post(`RegisterChat?input=${encodeURIComponent(text)}&step=${step}`);

        if (response.status == 200) {
            step = response.nextStep;

            if (step == 8) {

            }

            let effect = true;
            let input = trm.find('#input');

            input.attr('placeholder', response.placeHolder);
            input.attr('type', response.type);

            if (step === 2) {
                account.Name = text;
            } else if (step === 3) {
                account.Email = text;
            } else if (step === 4) {
                account.Phone = text;
            } else if (step === 5) {
                account.DateOfBirth = text;
            } else if (step === 6) {
                account.Password = text;
            } else if (step === 7) {
                account.ConfirmPassword = account.Password;
                effect = false;
                showTerms();
            }

            await terminalAddText(trm, response.object, effect);
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
                    })
            }

            let error = e.responseJSON[0];
            await terminalAddText(trm, error.errorMessage, false, false);
        }
    }
}

async function sendAccount() {
    sendChat($('#AcceptTerms').val() + '\0' + $('#AcceptTermsCaptcha').val());
}