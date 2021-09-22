var select = document.getElementById('selectSort');
var input = document.getElementById('inputPattern');

select.onchange = checkHidden;
let temp;

function checkHidden(){    
    if(select.options[select.selectedIndex].value === '_pattern'){
        input.parentNode.style.padding = temp;
        input.value = '';
        input.hidden = false;
    }
    else{
        temp = input.style.padding;
        input.hidden = true;
        input.parentNode.style.padding = '0';
    }
}