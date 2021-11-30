'use strict';

function setPostIsHidden(data, isHidden) {

    let bid = data.board;
    let tid = data.thread;
    let num = data.number;

    let hiddens = JSON.parse(localStorage.getItem('ignoring'));
    updatePostVisibility(num, isHidden);
    if (isHidden) {
        if (!hiddens) {
            hiddens = {};
        }

        if (!hiddens[bid]) {
            hiddens[bid] = {};
        }
        if (!hiddens[bid][tid]) {
            hiddens[bid][tid] = {};
        }
        hiddens[bid][tid][num] = Date.now();
    }
    else {
        delete hiddens[bid][tid][num];
        if (!Object.keys(hiddens[bid][tid]).length) {
            delete hiddens[bid][tid];
        }
        if (!Object.keys(hiddens[bid]).length) {
            delete hiddens[bid];
        }
    }
    localStorage.setItem('ignoring', JSON.stringify(hiddens));
}

//wow such a great function's name
function hideHidden() {
    let hidden = JSON.parse(localStorage.getItem('ignoring'));
    $('.thread').each(function () {
        let bid = $(this).data('bid');
        let tid = $(this).data('tid');
        if (hidden?.[bid]?.[tid]) {
            for (const pnum in hidden[bid][tid]) {
                updatePostVisibility(pnum, true);
            }
        }
    });
}

function updatePostVisibility(number, isHidden) {
    let $post = $('#p' + number);
    if (isHidden) {
        $post.addClass('post_hidden');
        if ($post.hasClass('oppost')) {
            $post.closest('.thread').addClass('thread_hidden');
        }
    }
    else {
        $post.removeClass('post_hidden');
        if ($post.hasClass('oppost')) {
            $post.closest('.thread').removeClass('thread_hidden');
        }
    }
}

$(document).ready(() => {
    hideHidden();
    window.PostMenu.addItem(
        getLocalizedString('hide'),
        post => setPostIsHidden(post, true),
        post => { return !$(post).hasClass('post_hidden'); });
    window.PostMenu.addItem(
        getLocalizedString('show'),
        post => setPostIsHidden(post, false),
        post => { return $(post).hasClass('post_hidden'); });
});