var select = document.getElementById('selectSort');
var input = document.getElementById('inputPattern');

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

function getWords(count, next){
    var request = new XMLHttpRequest();
    request.open('GET', `/Words/List?skip=${count}&next=${next}`, true);

    request.onload = function () {
        if (request.status >= 200 && request.status < 400) {
            var response = request.responseText;
            //comments.innerHTML += response;
            console.log(response);
        }
    }

    request.send();
}