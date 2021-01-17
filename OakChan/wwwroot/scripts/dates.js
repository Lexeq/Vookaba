ToClientTime();

function ToClientTime() {
    let list = document.getElementsByClassName('post__date');
    for (let item of list) {
        item.textContent = FormatTimeOffset(Number.parseInt(item.dataset.timestamp));
    }

    function FormatTimeOffset(milliseconds) {
        let padNum = (str) => { return str.toString().length < 2 ? "0" + str : str; };
        let days = ['Вс', 'Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб'];
        let d = new Date(milliseconds);
        return `${days[d.getDay()]} ${padNum(d.getDate())}/${padNum(d.getMonth() + 1)}/${padNum(d.getFullYear() % 100)}`
            + ` ${padNum(d.getHours())}:${padNum(d.getMinutes())}:${padNum(d.getSeconds())}`;
    }
}
