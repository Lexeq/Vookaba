//TODO: use local storage?
//TODO: detect locale
//TODO: store lang pack on server in static json?
window.settings = {};
window.settings.locale = 'ru';

const localizedStrings = {
    en: {
        show: "Show",
        hide: "Hide",
        del_post: "Delete",
        reason: "Enter the reason and confirm the deletion",
        cancel: "Cancel",
        reasonRequired: "Reason is required",
        modeRequired: "Select deleting mode",
        ok: "OK",
        delSinglePost: "only this post",
        delInThread: "all author's posts in the thread",
        delInCategory: "all author's posts across the board",
        delAll: "all author's posts across all boards",
        delByIp: "with same IP",
        delById: "with same ID"
    },
    ru: {
        show: "Показать",
        hide: "Скрыть",
        del_post: "Удалить",
        reason: "Введите причину и подтвердите удаление",
        cancel: "Отмена",
        reasonRequired: "Необходимо ввести причину удаления",
        modeRequired: "Выберите режим удаления постов",
        ok: "OK",
        delSinglePost: "только этот пост",
        delInThread: "все посты автора в треде",
        delInCategory: "все посты автора на доске",
        delAll: "все посты автора",
        delByIp: "искать по IP",
        delById: "искать по ID"
    }
}

function getLocalizedString(str) {
    return localizedStrings[window.settings.locale][str] || str;
}