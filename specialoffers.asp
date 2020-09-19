<!DOCTYPE html>
<html>
<style type="text/css">
</style>
<body>

<p>
  <%
'declare the variables
Dim Connection
Dim ConnString
Dim Recordset
Dim SQL

'define the connection string, specify database driver
ConnString="DRIVER={SQL Server};SERVER=sql2k5f.appliedi.net;UID=squadron;" & _
"PWD=Squ@dr0n;DATABASE=SQGI"

'declare the SQL statement that will query the database
SQL = "SELECT Product.ProductID, Product.Name, Product.SKU, Section_1.Name AS Base, Section.Name AS Unit, Product.ImageFilenameOverride FROM Product INNER JOIN ProductSection ON Product.ProductID = ProductSection.ProductID INNER JOIN Section ON ProductSection.SectionID = Section.SectionID INNER JOIN Section AS Section_1 ON Section.ParentSectionID = Section_1.SectionID WHERE (Product.ProductID ="&Request.QueryString("ProductID")&")"

'create an instance of the ADO connection and recordset objects
Set Connection = Server.CreateObject("ADODB.Connection")
Set Recordset = Server.CreateObject("ADODB.Recordset")

'Open the connection to the database
Connection.Open ConnString

'Open the recordset object executing the SQL statement and return records
Recordset.Open SQL,Connection
%>
</p>
<p>Squadron Graphics, Inc.</p>
<p>Unframed Order Form</p>
<div> <img src='images/product/large/<%=Recordset("ImageFilenameOverride")%>' width="200" height="134"></div>
<table width="800" border="1">
  <tbody>
    <tr>
      <td><table width="100%" border="1">
        <tbody>
          <tr>
            <td colspan="2"><strong>Billing Address</strong></td>
            </tr>
          <tr>
            <td width="14%">Name</td>
            <td width="86%"><input name="textfield6" type="text" id="textfield6" size="45"></td>
          </tr>
          <tr>
            <td>Address</td>
            <td><input name="textfield7" type="text" id="textfield7" size="45"></td>
          </tr>
          <tr>
            <td>Address</td>
            <td><input name="textfield8" type="text" id="textfield8" size="45"></td>
          </tr>
          <tr>
            <td>City</td>
            <td><input name="textfield9" type="text" id="textfield9" size="45"></td>
          </tr>
          <tr>
            <td>State</td>
            <td><input name="textfield10" type="text" id="textfield10" size="45"></td>
          </tr>
          <tr>
            <td>Zip</td>
            <td><input name="textfield11" type="text" id="textfield11" size="45"></td>
          </tr>
        </tbody>
      </table></td>
      <td><table width="100%" border="1">
        <tbody>
          <tr>
            <td colspan="2"><strong>Shipping Address</strong></td>
          </tr>
          <tr>
            <td width="14%">Name</td>
            <td width="86%"><input name="textfield13" type="text" id="textfield12" size="45"></td>
          </tr>
          <tr>
            <td>Address</td>
            <td><input name="textfield14" type="text" id="textfield13" size="45"></td>
          </tr>
          <tr>
            <td>Address</td>
            <td><input name="textfield15" type="text" id="textfield14" size="45"></td>
          </tr>
          <tr>
            <td>City</td>
            <td><input name="textfield16" type="text" id="textfield15" size="45"></td>
          </tr>
          <tr>
            <td>State</td>
            <td><input name="textfield17" type="text" id="textfield16" size="45"></td>
          </tr>
          <tr>
            <td>Zip</td>
            <td><input name="textfield12" type="text" id="textfield17" size="45"></td>
          </tr>
        </tbody>
      </table></td>
    </tr>
  </tbody>
</table>
<table width="800" border="1" cellpadding="3">
  <tbody>
    <tr>
      <td width="7%">Print #</td>
      <td width="51%">Description</td>
      <td width="7%">QTY</td>
      <td width="11%">Price</td>
      <td width="13%">QTY Ordered</td>
      <td width="11%">Total</td>
    </tr>
    <tr>
      <td rowspan="5"><%=Recordset("SKU")%></td>
      <td rowspan="5" style="text-align: center"><p><%=Recordset("name")%></p>
      <p><%=Recordset("Unit")%></p>
      <p><%=Recordset("Base")%></p></td>
      <td>1-10</td>
      <td>$26.00 ea.</td>
      <td>
      <input type="text" name="textfield" id="textfield"></td>
      <td>&nbsp;</td>
    </tr>
    <tr>
      <td>11-24</td>
      <td>$21.00 ea.</td>
      <td><input type="text" name="textfield2" id="textfield2"></td>
      <td>&nbsp;</td>
    </tr>
    <tr>
      <td>25+</td>
      <td>$16.40 ea.</td>
      <td><input type="text" name="textfield3" id="textfield3"></td>
      <td>&nbsp;</td>
    </tr>
    <tr>
      <td>100</td>
      <td>$1,350.00</td>
      <td><input type="text" name="textfield4" id="textfield4"></td>
      <td>&nbsp;</td>
    </tr>
    <tr>
      <td>200</td>
      <td>$1,650.00</td>
      <td><input type="text" name="textfield5" id="textfield5"></td>
      <td>&nbsp;</td>
    </tr>
    <tr>
      <td colspan="3" rowspan="2">&nbsp;</td>
      <td colspan="2">&nbsp;</td>
      <td>&nbsp;</td>
    </tr>
    <tr>
      <td colspan="2">&nbsp;</td>
      <td>&nbsp;</td>
    </tr>
  </tbody>
</table>
<form id="form1" name="form1" method="post">
</form>
<p>&nbsp;</p>
</body>
</html> 

<%

'close the connection and recordset objects to free up resources
Recordset.Close
Set Recordset=nothing
Connection.Close
Set Connection=nothing
%>