$(document).ready(function () {
    loadInputs();

    urlBack = $('.terminal')
        .data('redirect');

    if (urlBack == undefined) {
        urlBack = '/Applications'
    }

    $('.terminal input').keydown(async (eve) => {
        if (eve.keyCode === 13 && terminalBlocked == false) {
            let input = eve.target.value;
            input = $('<div>').text(input).html();

            await terminalAddText($(eve.currentTarget).parent(), input, false, true);
            await sendChat(input);
        }
    })

    sendChat('');
});

var account = {};
var step = 0;
async function sendChat(text) {
    terminalBlocked = true;
    let trm = $('.terminal');

    try {
        let response = await $.post(`RegisterChat?input=${encodeURIComponent(text)}&step=${step}`);

        if (response.status == 200) {
            step = response.nextStep;
            let input = trm.find('#input');

            input.attr('placeholder', response.placeHolder);
            input.attr('type', response.type);

            switch (step) {
                case 2:
                    account.Name = text;
                case 3:
                    account.Name = text;
                case 4:
                    account.Phone = text;
                case 5:
                    account.Birthday = text;
                case 6: 
                    account.Password = text;
                default:
            }

            await terminalAddText(trm, response.object, true);
        }
    } catch (e) {
        console.log(e);

        if (e.status == 400) {
            let error = e.responseJSON[0];
            await terminalAddText(trm, error.errorMessage, false, false);
        }
    }
}
