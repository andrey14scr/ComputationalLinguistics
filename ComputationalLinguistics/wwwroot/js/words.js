var select = document.getElementById('selectSort');
var input = document.getElementById('inputPattern');
var tableBody = document.getElementById('tableBody');
var addingBtn = document.getElementById('adding-words-btn');
var blockIndex = 1;

select.onchange = checkHidden;

function checkHidden(){    
    if (select.options[select.selectedIndex].value === '_pattern') {
        input.parentNode.classList.add("p-2");
        input.value = '';
        input.hidden = false;
    }
    else {
        input.parentNode.classList.remove("p-2");
        input.hidden = true;
    }
}

function getWords(blockSize, sortBy, pattern) {
    var request = new XMLHttpRequest();
    var skip = blockIndex * blockSize;
    blockIndex++;

    var sorting = "";
    var patterning = "";

    if (sortBy !== null) {
        sorting = `&sortBy=${sortBy}`;
    }

    if (pattern !== null) {
        patterning = `&pattern=${pattern}`;
    }

    request.open('GET', `/Words/List?skip=${skip}&next=${blockSize}${sorting + patterning}`, true);

    request.onload = function () {
        if (request.status >= 200 && request.status < 400) {
            var response = request.responseText;
            if (response.length < 10) {
                addingBtn.hidden = true;
            }
            tableBody.innerHTML += response;
        }
    }

    request.send();
}