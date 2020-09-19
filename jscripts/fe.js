var frame_box_selected = 0; 

var mat_tab_selected=1;
var mat_box_selected = 0;
var select_mat1id=0;
var select_mat2id=0;
var default_select_mat_id=1;

var default_mat1_width=2.5;
var min_mat1_width=1.0;
var max_mat1_width=6.0;
var mat1_step=0.5;

var default_mat2_width=0.25;
var min_mat2_width=0.25;
var max_mat2_width=0.625;
var mat2_step=0.125;

var default_mat3_width=0.25;
var min_mat3_width=0.25;
var max_mat3_width=0.625;
var mat3_step=0.125;

var need_refresh=false;


function frame_box_over_unc(id)
{
    $("frame_box_"+id).style.border="solid 2px #0099cc";
}

function frame_box_out_unc(id)
{
  var box=$("frame_box_"+id);
  if (box!=undefined)
   box.style.border="solid 2px #FFFFFF";
}

function frame_box_select(id)
{
  if (id>0) 
    $("frame_box_"+id).style.border="solid 2px #70c132";
}

function frame_box_over(id)
{
  if(frame_box_selected != id)
    frame_box_over_unc(id);
}
function frame_box_out(id)
{
  if(frame_box_selected != id)
    frame_box_out_unc(id);
}

function sfr(id,name)
{
  setFrame(id,name);
  lazyRefresh();
}

function setFrame(id,name) 
{
    if(frame_box_selected!=0) frame_box_out_unc(frame_box_selected); 
    frame_box_selected=id;
    frame_box_select(id);
    if ($("fr").value != id)
    { 
        need_refresh=true;
        $("fr").value = id
    }
    if (id<=0)
     $("FrameName").value='';
    else 
     $("FrameName").value=name;
}

function mat_box_over_unc(id){
    $("mat_box_"+id).style.border="solid 2px #0099cc";
}
function mat_box_out_unc(id){
  $("mat_box_"+id).style.border="solid 2px #EDF5FF";
}

function mat_box_select(id){
    $("mat_box_"+id).style.border="solid 2px #70c132";
}

function mat_box_over(id){
  if(mat_box_selected != id){
    mat_box_over_unc(id);
  }
}

function mat_box_out(id){
  if(mat_box_selected != id){
    mat_box_out_unc(id);
  }
}

function smt(id,name)
{
  setMat(id,name);
  lazyRefresh();
}

function setMat(id,name)
{
    if(mat_box_selected!=0) mat_box_out_unc(mat_box_selected); 
    mat_box_selected=id;
    mat_box_select(id);
    $('MatWidth').innerHTML=($('mtt'+mat_tab_selected).value*1.0)+'"';
    if ($("mt"+mat_tab_selected).value!=id)
    { 
      $("mt"+mat_tab_selected).value = id;
      need_refresh=true;
    }
    if (id<=0)
     $("MatName").innerHTML='';
    else 
     $("MatName").innerHTML=name;
}

function ssmt(id,name,matid1,matid2)
{ 
  setSelectMat(id,name,matid1,matid2);
  lazyRefresh();
}

function setSelectMat(id,name,matid1,matid2)
{
    if(mat_box_selected!=0) mat_box_out_unc(mat_box_selected); 
    mat_box_selected=id;
    mat_box_select(id);
    if ($("smt").value!=id)
    { 
      $("smt").value = id;
      need_refresh=true;
    }
    if (id<=0)
     $("MatName").innerHTML='';
    else 
     $("MatName").innerHTML=name;
    
    $("MatN").value=2;
    $("mt1").value=matid1;
    $("mtt1").value=def_selmat_tb1;
    $("mtb1").value=def_selmat_tb1;
    $("mtr1").value=def_selmat_rl1;
    $("mtl1").value=def_selmat_rl1;
    
    $("mt2").value=matid2;
    $("mtt2").value=def_selmat_tb2;
    $("mtb2").value=def_selmat_tb2;
    $("mtr2").value=def_selmat_rl2;
    $("mtl2").value=def_selmat_rl2;
}

function setActiveMatTab(mat_tabnumber)
{ 
  if (mat_tab_selected!=mat_tabnumber)
  {
    mat_tab_selected=mat_tabnumber;
    var tab=matsView.getTab(mat_tabnumber-1);
    matsView.set('activeTab',tab);
    var matid=$("mt"+mat_tabnumber).value;
    if (matid>0)
    { setMat(matid);
    }
    else
     $('MatWidth').innerHTML='&#160;';
  }
}

