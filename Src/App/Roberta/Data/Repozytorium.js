var mBrowserVersion = msieversion();
var mMouseDownFlag = false;
var mTable=null, mTdLeft=null, mTreeView=null, mSplitter=null, mHidden=null;
var min = 150;
var mDiff = null, mDiff2 = null;

function fOnBodyLoad()
{
    mMouseDownFlag = false;
    mTable = document.getElementById ('Tab');
    mTdLeft = document.getElementById ('tdLeft');
    mTreeView = document.getElementById (TREECLIENTID);
    mSplitter = document.getElementById ('tdSplitter');
    mHidden = document.getElementById (HIDDENLEFTCLIENTID);
    mDiff = mTreeView != null ? mSplitter.offsetLeft - mTreeView.offsetWidth : 0;
    mDiff2 = mTreeView != null ? mTdLeft.offsetWidth - mTreeView.offsetWidth : 0;
    
    if (mHidden && mHidden.value && mHidden.value.length > 0)
    {
        var p = parseInt(mHidden.value);
        if (!isNaN(p))
            fSetLeftWidth (p);
    }
}

function fOnMouseDown (event)
{
	event = event || window.event;
	mMouseDownFlag = true;
}

function fOnMouseMove(event)
{
	if (mMouseDownFlag)
		fSplitLayout (event);			
}

function fIsInSplitter (mouseX)
{
    return (mSplitter.offsetLeft <= mouseX) && ((mSplitter.offsetLeft + mSplitter.offsetWidth) >= mouseX)
}

function fSplitLayout (event)
{
    if (mTreeView == null)
        return;
        
	event = event || window.event;

	var x = mBrowserVersion == 0 ? event.pageX : event.x;
	    // jeœli wewn¹trz splittera to nic nie zmieniaj
//	if (fIsInSplitter (x))
//	    return;

    x -= mDiff;            
    var max = mTable.offsetWidth-100; // maksymalny rozmiar TreeView;            
                                      // 100 = minContentWidth + scrollWidth + padding + margin
    var p = Math.min (Math.max (min, x), max);
    fSetLeftWidth (p);
	if (mHidden)
	    mHidden.value = p;	   
}

function fSetLeftWidth (p)
{
	mTreeView.style.width = p +"px";
	mTdLeft.style.width = p + mDiff2 +"px";
}

function fOnMouseUp (event)
{
	if (mMouseDownFlag)
	{
		fSplitLayout (event);			
		mMouseDownFlag = false;
	}
}

function fOnSpliterMouseOver(event)
{
    event = event || window.event;            
    var tgt = event.target || event.srcElement;
    var cursor = mBrowserVersion == 0 ? 'e-resize' : 'col-resize';

    tgt.style.cursor = cursor;
}
		
function msieversion()
{
	var ua = window.navigator.userAgent;
	var msie = ua.indexOf ("MSIE ");
	if ( msie > 0 )      // If Internet Explorer, return version number
		return parseInt (ua.substring (msie+5, ua.indexOf (".", msie )))
	else                 // If another browser, return 0
		return 0;
}

function populate(month, year, day)
{ 
    var e = document.getElementById(month); 
    var y = document.getElementById(year); 
    var d = document.getElementById(day); 
    var temp = 0; 
    
    if((e.options[e.selectedIndex].value != 'Default') && (y.options[y.selectedIndex].text != '-')) 
    { 
        timeA = new Date(y.options[y.selectedIndex].value, e.options[e.selectedIndex].value, 1); 
        timeDifference = timeA - 86400000; 
        timeB = new Date(timeDifference); 
        
        var daysInMonth = timeB.getDate(); 
        temp = d.selectedIndex; 
        
        for (var i = 1; i < d.length; i++) { d.options[1] = null; } 
        for (var i = 1; i <= daysInMonth; i++){ d.options[i] = new Option(i); } 
        if (temp <= daysInMonth )
        {   
            d.options[temp].selected = true; 
        } 
        else 
        {
            d.options[daysInMonth].selected = true;
        }
    } 
    else 
    {
        var len = d.length; 
        for (var i = 0; i < len; i++) { d.options[d.length-1] = null; } 
        d.length = 1; 
        d.options[0].text = '-';
    }
}
