document.onload = function () {
    var htmlId = document.getElementById('client_id');
    console.log(htmlId);
}

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
    }

    xhr.send();
}