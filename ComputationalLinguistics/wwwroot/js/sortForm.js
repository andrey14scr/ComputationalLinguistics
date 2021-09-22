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