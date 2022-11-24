const listId = '#applications';
const appsId = listId + ' .accordion-item.application';
const secret = '*********************';
const faEye = '<i class="fa-solid fa-eye"/>';
needConfirmation = true;
loginRequired = true;

$(document).ready(async function () {
    await loadApplications();
});

async function loadApplications() {
    $(appsId)
        .remove();

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

    $(appsId + ' .collapse')
        .first()
        .collapse('toggle');
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
        let app = new Application(listId, index, item.id, item.name, item.key, secret, item.status, item.description, logo);
        let coll = await app.collapse();

        appsColl.push({
            obj: app,
            coll: coll
        });
    }));

    return appsColl;
}

function showModalCreate() {
    $('#createApplicationModal')
        .modal()
        .show();

    $('body')
        .addClass('modal-open');
}

function closeModalCreate() {
    $('#createApplicationModal')
        .modal()
        .hide();

    $('body')
        .removeClass('modal-open');
}

function createApplication() {
    closeModalCreate();
}

function deleteClick(event) {
    $(event.target)
        .closest('.accordion-item.application')
        .find('#');
}

async function copyClientId(event) {
    let inpt = $(event.target)
        .closest('.input-group')
        .find('input');
    inpt.focus;
    inpt.select();

    await navigator.clipboard.writeText(inpt.text());
}

async function getSecretClick(event) {
    let group = $(event.target)
        .closest('.input-group');

    let gpText = group.find('.input-group-text');
    gpText.off('click');
    gpText.html('<span class="spinner-border spinner-border-sm" role="status"/>');

    let clientId = group
        .closest('.application')
        .find('#ClientId')
        .val();

    let app = await getApplication(clientId);

    group
        .find('input')
        .val(app.secret);

    gpText
        .html('<i class="fa-solid fa-eye-slash"></i>');
    gpText
        .on('click', hideSecretClick);
}

function hideSecretClick(event) {
    let group = $(event.target)
        .closest('.input-group');

    let gpText = group
        .find('.input-group-text');

    group
        .find('input')
        .val(secret);

    gpText.html(faEye);
    gpText.off('click');
    gpText.on('click', getSecretClick);
}

async function getApplication(clientId) {
    let url = apiHost + 'Applications/ByClientId?client_id=' + encodeURIComponent(clientId);
    return await $.ajax({
        url: url,
        method: 'GET',
        xhrFields: {
            withCredentials: true
        }
    });
}

class Application {
    constructor(accordion, position = Number, id = Number, name, key, secret, status, description, image = NFile) {
        this.accordion = accordion;
        this.position = position;
        this.name = name;
        this.description = description;
        this.logo = image;
        this.key = key;
        this.secret = secret;
        this.id = id;
        this.status = status;
    }

    async collapse() {
        var item = $('<div class="accordion-item application"/>');

        var headerId = 'app' + this.position;
        var collId = 'coll' + this.position;

        var header = await this.collapseHeader(headerId, collId);
        var body = this.collapseBody(headerId, collId);

        item.data('id', this.id);
        item.append(header);
        item.append(body);

        return item;
    }

    async collapseHeader(headerId, collId) {
        let header = $('<h2 class="accordion-header" id="' + headerId + '">');
        let button = $('<button class="accordion-button" type="button" data-bs-toggle="collapse" data-bs-target="#' + collId + '" aria-expanded="true" aria-controls="' + collId + '">');
        let status = $('<span class="badge status" type="' + this.status.toLowerCase() + '">' + this.status + '</span>');

        button.html('<img/>' + this.name);
        button.find('img').attr('src', await this.logo.download());
        button.append(status);
        header.append(button);
        return header;
    }

    collapseBody(headerId, collId) {
        let coll = $('<div id="' + collId + '" class="accordion-collapse collapse" aria-labelledby="' + headerId + '" data-bs-parent="' + this.accordion + '"></div>');
        let body = $('<div class="accordion-body"/>');
        let btnDelete = $('<div class="delete content">' +
            '<button class="form-control" id="delete" type="button">' +
            '<i class="fa-solid fa-trash"/></button></div>');
        let description = $('<div class="description content">' +
            '<textarea class="form-control" disabled rows="8"/></div>');
        let inputs = this.keysBody();
        let status = this.statusBody();

        btnDelete
            .on('click', deleteClick);

        description
            .find('textarea')
            .text(this.description);

        body.append(description);
        body.append(inputs);
        body.append(status);
        body.append(btnDelete);
        coll.append(body);
        return coll;
    }

    keysBody() {
        let content = $('<div class="keys content">');

        let inClientId = $('<div class="form-group client-id">' +
            '<label class="control-label" for="ClientId">Client ID</label>' +
            '<div class="input-group mb-3">' +
            '<input type="text" class="form-control" name="ClientId" id="ClientId" disabled>' +
            '<span class="input-group-text"><i class="fa-solid fa-clipboard-list"/></span></div>');

        let inClientSecret = $('<div class="form-group secret">' +
            '<label class="control-label" for="ClientSecret">Secret</label>' +
            '<div class="input-group mb-3">' +
            '<input type="text" class="form-control" name="ClientSecret" id="ClientSecret" disabled>' +
            '<span class="input-group-text">' + faEye + '</span></div>');

        inClientId
            .find('.input-group-text')
            .on('click', copyClientId);

        inClientId
            .find('input')
            .val(this.key);

        inClientSecret
            .find('.input-group-text')
            .on('click', getSecretClick);

        inClientSecret
            .find('input')
            .val(this.secret);

        content.append(inClientId);
        content.append(inClientSecret);

        return content;
    }

    statusBody() {
        let spnStatus = $('<span class="badge status" type="' + this.status.toLowerCase() + '"></span>');
        let status = $('<div></li></div>');
        let popover = spnStatus.popover({
            html: true,
            placement: 'right',
            trigger: 'click',
            title: 'Set Status',
            content: '<div>' + status.html() + '</div>'
        });

        return spnStatus;
    }
}
