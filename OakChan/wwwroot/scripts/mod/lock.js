$(document).ready(() => {
    let level = $('#accountLevel').val();
    if (level > 3) { //moderators and admin
        window.PostMenu.addItem(
            getLocalizedString('lock_thread'),
            info => lockThread(info.board, info.thread, true),
            (info, post) =>
                $(post).hasClass('oppost') &&
                $(`#thread-${info.thread} .lock-icon`).length == 0
        );

        window.PostMenu.addItem(
            getLocalizedString('unlock_thread'),
            info => lockThread(info.board, info.thread, false),
            (info, post) =>
                $(post).hasClass('oppost') &&
                $(`#thread-${info.thread} .lock-icon`).length > 0
        );
    }
})

function lockThread(board, thread, lock) {
    const controller = new AbortController();
    const timeoutId = setTimeout(() => controller.abort(), 5000)
    const uri = `/${board}/${thread}/LockThread`;
    let antiforgery = $("input[name='__RequestVerificationToken']").val();
    return fetch(uri, {
        signal: timeoutId.signal,
        method: "POST",
        headers: {
            "Content-Type": "application/x-www-form-urlencoded",
            "RequestVerificationToken": antiforgery
        },
        body: `lock=${lock}`
    }).then(() => window.location.reload());
}