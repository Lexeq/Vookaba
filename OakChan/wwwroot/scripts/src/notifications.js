$(document).ready(() => {
    let notifier = $("<div>");
    notifier.prop("id", "notifier")
    notifier.addClass('notifier');
    $('body').append(notifier);
});

function showNotification(text, isError) {
    let note = $("<div>")
        .addClass("notification")
        .append(text);

    if (isError === true) {
        note.addClass('notification_error');
    }

    $('#notifier').append(note);
    setTimeout(() => {
        note.remove();
    }, 5000);
}