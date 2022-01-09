$(document).ready(() => {
    let notifier = $("<div>");
    notifier.prop("id", "notifier")
    notifier.addClass('notifier');
    $('body').append(notifier);
});

function showNotification(text) {
    let note = $("<div>")
        .addClass("notification")
        .append(text);

    $('#notifier').append(note);
    setTimeout(() => {
        note.remove();
    }, 5000);
}