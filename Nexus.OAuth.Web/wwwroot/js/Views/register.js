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
            await terminalAddText($(eve.currentTarget).parent(), input);
            await sendChat(input);
        }
    })

    sendChat('');
});

var step = 0;
async function sendChat(text) {
    terminalBlocked = true;
    let response = await $.get(`RegisterChat?input=${encodeURIComponent(text)}&step=${step}`);

    if (response.status == 200) {
        await terminalAddText($('.terminal'), response.object, true);
    }

    console.log(response);
}
