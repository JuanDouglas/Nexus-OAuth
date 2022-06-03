const listId = '#applications';
$(document).ready(async function () {
    let account = await accountAsync(true);

    await loadApplications();
});

async function loadApplications() {
    var loaders = $(listId + ' .application.gray-gradient');

    loaders
       .removeClass('hidden');

    let apps = await getApplications();

    loaders
        .addClass('hidden');

    apps.forEach((obj) => {
        $(listId)
            .append(obj.coll);
    });
}

async function getApplications() {
    var appsColl = [];

    let apps = await $.ajax({
        type: 'GET',
        xhrFields: { withCredentials: true },
        url: apiHost + 'Applications/MyApplications',
    }).catch(e => {
        if (e.status == 500) {

        }
    });

    await Promise.all(apps.map(async (item, index) => {
        let logo = new NFile(item.logo.fileName, item.logo.type, item.logo.resourceType, 'png');
        let app = new Application(listId, index, item.id, item.name, item.key, item.secret, item.description, logo);
        let coll = await app.collapse();

        appsColl.push({
            obj: app,
            coll: coll
        });
    }));

    return appsColl;
}

function showModalCreate() { 
    $('#confirmationModal')
        .modal({
            backdrop: false,
            escapeClose: false,
            clickClose: false,
            showClose: false
        });

    $('body')
        .addClass('modal-open');

    $('#confirmationModal')
        .modal('show');
}

function createApplication() {

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
        var item = $('<div class="accordion-item application"/>');

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

        var inClientId = $('<div class="form-group client-id">' +
            '<label class="control-label" for="ClientId">Client ID</label>' +
            '<div class="input-group mb-3">' +
            '<input type="text" class="form-control" name="ClientId" disabled>' + 
            '<span class="input-group-text"><i class="fa-solid fa-clipboard-list"/></span></div>');

        var inClientSecret = $('<div class="form-group secret">' +
            '<label class="control-label" for="ClientSecret">Client Secret</label>' +
            '<div class="input-group mb-3">' +
            '<input type="text" class="form-control" name="ClientSecret" disabled>' +
            '<span class="input-group-text"><span class="spinner-border spinner-border-sm" role="status"/></span></div>');

        inClientId
            .find('input')
            .val(this.secret);

        inClientId
            .find('input')
            .val(this.key);

        content.append(inClientId);
        content.append(inClientSecret);

        return coll;
    }
}