// -------------------------------------------
// -- Utils
// -------------------------------------------

// IMPORTANT: the order of operators in this string should be the inverse of precedence order for calculations purposes
const operatorsStr = '+-*/';

let getLast = function(str) {
    return str[str.length - 1];
}

let removeLast = function(str) {
    return str.substring(0,str.length-1);
}

let strContains = function(str, c) {
    return str.indexOf(c) >= 0;
}

let isOperator = function(c) {
    return strContains(operatorsStr, c);
}

let binaryStrToNumber = function(binStr) {
    return parseInt(binStr, 2);
}

let numberToBinaryStr = function(n) {
    return n.toString(2);
}

let performOperator = function(op, operands) {
    return eval(operands.map((operand) => { return operand.toString(); }).join(op));
}

let getLeftRightOperands = function(op, formula) {
    let pos = formula.indexOf(op);
    return [
        formula.substring(0, pos),
        formula.substring(pos+1, formula.length),
    ];
}

let calculate = function(formula) {
    // sanitizing formula
    if (isOperator(getLast(formula))) formula = removeLast(formula);
    
    // 
    for (let op of operatorsStr) {
        if (strContains(formula, op)) {
            return numberToBinaryStr(performOperator(op, getLeftRightOperands(op, formula).map((operand) => { return binaryStrToNumber(calculate(operand)); })));
        }
    }
    
    return formula;
}

// -------------------------------------------
// -- UI
// -------------------------------------------

let setRes = function(txt) {
    document.getElementById('res').innerHTML = txt;
}

let getRes = function() {
    return document.getElementById('res').innerHTML;
}

let appendOnRes = function(txt) {
    document.getElementById('res').innerHTML += txt;
}

let setOperator = function(op) {
    let formula = getRes();
    if (formula.length > 0) {
        if (isOperator(getLast(formula)))
            setRes(removeLast(formula) + op);
        else
            appendOnRes(op);
    }
}

let btn0Press = function() { appendOnRes('0'); }

let btn1Press = function() { appendOnRes('1'); }

let btnClrPress = function() { setRes(''); }

let btnSumPress = function() { setOperator('+'); }

let btnSubPress = function() { setOperator('-'); }

let btnMulPress = function() { setOperator('*'); }

let btnDivPress = function() { setOperator('/'); }

let btnEqlPress = function() { setRes(calculate(getRes())); }
