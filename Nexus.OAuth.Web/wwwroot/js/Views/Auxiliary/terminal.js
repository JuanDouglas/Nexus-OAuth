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
        output.append('<a class="server"></a>');
        let source = output.find('.server').last();

        for (var i = 0; i < text.length; i++) {
            source.append(text[i]);

            await sleep(35);
        }
    } else {
        if (me) {
            output.append('<a class="me"></a>');
            let source = output.find('.me').last();
            source.text(text);
            input.val('');
        } else {
            output.append('<a class="server"></a>');
            let source = output.find('.server').last();
            source.append(text);
        }
    }

    terminalBlocked = false;
}