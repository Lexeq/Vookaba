function showModal(content, okCallback) {
    let modal = $('<div>')
        .attr('id', 'modal')
        .addClass('modal')
        .on('click', (e) => {
            if (e.target == $('#modal')[0]) {
                $('#modal-cancel').click();
            }
        })
        .keydown((e) => {
            if (e.key === 'Escape') {
                $('#modal-cancel').click();
            }
            else if (e.key === 'Enter') {
                $('#modal-ok').click();
            }
        });

    let contentContainer = $('<div>')
        .addClass('modal-content')
        .attr('id', 'modal-content');

    let btnRow = $('<div>')
        .addClass('button-row')
        //Ok button
        .append($('<button>')
            .attr('id', 'modal-ok')
            .append(getLocalizedString('ok'))
            .addClass('oakbutton')
            .on('click', () => {
                if ($('#modal-ok').prop('disabled')) {
                    return;
                }
                setModalButtonsDisabled(true);
                Promise.resolve(okCallback())
                    .then(x => { if (x !== false) { removeModal(); } })
                    .finally(_ => setModalButtonsDisabled(false));
            }))
        //Cancel button
        .append($('<button>')
            .attr('id', 'modal-cancel')
            .append(getLocalizedString('cancel'))
            .addClass('oakbutton')
            .on('click', () => {
                if ($('#modal-cancel').prop('disabled')) {
                    return;
                }
                removeModal();
            }));

    contentContainer.append(content);
    contentContainer.append(btnRow);

    modal.append(contentContainer);

    $('body').prepend(modal);
    return modal;
}

function removeModal() {
    $('#modal').remove();
}

function setModalButtonsDisabled(disabled) {
    $('#modal-cancel, #modal-ok').prop('disabled', disabled);
}