function setMatN(number)
{
   var prevNumber=$("MatN").value;
   $("MatN").value = number;
   if (number>0)
   { 
     $("mat_tabs").style.display='block';
     $("mat1").style.display='block';
   }
   else
   { 
     $("mat_tabs").style.display='none';
     $("mat1").style.display='none';
   }
   var tab1=matsView.getTab(0);
   var tab2=matsView.getTab(1);
   var tab3=matsView.getTab(2);

   if (mat_tab_selected>number)
    setActiveMatTab(1);

   tab1.set('disabled',number<1);
   tab2.set('disabled',number<2);
   tab3.set('disabled',number<3);
   
   if (prevNumber!=number)
    need_refresh=true;
}

function changeMatWidth(direction)
{
  var mat_el="mt"+mat_tab_selected;
  var step=direction*eval("mat"+mat_tab_selected+"_step");
  $('mtt'+mat_tab_selected).value=1*$('mtt'+mat_tab_selected).value+step;
  if ($('mtt'+mat_tab_selected).value>eval("max_mat"+mat_tab_selected+"_width"))
   $('mtt'+mat_tab_selected).value=eval("max_mat"+mat_tab_selected+"_width");
  else if ($('mtt'+mat_tab_selected).value<eval("min_mat"+mat_tab_selected+"_width"))
   $('mtt'+mat_tab_selected).value=eval("min_mat"+mat_tab_selected+"_width");
  else
  { $('mtb'+mat_tab_selected).value=1*$('mtb'+mat_tab_selected).value+step;
    $('mtl'+mat_tab_selected).value=1*$('mtl'+mat_tab_selected).value+step;
    $('mtr'+mat_tab_selected).value=1*$('mtr'+mat_tab_selected).value+step;
    setMatWidthLabel();
    need_refresh=true;
  }
}

function setMatWidthLabel()
{ 
  $('MatWidth').innerHTML=($('mtt'+mat_tab_selected).value*1.0)+'"';
}

function setGlaze(id,name) 
{
  $("gl").value = id
  if (id<=0)
   $("GlazeName").value='';
  else
   $("GlazeName").value=name;
  $("glaze_rb"+id).checked=true;
}

function lazyRefresh()
{ 
  if (need_refresh)
  { 
    need_refresh=false;
    forceRefresh();
  }
}

function forceRefresh()
{ 
  //document.body.style.cursor = 'wait';
  setTimeout("UpdateImage()", 0);
  if (frtype=='c') startCustomPriceUpdate();
  else if (frtype=='s') updateSelectPrice();
}

function updateSelectPrice()
{
  var sum=PrintPrice;
  var fr_id=$("fr").value;
  if (fr_id>0)
   sum=sum+frameprices[fr_id];
  $('TotalPrice').innerHTML='$'+sum;
  $('frp').value=frameprices[fr_id];
  $('FinishedPrice').value=(sum).toFixed(2);
  $('FinishedWeight').value=frameweights[fr_id];
}

function priceRefresh()
{
  startCustomPriceUpdate();
}

scaleMore = 0;
function UpdateImage() 
{
    $("framedImage").src = "fs/get_framedimage.ashx?" + getQueryString();   
    
    
	//$('imgSmall').src = $('imgFramed').src;
	//if($('sizeId').value==2){
	//    scaleMore=1;
	//}else if($('sizeId').value==1){
	//    scaleMore=12;
	//}else{
    //	scaleMore=0;
	//}
    //$('imgSmall').width=(((7-$('sizeId').value)+12)*$('sizeId').value)+scaleMore;
}

