const apiHost = 'https://nexus-oauth.duckdns.org/api/';
const signUpButton = document.getElementById('signUp');
const signInButton = document.getElementById('signIn');
const container = document.getElementById('container');
const rand = () => Math.random(0).toString(36).substr(2);
const token = (length) => (rand() + rand() + rand() + rand()).substr(0, length);


signUpButton.addEventListener('click', () => {
    container.classList.add("right-panel-active");
});

signInButton.addEventListener('click', () => {
    container.classList.remove("right-panel-active");
});

function loginClick() {
    const user = document.getElementById('user').value;
    const password = document.getElementById('pwd').value;

    openLoader();
    login(user, password);
}

function getClientKey() {
    var original = window.localStorage.getItem('clientKey');

    if (original == null) {
        original = token();
        window.localStorage.setItem('clientKey', original);
    }

    return original;
}

function login(user, password) {
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
            secondStep(password, xhr.response.id, xhr.response.token);
        } else {
            setError('user', 'User not found!');
        }
    }

    xhr.setRequestHeader('Client-Key', clientKey);
    xhr.send();
}

function secondStep(pwd, fs_id, token) {
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
        } else {
            setError('pwd', 'Incorrect user or password!');
        }
    }
    xhr.setRequestHeader('Client-Key', clientKey);
    xhr.send();
}

function setAuthenticationCookie(token, firstStepToken, tokenType) {
    var header = tokenType + ' ' + token + '.' + firstStepToken;
    var clientKey = getClientKey();

    console.log('Define Authentication token cookie!');
}

function setError(field, error) {
    console.log(field + ': ' + error);
    var htmlField = document.getElementById(field);

    htmlField.classList.add('form-error');
    htmlField.addEventListener('click', clearError)
    closeLoader();
}

function clearError() {
    this.classList.remove('form-error');
}

function openLoader() {
    console.log('Open loader!')
}

function closeLoader() {
    console.log('Close loader!')
}

//PS: Odeio javascript
//PS: Odeio duplamente javascript