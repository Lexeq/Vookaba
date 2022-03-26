'use strict';

class PostMenu {
    constructor() {
        this.items = [];
    }

    /**
     * Add new item to menu.
     * @param {String}      text    text for item 
     * @param {Function}    action  action when clicking on item
     * @param {Function}    filter  A function that accepts a post and is called every time the menu is opened.
     * If it returns false, then the item is not displayed
     * @param {String}      title   tooltip for item
     */
    addItem(text, action, filter, title) {
        this.items.push({ text, title, action, filter });
    }

    show(button) {
        let generatePosition = function ($parent) {
            let position = {};
            let offset = $parent.offset();

            position.left = (offset.left + $parent.outerWidth()) + 'px';
            position.top = (offset.top + $parent.outerHeight()) + 'px';

            return position;
        }

        let fillMenu = function ($root, $post, items) {
            let postInfo = {};
            postInfo.board = $post.closest('.thread').data('bid');
            postInfo.thread = $post.closest('.thread').data('tid');
            postInfo.number = $post.find('.post__number').data('pnum');

            for (let e of items) {
                if (!e.filter || e.filter(postInfo, $post) !== false) {
                    let row = $(`<li>${e.text}</li>`)
                        .prop('title', e.title)
                        .click(() => e.action(postInfo, $post));
                    $root.append(row);
                }
            }

            return;
        }

        let $menu = $("<div id='post-menu'><ul>")
            .addClass('post-menu')
            .css(generatePosition($(button)));

        fillMenu($menu.find('ul'), $(button).closest('.post'), this.items);

        $('body').append($menu);
        $(button).addClass('post__menu-button_opened');
        return false;
    }

    remove() {
        $('#post-menu').remove();
        $('.post__menu-button_opened').removeClass('post__menu-button_opened');
    }
}

$(document).ready(function () {
    window.PostMenu = new PostMenu();
    $(document).click(function (e) {
        if ($(e.target).hasClass('post__menu-button')) {
            e.preventDefault();
            if (!$(e.target).hasClass('post__menu-button_opened')) {
                if ($('#post-menu').length) {
                    window.PostMenu.remove();
                }
                window.PostMenu.show(e.target);
                return;
            }
        }

        window.PostMenu.remove();
    });

});

