$(document).ready(() => {
    let level = $('#accountLevel').val();
    if (level > 3) { //moderators and admin
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
    const uri = `/${board}/${thread}/PinThread`;
    let antiforgery = $("input[name='__RequestVerificationToken']").val();
    return fetch(uri, {
        signal: timeoutId.signal,
        method: "POST",
        headers: {
            "Content-Type": "application/x-www-form-urlencoded",
            "RequestVerificationToken": antiforgery
        },
        body: `pin=${pin}`
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