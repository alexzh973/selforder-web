/*
*       checkNumeric(nameField,defValue)
*       myParseFloat(val)
*       myRound(num)
*       myRoundMoney(num)
*/

var DigitSymbol = String(3/2).charAt(1);
var	WrongDigitSymbol= (DigitSymbol == ".") ? "," : ".";

function checkNumeric(nameField,defValue) {
    var valueField = $("#"+nameField).val(); // this.document.forms[0][nameField].value;
	if (valueField.length == 0) { 
		$("#"+nameField).val(defValue); //this.document.forms[0][nameField].value = defValue;
	}
	else {
		var positSpace;
		while (valueField.indexOf(" ") !=-1) {
			positSpace = valueField.indexOf(" ");
			valueField = valueField.substring(0,positSpace)+valueField.substring(positSpace+1,valueField.length);
		}
		if (valueField.length == 0) { 
			this.document.forms[0][nameField].value = defValue;
		}
		else {
			var arStr = valueField.split(WrongDigitSymbol);
			if (arStr.length>1) {
				retStr = (arStr[1].length>0 && String(1*arStr[1]) != 'NaN') ? arStr[0]+DigitSymbol+arStr[1] : arStr[0];		
			}
			else {
				retStr = valueField;
			}
			this.document.forms[0][nameField].value = (String(1*retStr) == 'NaN') ? defValue : retStr;
		}
	}
}

function myParseFloat(val){
	return parseFloat(myroundEx(String(val).replace(' ',''),7));
}


function roundNum777_(x)
{
    var DigitSymbol = String(3/2).charAt(1);
	var p,i,j;
	var s="", s2="";
		p=roundEx(x,3);	s=p.toString();
		p=s.lastIndexOf(DigitSymbol);
		if (p>0) {s2=DigitSymbol + s.substring(p+1, s.length); s = s.substring(0,p);} else {s2 = DigitSymbol+"00";};
		if(s2.length<3)s2+="0";	j=1; i=s.length;
		for((i%3)?i=Math.floor(i/3):i=Math.floor(i/3)-1; i>0;--i) {
			p=s.length-(3*j)-(j-1);j++;
			s=s.substring(0,p) + ""+ s.substring(p,s.length);
		}
		return(s+s2);
}

function roundEx777_(x,d) {
	var p, n, dgt = 3;
	p=Math.floor(Math.pow(10,dgt));
	(x>0) ? n = Math.floor(x) : n = Math.ceil(x);
	return(n + Math.round((x-n)*p)/p);
}

function myRound(num) {
	return parseFloat(myroundEx(num,3));
}

function myRoundMoney(num){
	return myRound(num);
}
function myRoundMoney_(num) {
	var d = parseFloat(myroundEx(num,3));
	var dd = "";
	var str = String(d); 
	var retVal;
	if(str.indexOf(DigitSymbol)!=-1) 
	{
		var s = str.split(DigitSymbol);		
		if (s[1].length == 2) {dd = DigitSymbol+s[1];}
		if (s[1].length == 1) {dd = DigitSymbol+s[1]+"0";}
		if (s[1].length == 0) {dd=DigitSymbol+"00";}
		retVal = s[0] +dd;
	}
	else {
		dd= DigitSymbol+"00";
		retVal = str+DigitSymbol+"00";
	}
	return retVal;
}


function myroundEx(num,qtyD) {
	return Math.round(num*Math.pow(10,qtyD))/Math.pow(10,qtyD);
}
