$(document).ready(ToClientTime);

function ToClientTime() {
    let list = $('.post__date');
    for (let item of list) {
        $(item).text(FormatTimeOffset(Number.parseInt(item.dataset.timestamp)));
    }

    //Todo date format
    function FormatTimeOffset(milliseconds) {
        let padNum = (str) => { return str.toString().length < 2 ? "0" + str : str; };
        let days = localizedStrings.daysShot;
        let d = new Date(milliseconds);
        return `${days[d.getDay()]} ${padNum(d.getDate())}/${padNum(d.getMonth() + 1)}/${padNum(d.getFullYear() % 100)}`
            + ` ${padNum(d.getHours())}:${padNum(d.getMinutes())}:${padNum(d.getSeconds())}`;
    }
}
