$(document).ready(_ => {
    if ($('.threads-container').length > 0) {
        return;
    }
    let details = $('body > div > .thread .post__details',);
    for (let i = 0; i < details.length;) {
        $(details[i]).prepend($(`<span>${++i}</span>`).addClass('numbering'));
    }
});