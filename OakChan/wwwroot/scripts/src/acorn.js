$(document).ready(() => {
    subscribeReplyOnClick();
    buildRefMap();
    if ($('#attachment-input').val()) {
        loadImagePreview($('#attachment-input').prop('files'));
    }
    $('.post__file-link img').on('load', (x) => imageLoaded(x.target));
    $('.full-size').on('click', (x) => toggleFullSize(x.target));
    $('.post__file-link').on('click', (x) => { x.preventDefault(); toggleExpanded(x.target); });
})

function imageLoaded(img) {
    img.classList.remove('loading');
    let setNaturalSize = true;
    let container = img.closest('.post__image-container');
    if (container.classList.contains('full')) {
        img.closest('.post').style.maxWidth = 'unset';
    }
    else if (container.classList.contains('expanded')) {
        let body = document.getElementsByTagName('body')[0];
        img.closest('.post').style.maxWidth = 'unset';

        let w = img.naturalWidth;
        let h = img.naturalHeight;
        let bw = body.clientWidth * 0.9;
        let bh = window.innerHeight * 0.9;

        if (w > bw || h > bh) {

            let rw = bw / w;
            let rh = bh / h;
            let shrinkRatio = Math.min(rw, rh);

            img.width = img.naturalWidth * shrinkRatio;
            img.height = img.naturalHeight * shrinkRatio;
            setNaturalSize = false;
        }
    }
    else {
        img.closest('.post').style.maxWidth = '';
    }
    if (setNaturalSize) {
        img.width = img.naturalWidth;
        img.height = img.naturalHeight;
    }
}

function toggleFullSize(btn) {
    let id = btn.id.substring(5);
    let img = document.getElementById('img' + id);
    if (img.classList.contains('loading')) {
        return;
    }
    else {
        img.classList.add('loading');
    }
    let container = img.closest('.post__image-container');
    img.removeAttribute('loading')
    container.classList.toggle('full');
    img.src = selectImgSrc(img, container);
}

function toggleExpanded(img) {
    if (img.classList.contains('loading')) {
        return;
    }
    else {
        img.classList.add('loading');
    }
    let container = img.parentNode.parentNode;
    if (container.classList.contains('full')) {
        container.classList.remove('full');
        container.classList.remove('expanded')
    }
    else {
        container.classList.toggle('expanded');
    }
    img.removeAttribute('loading')
    img.src = selectImgSrc(img, container);
}

function selectImgSrc(img, container) {
    if (container.classList.contains('full')) {
        return img.parentNode.href;
    }
    if (container.classList.contains('expanded')) {
        return img.parentNode.href;
    }
    else {
        return img.dataset.thumb;
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
    if ($('#posting-form').css('display') == 'none') {
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

$('#brd-delete').click((e) => {

    if (confirm(getLocalizedString('brdDeleteConfirmMessage')) !== true) {
        e.preventDefault();
    }
})