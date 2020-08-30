window.onload = () => {
    subscribeImageOnClick();
    subscribeReplyOnClick();
    markOpPosts();
}

function subscribeImageOnClick() {
    var list = document.getElementsByClassName('post__image');
    for (let item of list) {
        item.onclick = () => toggleImageSIze(item);
    }
}

function toggleImageSIze(el) {
    const expandedClassName = "post__image--expanded";
    if (el.classList.contains(expandedClassName)) {
        el.classList.remove(expandedClassName);
    } else {
        el.classList.add(expandedClassName);
    }
}

function markOpPosts() {
    var threads = document.getElementsByClassName("thread");
    for (let i = 0; i < threads.length; i++) {
        threads[i].children[0].classList.add("op-post");
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
    input.value += '>>' + e.dataset.pid;
    let sel = window.getSelection();
    if (sel && sel.anchorNode == sel.focusNode && sel.anchorNode == document.getElementById("msg-" + e.dataset.pid).firstChild) {
        input.value += document.getSelection();
    }
    input.value += '\n';
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
}