$(document).ready(() => {
    let level = $('#accountLevel').val();
    window.PostMenu.addItem(
        getLocalizedString('del_post'),
        data => showDeletingModal(data.board, data.number, level))
})

function deletePosts(options) {
    console.info(JSON.stringify(options));
    const controller = new AbortController();
    const timeoutId = setTimeout(() => controller.abort(), 5000)
    const uri = '/api/v1/posts';
    let antiforgery = $("input[name='__RequestVerificationToken']").val();
    return fetch(uri, {
        signal: controller.signal,
        method: "DELETE",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json",
            "RequestVerificationToken": antiforgery
        },
        body: JSON.stringify(options)
    });
}

function showDeletingModal(board, number, level) {
    let content = $('<form id="delForm">')
        .css('min-width', '250px');

    content.append([
        $(`<input type="hidden" name="board" value="${board}">`),
        $(`<input type="hidden" name="number" value="${number}">`),
        $('<label class="input-row__label" for="tb-reason">')
            .append(getLocalizedString('reason')),
        $('<input id="tb-reason" name="reason" autocomplete="off" class="input-row__input">'),
        createDeletionOptionsContent(level)
    ]);
    if (level >= ModeratorLevel) {
        content.append(
            $('<div>').append(createCheckbox('cbAddBan', 'cbAddBan', getLocalizedString('Ban for this post'))),
            createBanOptionsContent(level));

        let ipRow = $('<div>').append(createCheckbox('byIp', "ipMode", getLocalizedString('delByIp'), 2));
        content.append(ipRow);
    }
    content.append([
        $('<label id="fromErrors" class="error-text">')
            .css('display', 'none'),
    ]);
    content.on('click', '#cbAddBan', (a) => {
        let b = $('#banOpts');
        b.css('display', a.target.checked ? 'block' : 'none');
    });

    showModal(content, okCallback);
    content.find('input:not(:hidden)')[0].focus()
}

function createDeletionOptionsContent(accountLevel) {
    let delOpts;
    if (accountLevel >= JanitorLevel && accountLevel < ModeratorLevel) {
        delOpts = $('<input type=hidden name=area value=1>')
    }
    if (accountLevel >= ModeratorLevel) {
        delOpts = $('<fieldset>');
        delOpts.append($('<legend>').append(getLocalizedString('postDelOpts')));
        delOpts.append(createRadioRow('rd', 'area', getLocalizedString('delSinglePost'), 1, true));
        delOpts.append(createRadioRow('rdt', 'area', getLocalizedString('delInThread'), 2));
        delOpts.append(createRadioRow('rdc', 'area', getLocalizedString('delInCategory'), 3));
    }
    if (accountLevel >= AdminLevel) {
        delOpts.append(createRadioRow('rda', 'area', getLocalizedString('delAll'), 4));
    }

    return delOpts;
}

function createBanOptionsContent(level) {
    let banOpts = $('<fieldset id="banOpts" style="display: none;">');
    banOpts.append($('<legend>').append(getLocalizedString('postBanOpts')));

    banOpts.append([
        $('<label class="input-row__label" for="ban-dur">')
            .append(getLocalizedString('postBanDuration')),
        $('<input id="ban-dur" name="banDuration" autocomplete="off" class="input-row__input" placeholder="1y2d3h4m">')]);

    if (level >= AdminLevel) {
        banOpts.append(createCheckbox('cbAllbrd', 'allBoards', getLocalizedString('All boards')));
    }
    return banOpts;
}

function createRadioRow(id, name, text, value, checked) {
    return $('<div>')
        .append(createRadio(id, name, text, value, checked));
}

function createRadio(id, name, text, value, checked) {
    return [
        $("<input>")
            .attr('type', 'radio')
            .attr('id', id)
            .attr('name', name)
            .attr('value', value)
            .attr('checked', checked),
        $('<label>')
            .attr('for', id)
            .append(text)];
}

function createCheckbox(id, name, text, value, checked) {
    return [
        $("<input>")
            .attr('type', 'checkbox')
            .attr('id', id)
            .attr('name', name)
            .attr('value', value)
            .attr('checked', checked),
        $('<label>')
            .attr('for', id)
            .append(text)];
}

function okCallback() {
    $('#fromErrors').css('display', 'none');
    let delForm = new FormData($('#delForm')[0]);
    let reason = delForm.get('reason');
    if (reason?.length < 1) {
        $('#fromErrors')
            .css('display', 'block')
            .text(getLocalizedString('reasonRequired'));
        return Promise.resolve(false);
    }

    let area = parseInt(delForm.get('area'));

    if (!area || area < 1 || area > 4) {
        throw `bad area (${area})`;
    }

    let options = {
        reason: reason,
        board: delForm.get('board'),
        number: parseInt(delForm.get('number')),
        area: area,
        ipMode: delForm.get('ipMode') ? true : false
    };

    if (delForm.get('cbAddBan') == 'on') {
        let duration = delForm.get('banDuration');
        if (duration?.length < 2) {
            $('#fromErrors')
                .css('display', 'block')
                .text(getLocalizedString('banDurationRequired'));
            return Promise.resolve(false);
        }
        let banOptions = {
            duration: duration,
            allBoards: delForm.get('allBoards') ? true : false
        };

        options.ban = banOptions;
    }

    return deletePosts(options)
        .then(response => {
            if (response.ok) {
                return response;
            } else {
                throw new Error(response.statusText);
            }
        })
        .then(_ => {
            if (area == 1 && !$('#p' + options.number).hasClass('oppost')) {
                $('#pc' + options.number).remove();
                showNotification(getLocalizedString('postDeleted'));
            }
            else {
                location.reload();
            }
        })
        .catch(er => {
            showNotification(er, true);
            console.error(er);
        })
}