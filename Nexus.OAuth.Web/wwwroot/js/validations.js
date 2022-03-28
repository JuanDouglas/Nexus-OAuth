function showOrHidePassword(idPasswordInput, idCheckbox) {
    var inputPassword = document.getElementById(idPasswordInput)
    var checkbox = document.getElementById(idCheckbox);
    if (inputPassword.type == "password") {
        inputPassword.type = "text";
        checkbox.innerText = 'Hide password';
    } else {
        inputPassword.type = "password";
        checkbox.innerText = 'Show password';
    }
}

function phone(e)
{
    phoneMask('#' + e.currentTarget.id);
}

function phoneMask(selector) {
    if (typeof selector == "string") {
        var input = $(selector)[0];
    }

    if (typeof e == "object") {
        var input = e.currentTarget;
    }

    removeLatters(input);
    const phone = numbersOnly(input.value);
    let text = phone;

    if (phone.length > 5 && phone.length < 9) {
        const part1 = phone.slice(0, 5);
        const part2 = phone.slice(5, phone.length);
        text = `${part1}-${part2}`
    }

    if (phone.length > 8 && phone.length < 10) {
        const p1 = phone.slice(0, 1);
        const p2 = phone.slice(1, 5);
        const p3 = phone.slice(5, 9);
        text = `${p1}${p2}-${p3}`
    }

    if (phone.length == 11) {
        const p1 = phone.slice(0, 2);
        const p2 = phone.slice(2, 3);
        const p3 = phone.slice(3, 7);
        const p4 = phone.slice(7, 11);
        text = `(${p1}) ${p2}${p3}-${p4}`
    }

    if (phone.length == 13) {
        const p0 = phone.slice(0, 2);
        const p1 = phone.slice(2, 4);
        const p2 = phone.slice(4, 5);
        const p3 = phone.slice(5, 9);
        const p4 = phone.slice(9, 13);
        text = `+${p0} (${p1}) ${p2}${p3}-${p4}`
    }

    input.value = text;
}

function zipCodeMask(input) {
    removeLatters(input);
    const zipCode = numbersOnly(input.value);
    let text = zipCode;
    if (zipCode.length > 5) {
        const part1 = zipCode.slice(0, 5);
        const part2 = zipCode.slice(5, zipCode.length);
        text = `${part1}-${part2}`
        if (zipCode.length > 7) {
            getAdressByCep(input.id.slice(0, input.id.length - 3));
        }
    }
    input.value = text;
}

function getAdressByCep(id) {
    var cepField = document.getElementById(id + 'Cep');
    var cityField = document.getElementById(id + 'City');
    var nightboordField = document.getElementById(id + 'Nbhood');
    var placeField = document.getElementById(id + 'PPlace');

    var url = 'https://open-cep.azurewebsites.net/api/Cep/ByNumber?number=' + numbersOnly(cepField.value);
    var xhr = new XMLHttpRequest();
    xhr.open('GET', url, true);
    xhr.orgin = origin;
    xhr.withCredentials = false;
    xhr.responseType = 'json';
    xhr.onload = function () {
        var status = xhr.status;
        if (status == 200) {
            var resp = xhr.response;

            if (cityField != null) {
                url = 'https://open-cep.azurewebsites.net/api/Cities/ByID?id=' + resp.cityID;
                xhr.open('GET', url, true);
                xhr.onload = function () {
                    status = xhr.status;
                    if (status == 200) {
                        resp = xhr.response;
                        cityField.value = resp.name;
                    }
                }
                xhr.send();
            }

            if (nightboordField != null) {
                nightboordField.value = resp.neighborhood;
            }

            if (placeField != null) {
                placeField.value = resp.publicPlace;
            }
        }
        else {
            enableError(cepField.id, 'Cep nao encontrado!');
        }
    };
    xhr.send();
}

function numbersOnly(str) {
    return str.replace(/[^0-9]/g, "")
}

const getCookie = (name) => {
    const cookies = document.cookie.split(';');
    for (let i = 0; i < cookies.length; i++) {
        let c = cookies[i].trim().split('=');
        if (c[0] === name) {
            return c[1];
        }
    }
    return "";
}

function removeLatters(input) {
    var regex = /^[0-9-./() ]+$/;
    var match = regex.test(input.value);
    if (!match) {
        input.value = input.value.replace(/^[0-9-() ]+$/);
    }
}

function cpfOrCnpjMask(input) {
    removeLatters(input);
    var numbers = numbersOnly(input.value);
    let text;
    if (numbers.length < 12) {
        if (numbers.length < 4) {
            text = numbers;
        } else if (numbers < 7) {
            var pOne = numbers.slice(0, 3);
            var pTwo = numbers.slice(3, numbers.length);
            text = pOne + '.' + pTwo;
        } else if (numbers.length < 10 && numbers.length > 6) {
            var pOne = numbers.slice(0, 3);
            var pTwo = numbers.slice(3, 6);
            var pThree = numbers.slice(6, numbers.length);
            text = pOne + '.' + pTwo + '.' + pThree;
        } else if (numbers.length == 11) {
            var pOne = numbers.slice(0, 3);
            var pTwo = numbers.slice(3, 6);
            var pThree = numbers.slice(6, 9);
            var pFor = numbers.slice(9, numbers.length);
            text = pOne + '.' + pTwo + '.' + pThree + '-' + pFor;
        }
    } else if (numbers.length > 11) {
        var pOne = numbers.slice(0, 2);
        var pTwo = numbers.slice(2, 5);
        var pThree = numbers.slice(5, 8);
        var pFor = numbers.slice(8, 12);
        var pFive = '';
        if (numbers.length > 12) {
            pFive = '-' + numbers.slice(12, 14);
        }
        text = pOne + '.' + pTwo + '.' + pThree + '/' + pFor + pFive;
    }
    input.value = text;
}

function checkErrors(fields) {
    var result = true;
    for (var i = 0; i < fields.length; i++) {
        let rst = checkField(fields[i]);

        if (rst == false) {
            result = false;
        }
    }

    return result;
}

function checkField(selector) {
    var field = $(selector);

    if (field.val() == '') {
        addError(field.attr('id'), field.data('val-required'));
        return false;
    }

    return true;
}