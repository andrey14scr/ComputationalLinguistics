var textFile = document.getElementById('text');
var btnReparse = document.getElementById('btn-reparse');

var word, indexes;
var current = 0;

function findWord(direction) {
    if (direction === "next") {
        selectText(indexes[current], word);
        current++;
    } else if (direction === "prev") {
        selectText(indexes[current], word);
        current--;
    }

    if (current === indexes.length) {
        current = 0;
    }

    if (current === -1) {
        current += indexes.length;
    }
}

function selectText(index, word) {
    textFile.selectionStart = textFile.selectionEnd = index;
    textFile.blur();
    textFile.focus();
    textFile.setSelectionRange(index, index + word.length);
}

function reparseText(id) {
    btnReparse.disabled = true;
    var request = new XMLHttpRequest();
    request.open("POST", '/Texts/ReParse', true);

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