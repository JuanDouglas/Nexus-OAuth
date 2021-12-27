﻿const rand = () => Math.random(0).toString(36).substr(2);
const token = (length) => (rand() + rand() + rand() + rand()).substr(0, length);

function loginClick(redirect) {
    let user = document.getElementById('user').value;
    let password = document.getElementById('pwd').value;

    openLoader();
    login(user, password, redirect);
}

function getClientKey() {
    var original = window.localStorage.getItem('clientKey');

    if (original == null) {
        original = token();
        window.localStorage.setItem('clientKey', original);
    }

    return original;
}

function login(user, password, redirect) {
    var clientKey = getClientKey();
    var url = apiHost + 'Authentications/FirstStep?user=' + user;

    var xhr = new XMLHttpRequest();

    xhr.open('GET', encodeURI(url));
    xhr.origin = origin;
    xhr.withCredentials = false;
    xhr.responseType = 'json';
    xhr.onload = function () {
        var status = xhr.status;
        if (status == 200) {
            secondStep(password, xhr.response.id, xhr.response.token, redirect);
        } else {
            setError('user', 'This user is invalid or not register.');
        }
    }

    xhr.setRequestHeader('Client-Key', clientKey);
    xhr.send();
}

function secondStep(pwd, fs_id, token, redirect) {
    var clientKey = getClientKey();
    var url = apiHost + 'Authentications/SecondStep?pwd=' + pwd + '&fs_id=' + fs_id + '&token=' + token;

    var xhr = new XMLHttpRequest();
    xhr.open('GET', encodeURI(url));
    xhr.origin = origin;
    xhr.withCredentials = false;
    xhr.responseType = 'json';
    xhr.onload = function () {
        var status = xhr.status;
        if (status == 200) {
            setAuthenticationCookie(xhr.response.token, token, xhr.response.tokenType);
            console.log('Login success!');
            closeLoader();
            redirectTo(redirect);
        } else {
            setError('pwd', 'User or password incorrect!');
        }
    }
    xhr.setRequestHeader('Client-Key', clientKey);
    xhr.send();
}

function setAuthenticationCookie(token, firstStepToken, tokenType) {
    var authorization = tokenType + ' ' + token + '.' + firstStepToken;
    var clientKey = getClientKey();
    var url = apiHost + 'Authentications/SetCookie';

    var xhr = new XMLHttpRequest();
    xhr.open('OPTIONS', url, true);
    xhr.origin = origin;
    xhr.withCredentials = true;
    xhr.onload = function () {
        console.log('Define Authentication token cookie!');
    }

    xhr.setRequestHeader('Authorization', authorization);
    xhr.setRequestHeader('Client-Key', clientKey);
    xhr.send();
}