function getQueryString() {
    var miw=490;
    var url = "pr1="+$("pr1").value;
    url +="&frtype="+frtype;
    if (frtype=='c')
      url += "&ft=" + 1;
    if ($("frp").value>0) 
    {
      url += "&frp=" + $("frp").value;
    }
    if ($("fr").value>0) 
    {
      url += "&fr=" + $("fr").value;
      miw=500;
    }
    var mat_number=$("MatN").value;
    if ($("mt1").value>0&&mat_number>0) 
    {
      url += "&mt1=" + $("mt1").value;
      if (!($("mtt1").value>0))
        $("mtt1").value=default_mat1_width;
      url += "&mtt1="+$("mtt1").value;
      if (!($("mtb1").value>0))
        $("mtb1").value=default_mat1_width;
      url += "&mtb1="+$("mtb1").value;
      if (!($("mtl1").value>0))
        $("mtl1").value=default_mat1_width;
      url += "&mtl1="+$("mtl1").value;
      if (!($("mtr1").value>0))
        $("mtr1").value=default_mat1_width;
      url += "&mtr1="+$("mtr1").value;
    }
    if ((mat_number>1)&&($("mt2").value>0))
    {
      url += "&mt2=" + $("mt2").value;
      if (!($("mtt2").value>0))
      $("mtt2").value=default_mat2_width;
      url += "&mtt2="+$("mtt2").value;
      if (!($("mtb2").value>0))
        $("mtb2").value=default_mat2_width;
      url += "&mtb2="+$("mtb2").value;
      if (!($("mtl2").value>0))
        $("mtl2").value=default_mat2_width;
      url += "&mtl2="+$("mtl2").value;
      if (!($("mtr2").value>0))
        $("mtr2").value=default_mat2_width;
      url += "&mtr2="+$("mtr2").value;
    }
    if ($("mt3").value>0&&mat_number>2) 
    {
      url += "&mt3=" + $("mt3").value;
      if (!($("mtt2").value>0))
        $("mtt3").value=default_mat3_width;
      url += "&mtt3="+$("mtt3").value;
      if (!($("mtb3").value>0))
        $("mtb3").value=default_mat3_width;
      url += "&mtb3="+$("mtb3").value;
      if (!($("mtl3").value>0))
        $("mtl3").value=default_mat3_width;
      url += "&mtl3="+$("mtl3").value;
      if (!($("mtr3").value>0))
        $("mtr3").value=default_mat3_width;
      url += "&mtr3="+$("mtr3").value;
    }
    
    if ($("gl").value>0) 
    {
      url += "&gl=" + $("gl").value;
    }
    return url+"&miw="+miw;
}

function startCustomPriceUpdate()
{
   if ($("pr1").value>0)
   { 
     var url = "fs/get_framedprice.aspx?"+getQueryString();
     //alert("url=" + url);
     makeHttpRequest(url,undefined,'framepricing');
   }
   document.body.style.cursor = 'default';
}

function writePriceRow(tbody,s1,s2)
{
  var tr=tbody.insertRow(-1);
  var td=tr.insertCell(0);
  td.innerHTML=s1;
  if(s2 == undefined)
   s2='';
  var td=tr.insertCell(1);
  td.align="right";
  td.innerHTML=s2;
}


function nodeMoney(xml,id)
{ 
  var tpnode = xml.getElementsByTagName(id)[0];
  if(tpnode != undefined)
  {
    return '$'+(tpnode.firstChild.data*1.0).toFixed(2);
  }
  return "";
}

function nodeSize(xml,id)
{ 
  var tpnode = xml.getElementsByTagName(id)[0];
  if(tpnode != undefined)
  {
    return (tpnode.firstChild.data*1.0).toFixed(3);
  }
  return "";
}

