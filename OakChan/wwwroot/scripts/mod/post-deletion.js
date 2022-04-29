$(document).ready(() => {
    let level = $('#accountLevel').val();
    window.PostMenu.addItem(
        getLocalizedString('del_post'),
        data => showDeletingModal(data.board, data.number, level))
})

function deletePosts(board, number, reason, area, mode) {
    const controller = new AbortController();
    const timeoutId = setTimeout(() => controller.abort(), 5000)

    const uri = `/${board}/BulkDeletePosts`;
    let antiforgery = $("input[name='__RequestVerificationToken']").val();
    return fetch(uri, {
        signal: timeoutId.signal,
        method: "POST",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json",
            "RequestVerificationToken": antiforgery
        },
        body: JSON.stringify({ number: number, reason: reason, area: area, mode: mode })
    });
}

//level:    0 - single
//          1 - in thread
//          2 - acrss the board
//          3 - across all boards
function showDeletingModal(board, num, level) {
    let content = $('<div>')
        .css('min-width', '250px')
        .attr('id', 'del-opts');

    if (level > 0) {
        content.append(createRadioRow('rd', 'rbdel', getLocalizedString('delSinglePost'), true)
            .attr('data-noopt', 1));
    }
    if (level > 1) {
        content.append(createRadioRow('rdt', 'rbdel', getLocalizedString('delInThread')));
    }
    if (level > 2) {
        content.append(createRadioRow('rdc', 'rbdel', getLocalizedString('delInCategory')));
    }
    if (level > 3) {
        content.append(createRadioRow('rda', 'rbdel', getLocalizedString('delAll')));
    }


    content.append([
        $('<label>')
            .addClass('input-row__label')
            .append(getLocalizedString('reason'))
            .attr('for', 'del-reason'),
        $('<input>')
            .attr('id', 'del-reason')
            .attr('autocomplete', 'off')
            .addClass('input-row__input'),
        $('<label>')
            .attr('id', 'del-error')
            .addClass('error-text')
            .css('display', 'none'),
        createModeSelector()
    ]);

    content.on('click', 'input:radio[name=rbdel]', radioClick);

    showModal(content, () => {
        $('#del-error').css('display', 'none');
        let reason = $('#del-reason').val();
        if (reason?.length < 1) {
            $('#del-error')
                .css('display', 'block')
                .text(getLocalizedString('reasonRequired'));
            return Promise.resolve(false);
        }
        let checked = $('input[name=rbdel]:checked', '#del-opts').val();

        let area = null;
        switch (checked) {
            case 'rd':
                area = 0;
                break;
            case 'rdt':
                area = 1;
                break;
            case 'rdc':
                area = 2;
                break;
            case 'rda':
                area = 3;
                break;
            default:
                throw 'bad area';
        }

        let mode = 0;
        if ($('#byIp').prop('checked')) {
            mode += 1;
        }
        if ($('#byId').prop('checked')) {
            mode += 2;
        }
        if (area != 0 && mode == 0) {
            $('#del-error')
                .css('display', 'block')
                .text(getLocalizedString('modeRequired'));
            return Promise.resolve(false);
        }

        return deletePosts(board, num, reason, area, mode)
            .then(response => {
                if (response.status >= 200 && response.status < 300) {
                    return response;
                } else {
                    let error = new Error(response.statusText);
                    error.response = response;
                    throw error
                }
            })
            .then(response => {
                if (area == 0 && !$('#p' + num).hasClass('oppost')) {
                    $('#pc' + num).remove();
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
    });
    $('#del-reason').focus();
}

function radioClick(rb) {
    if ($(this).parent().data('noopt')) {
        $('#umode').css('display', 'none');
    }
    else {
        let um = $('#umode');
        um.css('display', '');
        um.insertAfter($(this).next());
    }
}

function createModeSelector() {
    let mode = $('<div>').attr('id', 'umode');
    mode.append(createCheckbox('byId', getLocalizedString('delById'), true))
        .append(createCheckbox('byIp', getLocalizedString('delByIp')))
        .css('display','none');

    return mode;
}

function createRadioRow(id, name, text, checked) {
    return $('<div>')
        .append(createRadio(id, name, text, checked));
}

function createRadio(id, name, text, checked) {
    return [
        $("<input>")
            .attr('type', 'radio')
            .attr('id', id)
            .attr('name', name)
            .attr('value', id)
            .attr('checked', checked),
        $('<label>')
            .attr('for', id)
            .append(text)];
}

function createCheckbox(id, text, checked) {
    return [
        $("<input>")
            .attr('type', 'checkbox')
            .attr('id', id)
            .attr('checked', checked),
        $('<label>')
            .attr('for', id)
            .append(text)];
}