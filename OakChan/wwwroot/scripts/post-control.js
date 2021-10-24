'use strict';
hideHidden();
$('.thread').on('click', ".post__menu-button", showMenu);
$(document).click(hideMenu);

function showMenu() {
    var oldPid = $('#post-menu').data('pid');
    hideMenu();
    let pid = $(this).data('pid');
    if (oldPid == pid) {
        return;
    }

    let $menu = $("<div>");
    $menu.attr('id', 'post-menu');
    $menu.addClass('post-menu');
    $menu.data('pid', pid);
    $menu.data('pnum', $(this).data('pnum'));
    $menu.css(generatePosition($(this)))

    fillMenu($menu, pid);

    $('body').append($menu);
    $(this).addClass('post__menu-button_opened');
    return false;
}

function generatePosition($el) {
    let ret = {};
    let elPos = $el.offset();

    ret.left = (elPos.left + $el.outerWidth()) + 'px';
    ret.top = (elPos.top + $el.outerHeight()) + 'px';

    return ret;
}


function fillMenu($menu) {
    let $root = $('<ul>');

    //hide/show post
    let $post = $('#p' + $menu.data('pnum'));
    let isHidden = $post.hasClass('post_hidden');
    let $hiddingSwitchRow = $(`<li>${getLocalizedString(isHidden ? 'show' : 'hide')}</li>`);
    $hiddingSwitchRow.click(() => setPostIsHidden($post, !isHidden));
    $root.append($hiddingSwitchRow);

    $menu.append($root);
}

function hideMenu() {
    let $menu = $('#post-menu');
    let btnSelector = '#mb' + $menu.data('pid');
    $(btnSelector).removeClass('post__menu-button_opened');
    $menu.remove();
}

function setPostIsHidden($post, isHidden) {
    let bid = $post.closest('.thread').data('bid');
    let tid = $post.closest('.thread').data('tid');
    let pnum = $post.find('.post__number').data('pnum');

    let hiddens = JSON.parse(localStorage.getItem('ignoring'));
    updatePostVisibility($post, isHidden);
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
        hiddens[bid][tid][pnum] = Date.now();
    }
    else {
        delete hiddens[bid][tid][pnum];
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
                let $post = $('#p' + pnum);
                updatePostVisibility($post, true);
            }
        }
    });
}

function updatePostVisibility($post, isHidden) {
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