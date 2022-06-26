function textEditorAddTag(tag1, tag2) {
    let textArea = $('#message-text')[0];
    let text = textArea.value;
    let start = textArea.selectionStart;
    let end = textArea.selectionEnd;
    text = text.slice(0, start) +
        tag1 +
        text.slice(start, end) +
        tag2 +
        text.slice(end);
    textArea.value = text;
    textArea.focus();
    textArea.selectionStart = start + tag1.length;
    textArea.selectionEnd = end + tag1.length;
}