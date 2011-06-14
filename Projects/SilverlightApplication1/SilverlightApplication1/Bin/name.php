#!/usr/bin/php

<?php

if($_POST['name']) 
        { 
           $name = $_POST['name']; 
        } 
               
if($_POST['password']) 
        { 
           $password = $_POST['password']; 
        }                 
    //echo $name;
    //echo $password;
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
        if (!$result) { 
            echo "Problem with query " . $query . "<br/>"; 
        }
		     //$row = pg_fetch_assoc($result);
			 //$num = $row['playernumber'];
		
        while($row = pg_fetch_assoc($result)) {
		/*
		    echo $row['playernumber'];
			echo ' ';
			echo $row['username'];
			echo ' ';
			echo $row['password'];
			echo ' ';
			echo $row['playing];
			?><br></br><?php*/
			//if($row['username'] == $name && $row['password'] == $password)
			 //{
			   $num = $row['playernumber'];
			 //}
	    
        }


		$xml = '<players><person><playernumber>'. $num .'</playernumber><username>' . $name . '</username><password>' . $password . '</password></person></players>';
	    echo $xml;
	  }
?>
