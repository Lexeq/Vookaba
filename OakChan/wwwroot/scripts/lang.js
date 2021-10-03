//TODO: use local storage?
//TODO: detect locale
//TODO: store lang pack on server in static json?
window.settings = {};
window.settings.locale = 'ru';

const localizedStrings = {
    en: { show: "Show", hide: "Hide" },
    ru: { show: "Показать", hide: "Скрыть" }
}

function getLocalizedString(str) {
    return localizedStrings[window.settings.locale][str] || str;
}