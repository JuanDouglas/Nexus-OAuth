var type, token;

$(document).ready(async function () {
    var account = await accountAsync(true);
    var ctn = $('#content');

    type = ctn.data('type');
    token = ctn.data('token');

    await confirmAccount(type, token);
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