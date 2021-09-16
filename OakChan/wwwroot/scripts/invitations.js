document.forms["invForm"].addEventListener("submit", e => {
    e.preventDefault();
    const form = document.forms["invForm"];
    document.getElementById("inv-error").innerHTML = "";
    document.getElementById("inv-error").style.display = "none";
    createInvitation(form.elements["expDays"].value);
});

function createInvitation(days) {
    const uri = '/administration/account/createinvitation';
    const form = document.forms["invForm"];
    const submitBtn = form.querySelector("input[type='submit']");
    const antiforgey = form.querySelector("input[name='__RequestVerificationToken']").value;
    submitBtn.disabled = true;
    fetch(uri, {
        method: "POST",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "RequestVerificationToken": antiforgey },
        body: JSON.stringify(days)
    })
        .then(response => {
            if (response.status >= 200 && response.status < 300) {
                return response;
            } else {
                let error = new Error(response.statusText);
                error.response = response;
                throw error
            }
        })
        .then(response => response.json())
        .then(inv => {
            form.parentNode.appendChild(createInvitationInfoBlock(inv));
        })
        .catch(er => {
            document.getElementById("inv-error").append(er);
            document.getElementById("inv-error").style.display = "block";
        })
        .finally(() => submitBtn.disabled = false);
}

function createInvitationInfoBlock(invitation) {
    let readOnlyInputRow = function (value, title) {
        let row = document.createElement("div");
        row.classList.add("invite-info-block__row");

        let name = document.createElement("span");
        name.append(title);

        let val = document.createElement("input");
        val.setAttribute("readonly", "true");
        val.value = value;

        row.appendChild(name);
        row.appendChild(val);

        return row;
    }

    let formatDate = function (millisconds) {
        let padNum = (str) => { return str.toString().length < 2 ? "0" + str : str; };
        let expDate = new Date(millisconds);
        let dateStr = `${padNum(expDate.getUTCDate())}/${padNum(expDate.getUTCMonth() + 1)}/${expDate.getUTCFullYear()}`
            + ` ${padNum(expDate.getUTCHours())}:${padNum(expDate.getUTCMinutes())}:${padNum(expDate.getUTCSeconds())} UTC`;
        return dateStr;
    }

    let invBlock = document.createElement("div");
    invBlock.classList.add("invite-info-block");

    invBlock.appendChild(readOnlyInputRow(invitation["token"]["value"], invitation["token"]["localizedParamName"]));
    invBlock.appendChild(readOnlyInputRow(invitation["url"]["value"], invitation["url"]["localizedParamName"]));
    invBlock.appendChild(readOnlyInputRow(formatDate(invitation["expire"]["value"]), invitation["expire"]["localizedParamName"]));

    let brow = document.createElement("div");
    brow.classList.add("invite-info-block__row");
    let closeButton = document.createElement("input");
    closeButton.setAttribute("type", "button");
    closeButton.value = "OK";
    closeButton.addEventListener("click", () => invBlock.parentNode.removeChild(invBlock));
    brow.appendChild(closeButton);
    invBlock.appendChild(brow);

    return invBlock;
}
