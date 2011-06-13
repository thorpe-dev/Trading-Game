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
		   $query = "SELECT * FROM Players WHERE Players.username='$name'";
	       $result = pg_query($db,$query); 
           if (!$result) 
		        { 
                   echo "Problem with query " . $query . "<br/>"; 
                }
		   $register = true;
           while($row = pg_fetch_assoc($result)) {
			   $register = false;
           }
           if ($register) {
		       echo 'registered as ' . $name;
		   }
	   }
?>
