<!doctype html public "-//w3c//dtd html 3.2//en">

<html>

<head>
<title>Multiple image upload</title>
</head>

<body bgcolor="#ffffff" text="#000000" link="#0000ff" vlink="#800080" alink="#ff0000">
<?
$max_no_img=4;  // Maximum number of images value to be set here
echo "<form method=post action=addimgck.php enctype='multipart/form-data'>";
echo "<table border='0' width='400' cellspacing='0' cellpadding='0' align=center>";
for($i=1; $i<=$max_no_img; $i++){
echo "<tr><td>Images $i</td><td>
	<input type=file name='images[]' class='bginput'></td></tr>";
}
echo "<tr><td colspan=2 align=center><input type=submit value='Add Image'></td></tr>"; 

echo "</form> </table>";



?>

</body>

</html>
