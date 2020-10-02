window.onload = () => {
    markOpPosts();
    subscribeImageOnClick();
    subscribeFullSizeOnClick();
    subscribeReplyOnClick();
}

function subscribeFullSizeOnClick() {
    function toFullSize(e) {
        let container = e.target.parentNode.parentNode.parentNode;
        let imgId = 'img' + container.id.substring(2);
        let img = document.getElementById(imgId);
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

function markOpPosts() {
    var threads = document.getElementsByClassName("thread");
    for (let i = 0; i < threads.length; i++) {
        threads[i].children[0].children[0].classList.add("op-post");
    }
}

function subscribeReplyOnClick() {
    let elms = document.getElementsByClassName('post__number');
    for (let i = 0; i < elms.length; i++) {
        elms[i].onclick = () => reply(elms[i]);
    }
}

function reply(e) {
    let input = document.getElementById('form-msg');
    let text = '>>' + e.dataset.pid + ' ';
    let sel = window.getSelection();

    if (sel && sel.anchorNode == sel.focusNode && sel.anchorNode == document.getElementById("m" + e.dataset.pid).childNodes[1].firstChild) {
        text += document.getSelection();
    }
    text += '\n';
    if (document.getElementById('form').style.display == '') {
        switchFormVisibility();
    }
    input.value += text;
    input.focus();
}

function loadImagePreview(files) {
    let img = document.getElementById("preview-img");
    if (files && files[0]) {
        let reader = new FileReader();
        reader.onload = function (e) {
            img.src = e.target.result;
            img.style.display = "block";
        }
        reader.readAsDataURL(files[0]);
    }
    else {
        img.src = "#";
        img.style.display = '';
    }
}

function switchFormVisibility() {
    let form = document.getElementById("form");
    form.style.display = form.style.display ? '' : 'block';
    let switcher = document.getElementById("switcher");
    let newText = switcher.dataset.switchedText;
    switcher.dataset.switchedText = switcher.textContent;
    switcher.textContent = newText;
}