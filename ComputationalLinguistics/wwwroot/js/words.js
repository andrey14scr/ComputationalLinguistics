var select = document.getElementById('selectSort');
var input = document.getElementById('inputPattern');
var tableBody = document.getElementById('tableBody');
var addingBtn = document.getElementById('adding-words-btn');
var blockIndex = 1;
var scrollDiv = document.getElementById('div-table');

var isCompleteDownload = true;

select.onchange = checkHidden;

var tooltipElem;

document.onmouseover = function (event) {
    var target = event.target;

    if (!target.classList.contains('tag-tooltip')) {
        return;
    }

    var tooltip = getTagName(target.innerText);

    tooltipElem = document.createElement('div');
    tooltipElem.className = 'mytooltip';
    tooltipElem.innerHTML = tooltip;
    document.body.append(tooltipElem);

    var coords = target.getBoundingClientRect();

    var left = coords.left + (target.offsetWidth - tooltipElem.offsetWidth) / 2;
    if (left < 0)
        left = 0;

    var top = coords.top - tooltipElem.offsetHeight - 5;
    if (top < 0)
        top = coords.top + target.offsetHeight + 5;

    //tooltipElem.style.position = 'absolute';
    tooltipElem.style.left = left + 'px';
    tooltipElem.style.top = top + 'px';
};

document.onmouseout = function (e) {
    if (tooltipElem) {
        tooltipElem.remove();
        tooltipElem = null;
    }
};

function scrolled(blockSize, sortBy, pattern) {
    if (isCompleteDownload && scrollDiv.offsetHeight + scrollDiv.scrollTop >= scrollDiv.scrollHeight - 3) {
        isCompleteDownload = false;
        getWords(blockSize, sortBy, pattern);
    }
}

function checkHidden(){    
    if (select.options[select.selectedIndex].value === '_pattern' || select.options[select.selectedIndex].value === '_annotation') {
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
            isCompleteDownload = true;
        }
    }

    request.send();
}

function getTagName(name) {
    var request = new XMLHttpRequest();
    request.open('GET', `/Help/GetTagByName?name=${name}`, false);
    request.send();

    return request.response;
}