function framePricing(xml)
{ 
	 $('TotalPrice').innerHTML=nodeMoney(xml,'totalprice');
   	 var tpnode = xml.getElementsByTagName('totalprice')[0];
     if(tpnode != undefined)
     {
        $('FinishedPrice').value=(tpnode.firstChild.data*1.0).toFixed(2);
     }
     var tpnode = xml.getElementsByTagName('totalweight')[0];
     if(tpnode != undefined)
     {
        $('FinishedWeight').value=(tpnode.firstChild.data*1.0).toFixed(2);
     }

   	 var table=$('DetailsTable');
     var tbody = table.tBodies[0];
     for (var i=tbody.rows.length-1; i>0; i--) 
     {
       tbody.deleteRow(i);
     }
 
     writePriceRow(tbody,'Finished Size: '+nodeSize(xml,'width')+'" x '+nodeSize(xml,'height')+'"');
     
     var pnode = xml.getElementsByTagName('print')[0];
	 if(pnode != undefined)
     { 
       writePriceRow(tbody,'Print Size: '+nodeSize(pnode,'width')+'" x '+nodeSize(pnode,'height')+'"',nodeMoney(pnode,'price'));
     }
     
     var pnode = xml.getElementsByTagName('frame')[0];
     if(pnode != undefined)
     { 
       writePriceRow(tbody,'Frame Name: '+$("FrameName").value);
       writePriceRow(tbody,'Frame: '+nodeSize(pnode,'width')+'" x '+nodeSize(pnode,'height')+'"',nodeMoney(pnode,'price'));
     }
     
     var pnode = xml.getElementsByTagName('mat')[0];
     if(pnode != undefined)
     { 
       writePriceRow(tbody,'Top Mat: '+nodeSize(pnode,'width')+'" x '+nodeSize(pnode,'height')+'"',nodeMoney(pnode,'price'));
     }
     
     var pnode = xml.getElementsByTagName('mat')[1];
     if(pnode != undefined)
     { 
       writePriceRow(tbody,'Middle Mat: '+nodeSize(pnode,'width')+'" x '+nodeSize(pnode,'height')+'"',nodeMoney(pnode,'price'));
     }
     
     var pnode = xml.getElementsByTagName('mat')[2];
     if(pnode != undefined)
     { 
       writePriceRow(tbody,'Bottom Mat: '+nodeSize(pnode,'width')+'" x '+nodeSize(pnode,'height')+'"',nodeMoney(pnode,'price'));
     }
     
     var pnode = xml.getElementsByTagName('glaze')[0];
     if(pnode != undefined)
     { 
       writePriceRow(tbody,'Glazzing Type: '+$("GlazeName").value);
       writePriceRow(tbody,'Glazzing: '+nodeSize(pnode,'width')+'" x '+nodeSize(pnode,'height')+'"',nodeMoney(pnode,'price'));
     }
     
     var pnode = xml.getElementsByTagName('mounting')[0];
     if(pnode != undefined)
     { 
       writePriceRow(tbody,'Mounting Type: '+$("MountingName").value,nodeMoney(pnode,'price'));
       writePriceRow(tbody,'Mounting: '+nodeSize(pnode,'width')+'" x '+nodeSize(pnode,'height')+'"');
     }
     
     var pnode = xml.getElementsByTagName('fitting')[0];
     if(pnode != undefined)
     { 
       var cnode=xml.getElementsByTagName('caption')[0];
       writePriceRow(tbody,'Fitting Type: '+cnode.firstChild.data,nodeMoney(pnode,'price'));
     }
}


var animPriceDetailsHeight;
function initPriceDetails()
{
    animPriceDetailsHeight = new YAHOO.util.Anim('PriceDetails');
    animPriceDetailsHeight.duration = .1;
}
function togglePriceDetails()
{
    var pricingTableHeight = $("pricing_table_wrapper").offsetHeight;

    if($('PriceDetails').style.height == "0px"){
        animPriceDetailsHeight.attributes.height = { to: pricingTableHeight };
        animPriceDetailsHeight.onComplete.subscribe(function() { $('PriceDetails').style.height = 'auto'; } )
        animPriceDetailsHeight.animate();
    } 
    else
    {
        animPriceDetailsHeight.attributes.height = { from: pricingTableHeight, to: 0 };
        animPriceDetailsHeight.onComplete.subscribe(function() { $('PriceDetails').style.height = '0px'; } )
        animPriceDetailsHeight.animate();
    }
}

function setFrameMode(mode)
{
  if ((mode=='c'||mode=='s')&&$('frtype')!=mode)
  {
    $("mt1").value=0;
    $("mt2").value=0;
    $("mt3").value=0;
    $("fr").value=0;
  }
  $('frtype').value=mode;
  document.updatekit.submit();
}

function initFrameEngine() 
{ 
  var handleActiveMatTabChange = function(e) 
  { 
    setActiveMatTab(matsView.get('activeIndex')+1);
    lazyRefresh();
  };
  matsView.addListener('activeTabChange',handleActiveMatTabChange);
  
  setFramedAttributes();
  $("pr1").value=fixpr1;
  if (frtype=='c')
  { setMatN($("MatN").value);
    $("mat"+$('MatN').value+"rb").checked=true;
    mat_tab_selected=0;
    setActiveMatTab(1);
    setGlaze($("gl").value,$("GlazeName").value);
  }
  else
  { 
    mat_tab_selected=1;
  }
  setFrame($("fr").value,$("FrameName").value);
  initPriceDetails();
  forceRefresh();
}

function $(id){return document.getElementById(id);}

