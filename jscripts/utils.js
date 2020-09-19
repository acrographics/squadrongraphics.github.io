/* Initialisation for general pages */

addLoadListener(initRollovers);

/* Page steup functions */

function initRollovers() {
	if (!document.getElementById) return
	
	var aPreLoad = new Array();
	var tagsToConsider = new Array("img", "input");
	var sTempSrc;
	
	for (var j = 0; j < tagsToConsider.length; j++) {
		var aImages = document.getElementsByTagName(tagsToConsider[j]);
		
	//var aImages = document.getElementsByTagName('img');
	//var aFormInputs = document.getElementsByTagName('input');

		for (var i = 0; i < aImages.length; i++) {
		    var parentClass = "";
		    if (aImages[i].parentNode) parentClass = aImages[i].parentNode.className;		
			if (aImages[i].className == 'imgover' || parentClass == "imgover") {
				var src = aImages[i].getAttribute('src');
				var ftype = src.substring(src.lastIndexOf('.'), src.length);
				var hsrc = src.replace(ftype, '_over'+ftype);
	
				aImages[i].setAttribute('hsrc', hsrc);
				
				aPreLoad[i] = new Image();
				aPreLoad[i].src = hsrc;
				
				aImages[i].onmouseover = function() {
					sTempSrc = this.getAttribute('src');
					this.setAttribute('src', this.getAttribute('hsrc'));
				}	
				
				aImages[i].onmouseout = function() {
					if (!sTempSrc) sTempSrc = this.getAttribute('src').replace('_over'+ftype, ftype);
					this.setAttribute('src', sTempSrc);
				}
			}
		}
	}
}


/* Widely used functions */

function OpenDiscount() {
window.open("/discount.aspx", 'news', 'scrollbars,resizable,width=620,height=620');
return false;        
}

function OpenNews(url) {
window.open(url, 'news', 'scrollbars,resizable,width=620,height=620');  
return false;      
}

/* Utility functions */

function addLoadListener(fn)
{
  if (typeof window.addEventListener != 'undefined')
  {
    window.addEventListener('load', fn, false);
  }
  else if (typeof document.addEventListener != 'undefined')
  {
    document.addEventListener('load', fn, false);
  }
  else if (typeof window.attachEvent != 'undefined')
  {
    window.attachEvent('onload', fn);
  }
  else
  {
    var oldfn = window.onload;
    if (typeof window.onload != 'function')
    {
      window.onload = fn;
    }
    else
    {
      window.onload = function()
      {
        oldfn();
        fn();
      };
    }
  }
}

function attachEventListener(target, eventType, functionRef, capture)
{
  if (typeof target.addEventListener != "undefined")
  {
    target.addEventListener(eventType, functionRef, capture);
  }
  else if (typeof target.attachEvent != "undefined")
  {
    target.attachEvent("on" + eventType, functionRef);
  }
  else
  {
    eventType = "on" + eventType;

    if (typeof target[eventType] == "function")
    {
      var oldListener = target[eventType];

      target[eventType] = function()
      {
        oldListener();

        return functionRef();
      }
    }
    else
    {
      target[eventType] = functionRef;
    }
  }

  return true;
}

function getEventTarget(event)
{
  var targetElement = null;

  if (typeof event.target != "undefined")
  {
    targetElement = event.target;
  }
  else
  {
    targetElement = event.srcElement;
  }

  while (targetElement.nodeType == 3 && targetElement.parentNode != null)
  {
    targetElement = targetElement.parentNode;
  }

  return targetElement;
}

function getScrollingPosition()
{
  //array for X and Y scroll position
  var position = [0, 0];

  //if the window.pageYOffset property is supported
  if(typeof window.pageYOffset != 'undefined')
  {
    //store position values
    position = [
        window.pageXOffset,
        window.pageYOffset
    ];
  }

  //if the documentElement.scrollTop property is supported
  //and the value is greater than zero
  if(typeof document.documentElement.scrollTop != 'undefined'
    && document.documentElement.scrollTop > 0)
  {
    //store position values
    position = [
        document.documentElement.scrollLeft,
        document.documentElement.scrollTop
    ];
  }

  //if the body.scrollTop property is supported
  else if(typeof document.body.scrollTop != 'undefined')
  {
    //store position values
    position = [
        document.body.scrollLeft,
        document.body.scrollTop
    ];
  }

  //return the array
  return position;
}

function getElementsByAttribute(attribute, attributeValue)
{
  var elementArray = new Array();
  var matchedArray = new Array();

  if (document.all)
  {
    elementArray = document.all;
  }
  else
  {
    elementArray = document.getElementsByTagName("*");
  }

  for (var i = 0; i < elementArray.length; i++)
  {
    if (attribute == "class")
    {
      var pattern = new RegExp("(^| )" + attributeValue + "( |$)");

      if (elementArray[i].className.match(pattern))
      {
        matchedArray[matchedArray.length] = elementArray[i];
      }
    }
    else if (attribute == "for")
    {
      if (elementArray[i].getAttribute("htmlFor") || elementArray[i].getAttribute("for"))
      {
        if (elementArray[i].htmlFor == attributeValue)
        {
          matchedArray[matchedArray.length] = elementArray[i];
        }
      }
    }
    else if (elementArray[i].getAttribute(attribute) == attributeValue)
    {
      matchedArray[matchedArray.length] = elementArray[i];
    }
  }

  return matchedArray;
}

function replaceInnerText(element, text) {
    var elem = document.getElementById(element);
    if (elem!=null)
    {
        if (typeof(elem.text) != 'undefined')
	        elem.text = text;
        else if (typeof(elem.textContent) != 'undefined')
	        elem.textContent = text;
        else if (typeof(elem.innerText) != 'undefined')
	        elem.innerText = text;
	}
}

function op_jump(objSelect)
{
    if (objSelect.selectedIndex<=0) return false; 
    var objOpt = objSelect.options[objSelect.selectedIndex]; 
    window.location.href=objOpt.value;
}
    
// Removes leading whitespaces
String.prototype.ltrim = function()
{	
	var re = /\s*((\S+\s*)*)/;
	return this.replace(re, "$1");
	
}

// Removes ending whitespaces
String.prototype.rtrim = function()
{
	var re = /((\s*\S+)*)\s*/;
	return this.replace(re, "$1");
	
}

// Removes leading and ending whitespaces
String.prototype.trim = function()
{	
    this.ltrim().rtrim();	
}
