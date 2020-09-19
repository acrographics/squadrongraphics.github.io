
<!doctype html public "-//w3c//dtd html 3.2//en">

<html>

<head>
<title>Multiple image upload</title>
</head>

<body bgcolor="#ffffff" text="#000000" link="#0000ff" vlink="#800080" alink="#ff0000">
<?
	
while(list($key,$value) = each($_FILES['images']['name']))
		{
			if(!empty($value))
			{
				$filename = $value;
					$filename=str_replace(" ","_",$filename);// Add _ inplace of blank space in file name, you can remove this line

					$add = "upload/$filename";
                       //echo $_FILES['images']['type'][$key];
			     // echo "<br>";
					copy($_FILES['images']['tmp_name'][$key], $add);
					chmod("$add",0777);
			

			}
		}


?>
</body>

</html>
