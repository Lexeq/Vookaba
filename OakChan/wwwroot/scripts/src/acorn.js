$(document).ready(() => {
    subscribeImageOnClick();
    subscribeFullSizeOnClick();
    subscribeReplyOnClick();
    buildRefMap();
    if ($('#attachment-input').val()) {
        loadImagePreview($('#attachment-input').prop('files'));
    }
})

function subscribeFullSizeOnClick() {
    function toFullSize(e) {
        let id = e.target.id.substring(5);
        let container = document.getElementById('ic' + id);
        let img = document.getElementById('img' + id);
        img.style.maxHeight = "";
        img.src = img.parentNode.href;
        container.style.maxWidth = img.dataset.imgWidth + "px";
        container.style.width = img.dataset.imgWidth + "px";
    }

    let list = document.getElementsByClassName('full-size');
    for (let item of list) {
        item.addEventListener('click', toFullSize, false);
    }
}

function subscribeImageOnClick() {

    function imLinkClick(e) {
        e.preventDefault();

        let img = e.target;
        let container = img.parentNode.parentNode;

        container.style.width = "";
        if (img.src.endsWith(img.dataset.imgThumb)) {
            container.style.maxWidth = "100vw";
            img.style.maxWidth = "100%";
            img.style.maxHeight = "90vh";
            img.src = img.parentNode.href;
        }
        else {
            container.style.maxWidth = "";
            img.style.maxWidth = "";
            img.style.maxWidth = "";
            img.style.maxHeight = "";
            img.src = img.dataset.imgThumb;
        }
    }

    let list = document.getElementsByClassName('post__file-link');
    for (let item of list) {
        item.addEventListener('click', imLinkClick, false);
    }
}

function subscribeReplyOnClick() {
    let elms = document.getElementsByClassName('post__number');
    for (let postNumber of elms) {
        let container = postNumber.closest('.threads-container');
        if (!container) {
            postNumber.onclick = () => {
                reply(postNumber.dataset.pnum);
                return false;
            }
        }
    }
}

function reply(postNumber) {
    let input = document.getElementById('message-text');
    let text = '>>' + postNumber + ' ';
    let sel = document.getSelection();

    let id1 = $(sel.anchorNode).closest('.post__message').attr('id');
    let id2 = $(sel.focusNode).closest('.post__message').attr('id');

    if (!sel.isCollapsed && id1 == id2 && id1 == "m" + postNumber) {
        text += '\n>' + sel;
    }
    text += '\n';
    if ($('#form').css('display') == 'none') {
        switchFormVisibility();
    }
    input.value += text;
    input.focus();
}

function loadImagePreview(files) {
    let img = document.getElementById("attachment-thumbnail");
    if (files && files[0]) {
        let reader = new FileReader();
        reader.onload = function (e) {
            img.src = e.target.result;
        }
        reader.readAsDataURL(files[0]);
    }
    else {
        clearImagePreview();
    }
}

function attachmentLoaded(img) {
    $('#attachment-selector').hide();
    let shrinkRatio = Math.max(img.naturalHeight / 130, img.naturalWidth / 100);
    $('#attachment-info')
        .width(img.naturalWidth / shrinkRatio)
        .height(img.naturalHeight / shrinkRatio)
        .show();
    $('#attachment-resolution').text(img.naturalWidth + "x" + img.naturalHeight);
    let fileSize = $('#attachment-input').prop('files')[0].size;
    $('#attachment-size').text(Math.ceil(fileSize / 1024) + getLocalizedString('KB'));

}

function attachmentLoadFailed() {
    clearAttachment();
    showNotification(getLocalizedString('Bad image.'), true);
}

function clearAttachment() {
    $('#attachment-selector').show();
    $('#attachment-input').val('');
    $('#attachment-thumbnail').removeAttr('src');
    $('#attachment-resolution').text('');
    $('#attachment-size').text('');
    $('#attachment-info').hide();
}

function switchFormVisibility() {
    let form = document.getElementById("posting-form");
    form.style.display = form.style.display ? '' : 'none';
    let switcher = document.getElementById("switcher");
    let newText = switcher.dataset.switchedText;
    switcher.dataset.switchedText = switcher.textContent;
    switcher.textContent = newText;
}

function buildRefMap() {
    const maxRefs = 30;
    let posts = document.getElementsByClassName("post");
    for (let post of posts) {
        let id = post.id.substring(1);
        let msg = document.getElementById("m" + id);
        let pattern = /&gt;&gt;(\d+)/g;
        let newHtml = msg.innerHTML;
        let refCount = 0;
        let replaced = [];
        let match;
        while ((match = pattern.exec(msg.innerHTML)) && refCount < maxRefs) {
            refCount++;
            if (replaced.includes(match[1])) {
                continue;
            }
            replaced.push(match[1]);

            let rid = match[1]; //id поста на который ведет ссыла
            let refPost = document.getElementById('p' + rid);
            if (refPost) {
                //добавляем negative-lookahead что-бы при совпадении >>1, не заменялись >>11, >>15, >>111 и т.д.
                let replaceRegexp = new RegExp(match[0] + '(?!\\d)', 'g');
                newHtml = newHtml.replaceAll(replaceRegexp, `<a class="ref-post" href="#p${rid}">${match[0]}</a>`);
                let refmap = document.getElementById('refmap' + rid);
                //создать список ссылок, если еще не создана
                if (!refmap) {
                    refmap = document.createElement('div');
                    refmap.id = 'refmap' + rid;
                    refmap.classList.add('refmap-body');
                    document.getElementById('m' + rid).after(refmap);
                }
                //добавить ссылку в список
                var sp = document.createElement('span');
                sp.classList.add('refmap-span');
                sp.innerHTML = `<a class="ref-post" href="#p${id}">>>${id}</a>`;
                refmap.appendChild(sp);
            }
        }
        msg.innerHTML = newHtml;
    }
}