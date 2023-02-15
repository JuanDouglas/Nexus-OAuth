var terminalBlocked = false;

function terminalText(terminal) {
    terminalBlocked = true;
    let output = terminal.find('#output');
    let input = terminal.find('input');

    output.append('>>  ' + input.val() + ' <br>')
    input.val('');
    terminalBlocked = false;
}

async function terminalAddText(terminal, text, effect = false) {
    terminalBlocked = true;
    let output = terminal.find('#output');

    if (effect) {
        output.append('>> ')

        for (var i = 0; i < text.length; i++) {
            output.append(text[i]);

            await esperar(50);
        }

        output.append('<br>');
    } else {
        output.append('>> ' + text + '<br>');
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