// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const apiHost = 'https://localhost:44360/api/';
/* const apiHost = 'https://nexus-oauth.duckdns.org/api/'; *///  publish site url

function getAccount(redirect) {
    var url = apiHost + 'Accounts/MyAccount';

    var xhr = new XMLHttpRequest();
    xhr.open('GET', url);
    xhr.origin = origin;
    xhr.withCredentials = true;
    xhr.responseType = 'json';

    xhr.onload = function () {
        var status = xhr.status;

        if (status == 401 && redirect) {

        }

        if (status == 200) {
            return xhr.response;
        }
    }

    xhr.send();
}


function setError(field, error) {
    console.log(field + ': ' + error);
    var htmlField = document.getElementById(field);

    htmlField.classList.add('input-validation-error');
    htmlField.addEventListener('click', clearError)

    var textField = document.getElementById(field + '-error');
    if (textField != null) {
        textField.classList.add('field-validation-error');
        textField.innerText = error;
    }

    closeLoader();
}

function clearError() {
    this.classList.remove('error');

    var textField = document.getElementById(this.id + '-error');
    if (textField != null) {
        textField.innerText = '';
    }
}

function openLoader() {
    hide('component');
    show('loader');
}

function closeLoader() {
    hide('loader');
    show('component');
}

/*
 
 
 @param 
 */
function show(id) {
    var element = document.getElementById(id);
    element.classList.remove('invisible');
}

function hide(id) {
    var element = document.getElementById(id);
    element.classList.add('invisible');
}

function redirectAndBack(url, containsQuery) {
    var backQuery = 'after=' + encodeURIComponent(window.location);
    if (containsQuery) {
        backQuery = '&' + backQuery;
    } else {
        backQuery = '?' + backQuery;
    }
    redirectTo(url + backQuery);
}

function redirectTo(url) {
    $('#redirectModal').modal('show', { backdrop: 'static', keyboard: false });
    setTimeout(function () {
        window.location = url;
        document.title = 'Redirecting...';
    }, 2500);
}

function redirectToLogin() {
    redirectAndBack('../Authentication',false);
}