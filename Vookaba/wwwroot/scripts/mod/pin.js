$(document).ready(() => {
    let level = $('#accountLevel').val();
    if (level >= ModeratorLevel) {
        window.PostMenu.addItem(
            getLocalizedString('pin_thread'),
            info => pinThread(info.board, info.thread, true),
            (info, post) =>
                $(post).hasClass('oppost') &&
                $(`#thread-${info.thread} .pin-icon`).length == 0
        );

        window.PostMenu.addItem(
            getLocalizedString('unpin_thread'),
            info => pinThread(info.board, info.thread, false),
            (info, post) =>
                $(post).hasClass('oppost') &&
                $(`#thread-${info.thread} .pin-icon`).length > 0
        );
    }
})

function pinThread(board, thread, pin) {
    const controller = new AbortController();
    const timeoutId = setTimeout(() => controller.abort(), 5000)
    const uri = `/api/v1/threads/${pin ? 'pin' : 'unpin'}?board=${board}&thread=${thread}`;
    return fetch(uri, {
        signal: controller.signal,
        method: "POST"
    })
        .then(response => {
            if (response.status >= 200 && response.status < 300) {
                window.location.reload();
            } else {
                let error = new Error(response.statusText);
                error.response = response;
                throw error
            }
        })
        .catch(er => showNotification(er, true));
}