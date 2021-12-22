var textFile = document.getElementById('text');
var btnReparse = document.getElementById('btn-reparse');

function reparseText(id) {
    btnReparse.disabled = true;
    var request = new XMLHttpRequest();
    request.open("POST", '/Texts/ReTag', true);

    var params = {
        "id": id.toString(),
        "txt": textFile.value
    }

    request.onload = function () {
        if (request.status >= 200 && request.status < 400) {
            alert("Изменения успешно сохранены!");
        }
        btnReparse.disabled = false;
    }

    request.setRequestHeader("Content-Type", "application/json");
    request.send(JSON.stringify(params));
}