$(document).ready(function () {
    openLoader();

    let clientId = $('#component').data('client-id');

    getApplication(clientId);
});

function getApplication(clientId) {
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
    if (application.internal)
    {
        console.log('Is internal');
    }
}

function authorize()
{

}
