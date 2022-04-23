$(document).ready(function () {
    openLoader();

    component = $('#component');

    let clientId = component.data('client-id');

    getApplication(clientId);
});

function getApplication(clientId) {
    getAccount(true);

    var xhr = new XMLHttpRequest();
    var url = apiHost + 'Applications/ByClientId?client_id=' + encodeURIComponent(clientId);

    xhr.open('GET', url, true);
    xhr.origin = origin;
    xhr.withCredentials = true;
    xhr.responseType = 'json';

    xhr.onload = function () {
        var status = xhr.status;

        if (status == 401) {
            redirectLogin();
        }

        application = xhr.response;

        loadApplication();
    }

    xhr.send();
}

function loadApplication() {
    var app = $('.application');

    var logo = application.logo;

    downloadFile(logo.fileName, 'Image', logo.resourceType, 'png',
        function (blob) {
            var img = component.find('.icon');
            img.attr('src', blob);
        });

    app.find('.name')
        .text(application.name);

    app.find('.description')
        .text(application.description);

    app.find('a.icon-back')
        .attr('href', application.site)

    app.find('.verified')
        .tooltip();

    closeLoader();

    if (application.internal) {
        app
            .find('.verified')
            .removeClass('visually-hidden');
    }
}

async function authorize() {
    while (typeof account == 'undefined') {
        await new Promise(r => setTimeout(r, 10));
    }

    var url = apiHost + 'OAuth/Authorize?client_id=' + encodeURIComponent(component.data('client-id'))
        + '&state=' + encodeURIComponent(component.data('state'))
        + '&scopes=' + encodeURIComponent(component.data('scopes'));

    redirectTo(url);
}
