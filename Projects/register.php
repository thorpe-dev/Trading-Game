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
		   $num_rows = pg_num_rows($result);
           if ($num_rows == 0) {
		       echo 'registered as ' . $name;
			   $query = "INSERT INTO Players (username, password)
						  VALUES ('" . $name . "', '" . $password ."')";
			   $result = pg_query($db, $query);
			   if (!$result)
			   {
					echo "Problem with query ". $query;
			   }
				
		   }
		   
	   }
?>
