$(document).ready(async function () {
    let account = await accountAsync(true);

    let apps = await getApplications();
});

async function getApplications() {
    openLoader();

    const listId = '#applications';
    let list = $(listId);

    let apps = await $.ajax({
        type: 'GET',
        xhrFields: { withCredentials: true },
        url: apiHost + 'Applications/MyApplications',
    }).catch(e => {
        if (e.status == 500) {

        }
    });

    closeLoader();

    apps.forEach(async function (item, index, arr) {
        var logo = new NFile(item.logo.fileName, item.logo.type, item.logo.resourceType, 'png');
        var app = new Application(listId, index, item.id, item.name, item.key, item.secret, item.description, logo);

        var coll = await app.collapse();

        list.append(coll);
    });

    list
        .find('.application')
        .removeClass('hidden');

    list
        .find('.application.gray-gradient')
        .addClass('hidden')

    return applications;
}

class Application {
    constructor(accordion, position = Number, id = Number, name, key, secret, description, image = NFile) {
        this.accordion = accordion;
        this.position = position;
        this.name = name;
        this.description = description;
        this.logo = image;
        this.key = key;
        this.secret = secret;
    }

    async collapse() {
        var item = $('<div class="accordion-item application hidden"/>');

        var headerId = 'app' + this.position;
        var collId = 'coll' + this.position;

        var header = await this.collapseHeader(headerId, collId);
        var body = this.collapseBody(headerId, collId);

        item.append(header);
        item.append(body);

        return item;
    }

    async collapseHeader(headerId, collId) {
        var header = $('<h2 class="accordion-header" id="' + headerId + '">');
        var button = $('<button class="accordion-button" type="button" data-bs-toggle="collapse" data-bs-target="#' + collId + '" aria-expanded="true" aria-controls="' + collId + '">');

        header.append(button);
        button.html('<img/>' + this.name);

        button.find('img').attr('src', await this.logo.download())
        return header;
    }

    collapseBody(headerId, collId) {
        var coll = $('<div id="' + collId + '" class="accordion-collapse collapse" aria-labelledby="' + headerId + '" data-bs-parent="' + this.accordion + '">');
        var content = $('<div class="accordion-body">');

        coll.append(content);

        var inClientId = $('<div class="form-group">' +
            '<label class="control-label" for="ClientId">Client ID</label>' +
            '<input class= "form-control" name="ClienId" disabled></div>');

        var inClientSecret = $('<div class="form-group">' +
            '<label class="control-label" for="ClientSecret">Client Secret</label>' +
            '<input class= "form-control" name="ClienSecret" disabled></div>');

        inClientId.val(this.secret);
        inClientId.val(this.key);

        content.append(inClientId);
        content.append(inClientSecret);

        return coll;
    }
}