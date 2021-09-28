var textFile = document.getElementById('text');

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