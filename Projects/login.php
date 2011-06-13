#!/usr/bin/php

<?php

if($_POST['name'] && $_POST['password']) 
        { 
           $name = $_POST['name'];
		   $password = $_POST['password'];
        }
$db = pg_connect('host=db port=5432 dbname=g1027131_u user=g1027131_u password=YRAkDJ2Xcr'); 
if(!$db) 
	   {
	       echo 'fail!';
	   }
else
	   {
	       $num = '0';
		   $query = "SELECT * FROM Players WHERE Players.username='$name' AND Players.password='$password'";
	       $result = pg_query($db,$query); 
           if (!$result) 
		        { 
                   echo "Problem with query " . $query . "<br/>"; 
                }
		
           while($row = pg_fetch_assoc($result)) {
			   $num = $row['playernumber'];
           }
           $xml = '<players><person><playernumber>'. $num .'</playernumber><username>' . $name . '</username><password>' . $password . '</password></person></players>';
	       echo $xml;
	   }
?>
