var type, token;

$(document).ready(async function () {
    var account = await accountAsync(true, false);
    var ctn = $('#content');

    type = ctn.data('type');
    token = ctn.data('token');

    if (type != undefined &&
        token != undefined) {
        openLoader();
        await confirmAccount(type, token);
    }
});

async function confirmAccount(type, token) {
    let url = apiHost + 'Account/Confirm?type=' + encodeURIComponent(type) +
        '&token=' + encodeURIComponent(token);

    return await $.ajax({
        url: url,
        method: 'POST',
        xhrFields: {
            withCredentials: true
        }
    });
}

async function sendConfirmation() {
    await $.ajax({
        url: apiHost + 'Account/SendConfirmation',
        method: 'POST',
        xhrFields: {
            withCredentials: true
        }
    });
}

async function checkConfirmation() {
    await accountAsync(true);
}