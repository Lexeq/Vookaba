'use strict';
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

    $menu.append($root);
}

function hideMenu() {
    let $menu = $('#post-menu');
    let btnSelector = '#mb' + $menu.data('pid');
    $(btnSelector).removeClass('post__menu-button_opened');
    $menu.remove();
}
