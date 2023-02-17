var terminalBlocked = false;

function terminalText(terminal) {
    terminalBlocked = true;
    let output = terminal.find('#output');
    let input = terminal.find('input');

    output.append('>>  ' + input.val() + ' <br>')
    input.val('');
    terminalBlocked = false;
}

async function terminalAddText(terminal, text, effect = false, me = false) {
    terminalBlocked = true;
    let output = terminal.find('#output');
    let input = terminal.find('#input');

    if (effect) {
        output.append('> ')

        for (var i = 0; i < text.length; i++) {
            output.append(text[i]);

            await esperar(45);
        }

        output.append('<br>');
    } else {
        if (me) {
            output.append('<a class="me">' + text + '</a>');
            input.val('');
        } else {
            output.append('> ' + text + '<br>')
        }
    }

    terminalBlocked = false;
}

function esperar(tempo) {
    return new Promise(function (resolve) {
        setTimeout(function () {
            resolve();
        }, tempo);
    });
}